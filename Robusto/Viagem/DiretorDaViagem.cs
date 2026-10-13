using System;
using System.Linq;
using System.Text.Json;
using Godot;
using ProjetoPlanetario.Apresentacao.Cenarios;
using ProjetoPlanetario.Apresentacao.HUD;
using ProjetoPlanetario.Jogabilidade.Viagem;
using ProjetoPlanetario.Robusto.Cenarios;
using ProjetoPlanetario.Robusto.Dominio;
using ProjetoPlanetario.Robusto.Entrada;
using ProjetoPlanetario.Robusto.Eventos;
using ProjetoPlanetario.Robusto.Projecao;
using ProjetoPlanetario.Robusto.UI;

namespace ProjetoPlanetario.Robusto.Viagem;

public partial class DiretorDaViagem : Node3D
{
    [Export(PropertyHint.File, "*.json")] public string ArquivoRoteiro { get; set; } = "res://Robusto/Dados/roteiro.json";
    [Export] public ConfiguracaoExibicao ConfiguracaoExibicao { get; set; } = null!;
    [Export] public string EtapaInicialDebug { get; set; } = "terra";
    [Export] public bool AtalhosDebug { get; set; } = true;
    [Export] public bool PausarAoPerderFoco { get; set; } = true;
    [Export(PropertyHint.Range, "0.1,10")] public float Ritmo { get; set; } = 1;
    public SequenciaDaViagem Sequencia { get; private set; } = null!;
    public GerenciadorCenarios Cenarios { get; private set; } = null!;
    public GerenciadorExibicao Exibicao { get; private set; } = null!;
    public PilotoAutomatico Nave { get; private set; } = null!;
    public CombateDeMarte Combate { get; private set; } = null!;
    public EventoDoGelo Gelo { get; private set; } = null!;
    public Camera3D Camera { get; private set; } = null!;
    public ControladorDoOlhar Olhar { get; private set; } = null!;
    public ContextoDeEntrada Contexto => !_pronto || Erro.Length > 0 ? ContextoDeEntrada.Bloqueado :
        Interface.MenuAberto || Sequencia.Terminou ? ContextoDeEntrada.Menu : Sequencia.Atual.Tipo switch
        {
            "combate" => ContextoDeEntrada.Combate,
            "gelo" => ContextoDeEntrada.Gelo,
            "salto" => ContextoDeEntrada.Bloqueado,
            _ => ContextoDeEntrada.Olhar
        };
    public InterfaceDaViagem Interface { get; private set; } = null!;
    public ControladorDoSalto Salto { get; } = new();
    public string Erro { get; private set; } = "";
    private AudioDaViagem _audio = null!;
    private ApresentacaoDoSalto _efeito = null!;
    private bool _pronto;
    private bool _recarregarEtapa;

    public override void _Ready()
    {
        try
        {
            EntradaDaViagem.Configurar();
            Nave = GetNode<PilotoAutomatico>("Nave");
            Camera = Nave.GetNode<Camera3D>("CameraRig/CameraPiloto");
            Olhar = Nave.GetNode<ControladorDoOlhar>("CameraRig");
            Cenarios = GetNode<GerenciadorCenarios>("SistemasPersistentes/GerenciadorCenarios");
            _audio = GetNode<AudioDaViagem>("SistemasPersistentes/AudioDaViagem");
            Combate = GetNode<CombateDeMarte>("CombateDeMarte");
            Gelo = GetNode<EventoDoGelo>("SistemasPersistentes/EventoDoGelo");
            var args = OS.GetCmdlineUserArgs();
            ConfiguracaoExibicao = (ConfiguracaoExibicao)(ConfiguracaoExibicao ?? new ConfiguracaoExibicao()).Duplicate();
            if (args.Contains("--fulldome")) ConfiguracaoExibicao.FulldomePreview = true;
            if (DisplayServer.GetName() != "headless") GetWindow().Mode = args.Contains("--janela") || !ConfiguracaoExibicao.TelaCheia ? Window.ModeEnum.Windowed : Window.ModeEnum.Fullscreen;
            Exibicao = new GerenciadorExibicao { Name = "Exibicao", Configuracao = ConfiguracaoExibicao };
            Exibicao.Configurar(Camera);
            AddChild(Exibicao);
            _efeito = new ApresentacaoDoSalto { Name = "ApresentacaoDoSalto" };
            _efeito.Configurar(Salto, Nave);
            AddChild(_efeito);
            var camada = new CanvasLayer { Name = "Interface", Layer = 10 };
            AddChild(camada);
            Interface = new InterfaceDaViagem { Name = "HUD" };
            Interface.Configurar(Exibicao);
            camada.AddChild(Interface);
            Interface.ReiniciarSolicitado += () => Reiniciar();
            Interface.MenuAlterado += _ => Olhar.DefinirContexto(Contexto);
            Interface.ContinuarSolicitado += () => { if (Erro.Length > 0) EntrarEtapa(Sequencia.Atual); Interface.MostrarMenu(false); };
            Interface.SairSolicitado += () => GetTree().Quit();
            var dados = JsonSerializer.Deserialize<EtapaDaViagem[]>(FileAccess.GetFileAsString(ArquivoRoteiro)) ?? Array.Empty<EtapaDaViagem>();
            Sequencia = new SequenciaDaViagem(dados);
            Sequencia.Alterada += EntrarEtapa;
            var inicio = OS.IsDebugBuild() ? EtapaInicialDebug : "terra";
            if (OS.IsDebugBuild()) inicio = args.FirstOrDefault(a => a.StartsWith("--etapa="))?.Substring(8) ?? inicio;
            _pronto = true;
            Sequencia.IrPara(inicio);
            if (args.Contains("--calibracao")) { Exibicao.DefinirModo(true); Exibicao.Calibracao = true; Interface.MostrarMenu(true); }
        }
        catch (Exception erro)
        {
            Erro = erro.Message;
            GD.PushError(erro.ToString());
            if (Interface is not null) Interface.Apresentar("ERRO NA VIAGEM", Erro, "", -1, false, false);
        }
    }
    public void Reiniciar() { Erro = ""; IrParaEtapa("terra"); }
    public void IrParaEtapa(string id)
    {
        if (!_pronto) return;
        _recarregarEtapa = true;
        Olhar.Recentralizar();
        Sequencia.IrPara(id);
    }
    private void EntrarEtapa(EtapaDaViagem etapa)
    {
        Erro = "";
        Combate.Limpar();
        Gelo.Limpar();
        Salto.Reiniciar();
        _efeito.Ocultar();
        Nave.Desvio = 0;
        _audio.Reproduzir(etapa.Audio);
        if (etapa.Tipo == "salto")
        {
            Salto.EscalaDuracao = Mathf.Max(0.2f, etapa.Duracao / 5);
            Salto.IniciarAutomatico(etapa.Cenario);
        }
        else
        {
            if ((_recarregarEtapa || Cenarios.ObterIdentificadorAtual() != etapa.Cenario) && !Carregar(etapa.Cenario)) return;
            var cenario = (CenarioDaViagem)Cenarios.CenarioAtual!;
            if (etapa.Tipo == "combate") Combate.Iniciar(cenario.GetNode<Node3D>("Jogabilidade/PontosDeAparecimento"), Nave.GlobalPosition);
            if (etapa.Tipo == "gelo") Gelo.Iniciar(cenario);
        }
        Interface.Mira = GetViewport().GetVisibleRect().Size / 2;
        _recarregarEtapa = false;
        Olhar.DefinirContexto(Contexto);
        GD.Print("[VIAGEM] " + etapa.Id);
    }
    private bool Carregar(string id)
    {
        if (!Cenarios.CarregarCenario(id))
        {
            Erro = Cenarios.UltimoErro;
            Interface.Apresentar("VIAGEM INTERROMPIDA", Erro, "", -1, false, false);
            GD.PushWarning(Erro);
            return false;
        }
        Nave.DefinirRota(((CenarioDaViagem)Cenarios.CenarioAtual!).Rota);
        return true;
    }
    public override void _Process(double delta)
    {
        if (_pronto) Olhar.DefinirContexto(Contexto);
        if (!_pronto || Erro.Length > 0 || Interface.MenuAberto) return;
        var dt = Mathf.Min((float)delta, 0.1f) * Ritmo;
        var etapa = Sequencia.Atual;
        Cenarios.CenarioAtual?.AtualizarAmbiente(dt);
        var concluido = false;
        if (etapa.Tipo == "salto")
        {
            Salto.Atualizar(dt);
            if (Salto.PrecisaCarregar) Salto.ConfirmarCarregamento(Carregar(etapa.Cenario));
            concluido = Salto.Fase == FaseDoSalto.Chegada;
        }
        var entrada = EntradaDaViagem.LerMira();
        // O olhar usa tempo real, independente da aceleracao de testes do roteiro.
        Olhar.Atualizar(Mathf.Min((float)delta, 0.1f), entrada);
        Combate.Atualizar(dt, Nave.GlobalPosition);
        if (Contexto == ContextoDeEntrada.Combate)
        {
            var tamanho = GetViewport().GetVisibleRect().Size;
            Interface.Mira = (Interface.Mira + entrada * 460 * (float)delta).Clamp(Vector2.One * 16, tamanho - Vector2.One * 16);
            if (Input.IsActionPressed("disparar"))
            {
                var direcao = Exibicao.DirecaoNaTela(Interface.Mira);
                if (!direcao.IsZeroApprox()) Combate.Disparar(Camera.GlobalPosition, direcao);
            }
            if (Input.IsActionJustPressed("escudo")) Combate.AtivarEscudo();
            concluido = Combate.Concluido;
        }
        if (Contexto == ContextoDeEntrada.Gelo)
        {
            Gelo.Atualizar(dt, entrada.X);
            Nave.Desvio = Gelo.Desvio;
            concluido = Gelo.Concluido;
        }
        // Durante o alerta o obstaculo mantem distancia; nao ha colisao inevitavel por falta de entrada.
        var velocidade = Gelo.Regra?.Fase == FaseDoGelo.Alerta ? 0 : etapa.Velocidade;
        Nave.Avancar(dt, velocidade);
        Interface.MiraVisivel = etapa.Tipo == "combate";
        var estado = etapa.Tipo == "combate" ? $"DEFESA / {Combate.Restantes} CONTATOS" + (Combate.Escudo > 0 ? " / ESCUDO ATIVO" : "") : "";
        var progresso = -1f;
        if (Gelo.Regra is { } gelo)
        {
            estado = gelo.Fase switch { FaseDoGelo.Alerta => "ALERTA / CORRECAO ESQUERDA OU DIREITA", FaseDoGelo.Evasao => "EVASAO EM CURSO", _ => "OBJETO EM APROXIMACAO" };
            progresso = gelo.Correcao;
        }
        Interface.Apresentar(etapa.Titulo, etapa.Texto, estado, progresso, etapa.Id == "ana_branca", Sequencia.Terminou);
        Interface.InformacaoDebug = $"{etapa.Id} | {Cenarios.ObterIdentificadorAtual()} | {Nave.Distancia:F1} m | {Engine.GetFramesPerSecond()} FPS\nSalto: {Salto.Fase} | Projecao: {(Exibicao.Fulldome ? "PREVIEW" : "NORMAL")} | Entrada: {Contexto}\nVisao: yaw {Olhar.Estado.Yaw:F1} / pitch {Olhar.Estado.Pitch:F1}";
        var permitirAvanco = true;
        if (etapa.Id is "marte" or "urano" && Cenarios.CenarioAtual?.GetNodeOrNull<Marker3D>("Jogabilidade/Gatilhos/InicioEvento") is { } marcador && Nave.Rota is { } rota)
            permitirAvanco = Nave.Distancia >= rota.Curve.GetClosestOffset(rota.ToLocal(marcador.GlobalPosition));
        Sequencia.Atualizar(dt, concluido, permitirAvanco);
    }
    public override void _Input(InputEvent evento)
    {
        if (!_pronto) return;
        Olhar.DefinirContexto(Contexto);
        if (evento is InputEventMouseMotion movimento)
        {
            if (Contexto == ContextoDeEntrada.Olhar) Olhar.ReceberMouse(movimento.ScreenRelative);
            else if (Contexto == ContextoDeEntrada.Combate) Interface.Mira = movimento.Position;
        }
        if (evento.IsActionPressed("recentralizar_visao") && Contexto == ContextoDeEntrada.Olhar) Olhar.Recentralizar();
        if (evento is not InputEventKey { Pressed: true, Echo: false } tecla) return;
        var codigo = tecla.PhysicalKeycode;
        if (codigo == Key.Escape) Interface.MostrarMenu(!Interface.MenuAberto);
        if (codigo == Key.F11) GetWindow().Mode = GetWindow().Mode == Window.ModeEnum.Fullscreen ? Window.ModeEnum.Windowed : Window.ModeEnum.Fullscreen;
        if (!OS.IsDebugBuild() || !AtalhosDebug) return;
        var id = codigo switch { Key.F1 => "terra", Key.F2 => "marte", Key.F3 => "piratas", Key.F4 => "urano", Key.F5 => "gelo", Key.F6 => "ana_branca", Key.F7 => "salto_final", _ => "" };
        if (id.Length > 0) { Interface.MostrarMenu(false); IrParaEtapa(id); }
        if (codigo == Key.F8) Exibicao.DefinirModo(!Exibicao.Fulldome);
        if (codigo == Key.F9) { Exibicao.DefinirModo(true); Exibicao.Calibracao = !Exibicao.Calibracao; }
        if (codigo == Key.F10) Interface.ExibirDebug = !Interface.ExibirDebug;
    }
    public override void _Notification(int notificacao)
    {
        if (notificacao == NotificationApplicationFocusOut && _pronto && PausarAoPerderFoco) Interface.MostrarMenu(true);
    }
}
