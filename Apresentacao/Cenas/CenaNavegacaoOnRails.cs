using System;
using Godot;
using ProjetoPlanetario.Apresentacao.Cenarios;
using ProjetoPlanetario.Apresentacao.HUD;
using ProjetoPlanetario.Dados.Nave;
using ProjetoPlanetario.Entrada.Adaptadores;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Roteamento;
using ProjetoPlanetario.Jogabilidade.Viagem;
using ProjetoPlanetario.Jogabilidade.Voo;
using ProjetoPlanetario.Narrativa.Diretor;
using ProjetoPlanetario.Nucleo.Configuracao;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;
using ProjetoPlanetario.Projecao.Adaptadores;
using ProjetoPlanetario.Publico.Estacoes;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public partial class CenaNavegacaoOnRails : Node3D
{
    private const float VelocidadeProgressoPorSegundo = 0.032f;
    private readonly ControladorDoSalto _salto = new();
    public FaseDoSalto FaseAtualDoSalto => _salto.Fase;
    public string DestinoSelecionado => _salto.DestinoSelecionado;

    private readonly BarramentoDeEventos _barramentoDeEventos = new();
    private readonly EstadoDaNave _estadoDaNave = new();
    private readonly ConfiguracaoDeExecucao _configuracaoDeExecucao = ConfiguracaoDeExecucao.CriarDesktopDev();
    private readonly ConfiguracaoDaNave _configuracaoDaNave = new()
    {
        LimiteCameraHorizontalGraus = 125f,
        LimiteCameraVerticalGraus = 82f
    };
    private readonly SegmentoDeViagem _segmentoDeViagem = SegmentoDeViagem.CriarSegmentoCometa();
    [Export] public string CenarioInicial { get; set; } = "prototipo";
    public GerenciadorCenarios GerenciadorDeCenarios => GetNode<GerenciadorCenarios>("GerenciadorCenarios");

    private ControladorDeVoo? _controladorDeVoo;
    private PathFollow3D? _seguidorDaNave;
    private CockpitDoPiloto? _cockpitDoPiloto;
    private HUDDeDebug? _hudDeDebug;
    private ApresentacaoDoSalto? _apresentacaoDoSalto;
    private AdaptadorOlharDesktop? _olharDesktop;
    private Curve3D? _curvaDaRota;
    private Node3D? _orientacaoDaNave;
    private EstacaoVirtual? _estacaoVirtual;

    public override void _Ready()
    {
        ConfiguradorInputMap.GarantirAcoesDesktopDev();

        var diretorNarrativo = new DiretorNarrativo(_barramentoDeEventos);
        new AdaptadorProjecaoDesktop(new Rect2(Vector2.Zero, GetViewport().GetVisibleRect().Size))
            .AplicarEmViewport(GetViewport());

        _barramentoDeEventos.EventoPublicado += AoEventoPublicado;

        CriarRota();
        if (!GerenciadorDeCenarios.CarregarCenario(CenarioInicial))
            GD.PushError(GerenciadorDeCenarios.UltimoErro);
        CriarCockpitVisual(diretorNarrativo);
        CriarHudDebug(diretorNarrativo);
        CriarEstacaoVirtual();
        CriarOverlayDeSalto();

        _salto.FaseAlterada += fase => _barramentoDeEventos.Publicar(
            new FaseDoSaltoAlterada(fase.ToString(), _salto.DestinoSelecionado, DateTime.UtcNow));
        ReiniciarSegmento();
    }

    public override void _Process(double delta)
    {
        if (_controladorDeVoo is null || _seguidorDaNave is null)
        {
            return;
        }

        var deltaFloat = (float)delta;

        if (_segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.EmViagem)
        {
            var resultado = _segmentoDeViagem.Atualizar(
                _controladorDeVoo.VelocidadeFrontalAtual * VelocidadeProgressoPorSegundo,
                deltaFloat);

            _seguidorDaNave.ProgressRatio = _segmentoDeViagem.ProgressoNaRota;
            if (_curvaDaRota is not null && _orientacaoDaNave is not null)
                _orientacaoDaNave.Basis = _orientacaoDaNave.Basis.Slerp(
                    TrajetoriaSuave.Orientacao(_curvaDaRota, _seguidorDaNave.Progress),
                    1f - Mathf.Exp(-4f * deltaFloat));
            _barramentoDeEventos.Publicar(new ProgressoNaRotaAlterado(
                _segmentoDeViagem.ProgressoNaRota,
                _controladorDeVoo.VelocidadeFrontalAtual,
                DateTime.UtcNow));

            foreach (var marcador in resultado.MarcadoresDisparados)
            {
                PublicarMarcador(marcador);
            }

            GerenciadorDeCenarios.CenarioAtual?.AtualizarAmbiente(deltaFloat);
        }

        if (_segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.Saltando)
        {
            AtualizarSalto(deltaFloat);
        }
    }

    public override void _UnhandledInput(InputEvent evento)
    {
        if (evento.IsActionPressed("destino_marte")) EscolherDestino("marte");
        if (evento.IsActionPressed("destino_jupiter")) EscolherDestino("jupiter");
        if (evento.IsActionPressed("alternar_debug") && _hudDeDebug is not null)
            _hudDeDebug.Visible = !_hudDeDebug.Visible;
        if (evento.IsActionPressed("alternar_estacao") && _estacaoVirtual is not null)
            _estacaoVirtual.Visible = !_estacaoVirtual.Visible;
        if (evento.IsActionPressed("piloto_confirmar") && _segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.Concluido)
            ReiniciarSegmento();
    }

    private void CriarRota()
    {
        var rota = new Path3D
        {
            Name = "TrajetoriaOnRails",
            Curve = TrajetoriaSuave.Criar()
        };
        _curvaDaRota = rota.Curve;
        AddChild(rota);

        _seguidorDaNave = new PathFollow3D
        {
            Name = "SeguidorDaNave",
            Loop = false,
            RotationMode = PathFollow3D.RotationModeEnum.None,
            CubicInterp = true
        };
        rota.AddChild(_seguidorDaNave);

        _controladorDeVoo = new ControladorDeVoo
        {
            Name = "ControladorDaNaveOnRails"
        };
        _controladorDeVoo.Configurar(new AdaptadorTecladoMouse(), _estadoDaNave, _barramentoDeEventos, _configuracaoDaNave);

        _controladorDeVoo.AddChild(new Camera3D
        {
            Name = "CameraPiloto",
            Current = true,
            Fov = 70f,
            Near = 0.03f,
            Far = 1500f
        });
        _controladorDeVoo.AddChild(new InteriorDaNave { Name = "InteriorDaNave" });
        _orientacaoDaNave = new Node3D { Name = "OrientacaoSuave" };
        _seguidorDaNave.AddChild(_orientacaoDaNave);
        _orientacaoDaNave.AddChild(_controladorDeVoo);
        var olhar = new AdaptadorOlharDesktop { Name = "OlharDesktop" };
        olhar.MovimentoDoOlhar += _controladorDeVoo.AlterarOlhar;
        olhar.RecentralizacaoSolicitada += _controladorDeVoo.RecentralizarCamera;
        _olharDesktop = olhar;
        AddChild(olhar);
    }

    private void CriarCockpitVisual(DiretorNarrativo diretorNarrativo)
    {
        if (_controladorDeVoo is null)
        {
            return;
        }

        _cockpitDoPiloto = new CockpitDoPiloto { Name = "InstrumentosDoPiloto" };
        _cockpitDoPiloto.Configurar(_controladorDeVoo, _estadoDaNave, null, diretorNarrativo, _barramentoDeEventos);
        _cockpitDoPiloto.DestinoSolicitado += destino => EscolherDestino(destino);
        AddChild(_cockpitDoPiloto);
    }

    private void CriarHudDebug(DiretorNarrativo diretorNarrativo)
    {
        _hudDeDebug = new HUDDeDebug { Name = "HUDDeDebug", Visible = false };
        _hudDeDebug.Configurar(_estadoDaNave, _configuracaoDeExecucao, diretorNarrativo, _barramentoDeEventos);
        AddChild(_hudDeDebug);
    }

    private void CriarEstacaoVirtual()
    {
        var estacao = new EstacaoVirtual { Name = "EstacaoVirtual", Visible = false };
        _estacaoVirtual = estacao;
        estacao.Configurar(new AdaptadorEntradaVirtual("Estacao1", _barramentoDeEventos));
        AddChild(estacao);
        _cockpitDoPiloto!.EstacaoSolicitada += () => estacao.Visible = !estacao.Visible;
    }

    private void CriarOverlayDeSalto()
    {
        _apresentacaoDoSalto = new ApresentacaoDoSalto { Name = "ApresentacaoDoSalto" };
        _apresentacaoDoSalto.Configurar(_salto, _controladorDeVoo!);
        AddChild(_apresentacaoDoSalto);
    }

    private void AoEventoPublicado(EventoDeDominio eventoDeDominio)
    {
        RegistrarEventoImportante(eventoDeDominio);

        if (eventoDeDominio is EstacaoAcaoExecutada evento &&
            Enum.TryParse<AcaoLogicaDaEstacao>(evento.NomeDaAcao, out var acao))
        {
            if (_salto.AceitaEscolha)
            {
                if (acao == AcaoLogicaDaEstacao.Acao1) EscolherDestino("marte");
                if (acao == AcaoLogicaDaEstacao.Acao2) EscolherDestino("jupiter");
            }
            if (_segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.Concluido && acao == AcaoLogicaDaEstacao.Confirmar)
            {
                ReiniciarSegmento();
            }
        }
    }

    private void PublicarMarcador(MarcadorDeRota marcador)
    {
        _barramentoDeEventos.Publicar(new MarcadorDeRotaAlcancado(marcador.Id, marcador.Tipo.ToString(), marcador.Progresso, DateTime.UtcNow));

        switch (marcador.Tipo)
        {
            case TipoMarcadorDeRota.InicioNarrativa:
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Avance pela rota de teste.", DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.AproximacaoCometa:
                _barramentoDeEventos.Publicar(new AproximacaoCometa(DateTime.UtcNow));
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Observe o cometa a direita da rota.", DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.PassagemCometa:
                _barramentoDeEventos.Publicar(new PassagemCometa(DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.AvisoSalto:
                _barramentoDeEventos.Publicar(new AvisoDeSalto(DateTime.UtcNow));
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Prossiga ate a anomalia gravitacional.", DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.InicioSalto:
                _controladorDeVoo?.ZerarVelocidade();
                if (_controladorDeVoo is not null) _controladorDeVoo.PropulsaoHabilitada = false;
                _salto.SolicitarDestino();
                _olharDesktop?.DefinirHabilitado(false);
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Escolha o destino do salto.", DateTime.UtcNow));
                _cockpitDoPiloto?.MostrarEscolhaDestino(true);
                break;
        }
    }

    public bool EscolherDestino(string destino)
    {
        if (!_salto.EscolherDestino(destino)) return false;
        _cockpitDoPiloto?.MostrarEscolhaDestino(false);
        _cockpitDoPiloto?.DefinirRotaAtual("DESTINO / " + destino.ToUpperInvariant());
        if (_controladorDeVoo is not null)
        {
            _controladorDeVoo.OlharHabilitado = false;
            _controladorDeVoo.SuavizacaoDoOlhar = 2.5f;
            _controladorDeVoo.RecentralizarCamera();
        }
        _barramentoDeEventos.Publicar(new SaltoGravitacionalIniciado(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new ObjetivoAlterado("Destino confirmado: " + destino.ToUpperInvariant(), DateTime.UtcNow));
        return true;
    }

    private void AtualizarSalto(float delta)
    {
        _salto.Atualizar(delta);
        if (_salto.PrecisaCarregar)
        {
            // Recursos pequenos e panorama ja em cache: troca sincrona sob o tunel opaco.
            var carregado = GerenciadorDeCenarios.CarregarCenario(_salto.DestinoSelecionado);
            _salto.ConfirmarCarregamento(carregado);
            if (!carregado)
            {
                GD.PushWarning(GerenciadorDeCenarios.UltimoErro);
                _cockpitDoPiloto?.MostrarEscolhaDestino(true, "Rota indisponivel. Escolha novamente.");
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Nao foi possivel concluir o salto.", DateTime.UtcNow));
            }
        }
        if (_salto.Fase != FaseDoSalto.Chegada) return;
        _segmentoDeViagem.ConcluirSalto();
        if (_controladorDeVoo is not null)
        {
            _controladorDeVoo.OlharHabilitado = true;
            _controladorDeVoo.SuavizacaoDoOlhar = 12f;
        }
        _olharDesktop?.DefinirHabilitado(true);
        _barramentoDeEventos.Publicar(new SaltoGravitacionalConcluido(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new SegmentoDeViagemConcluido(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new ObjetivoAlterado("Destino alcancado: " + DestinoSelecionado.ToUpperInvariant(), DateTime.UtcNow));
    }

    private void ReiniciarSegmento()
    {
        if (GerenciadorDeCenarios.ObterIdentificadorAtual() != CenarioInicial &&
            !GerenciadorDeCenarios.CarregarCenario(CenarioInicial))
        {
            GD.PushWarning(GerenciadorDeCenarios.UltimoErro);
            return;
        }
        _salto.Reiniciar();
        _apresentacaoDoSalto?.Ocultar();
        _cockpitDoPiloto?.MostrarEscolhaDestino(false);
        _cockpitDoPiloto?.DefinirRotaAtual("SETOR 01 / TRAVESSIA");
        _olharDesktop?.DefinirHabilitado(true);
        if (_controladorDeVoo is not null)
        {
            _controladorDeVoo.OlharHabilitado = true;
            _controladorDeVoo.SuavizacaoDoOlhar = 12f;
        }
        _estadoDaNave.ReiniciarParaTeste();
        _segmentoDeViagem.Iniciar();
        _controladorDeVoo?.ZerarVelocidade();
        if (_controladorDeVoo is not null) _controladorDeVoo.PropulsaoHabilitada = true;
        _controladorDeVoo?.RecentralizarCamera();

        if (_seguidorDaNave is not null)
        {
            _seguidorDaNave.ProgressRatio = 0f;
            if (_curvaDaRota is not null && _orientacaoDaNave is not null)
                _orientacaoDaNave.Basis = TrajetoriaSuave.Orientacao(_curvaDaRota, 0f);
        }

        _barramentoDeEventos.Publicar(new PerfilDeExecucaoCarregado(_configuracaoDeExecucao.Perfil.ToString(), DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new SegmentoDeViagemReiniciado(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new SegmentoDeViagemIniciado(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new ObjetivoAlterado("Avance pela rota de teste.", DateTime.UtcNow));
    }

    private static void RegistrarEventoImportante(EventoDeDominio eventoDeDominio)
    {
        switch (eventoDeDominio)
        {
            case SegmentoDeViagemIniciado:
                GD.Print("[ROTA] Segmento iniciado");
                break;
            case AproximacaoCometa:
                GD.Print("[ROTA] AproximacaoCometa");
                break;
            case PassagemCometa:
                GD.Print("[ROTA] PassagemCometa");
                break;
            case AvisoDeSalto:
                GD.Print("[ROTA] AvisoSalto");
                break;
            case SaltoGravitacionalIniciado:
                GD.Print("[SALTO] Iniciado");
                break;
            case SaltoGravitacionalConcluido:
                GD.Print("[SALTO] Concluido");
                break;
            case SegmentoDeViagemReiniciado:
                GD.Print("[ROTA] Reiniciado");
                break;
        }
    }
}
