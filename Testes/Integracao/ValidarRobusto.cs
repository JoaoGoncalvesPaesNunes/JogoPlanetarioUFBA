using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using ProjetoPlanetario.Robusto.Viagem;
using ProjetoPlanetario.Robusto.Dominio;

namespace ProjetoPlanetario.Testes.Integracao;

public partial class ValidarRobusto : Node
{
    private int _verificacoes;
    private DiretorDaViagem _jogo = null!;
    public override async void _Ready()
    {
        try
        {
            _jogo = GD.Load<PackedScene>("res://Robusto/Cenas/Experiencia.tscn").Instantiate<DiretorDaViagem>();
            _jogo.Ritmo = 4;
            _jogo.PausarAoPerderFoco = false;
            AddChild(_jogo);
            await Quadros(5);
            Exigir(_jogo.Erro == "", "Inicializacao sem erro");
            var ids = new[] { _jogo.Camera.GetInstanceId(), _jogo.Nave.GetInstanceId(), _jogo.Interface.GetInstanceId(), _jogo.GetNode("SistemasPersistentes/AudioDaViagem").GetInstanceId() };
            var visitados = new List<string> { "terra" };
            _jogo.Sequencia.Alterada += etapa => visitados.Add(etapa.Id);
            await Capturar("terra");
            var inicio = _jogo.Nave.GlobalPosition;
            await Quadros(30);
            Exigir(_jogo.Nave.GlobalPosition.DistanceTo(inicio) > 0.5f, "Nave avanca sem entrada");
            await Ate(() => _jogo.Sequencia.Atual.Id == "piratas", 1500);
            Exigir(_jogo.Cenarios.ObterIdentificadorAtual() == "marte", "Marte na sequencia fixa");
            await Capturar("piratas");
            inicio = _jogo.Nave.GlobalPosition;
            await Quadros(30);
            Exigir(_jogo.Nave.GlobalPosition.DistanceTo(inicio) > 0.1f, "Movimento continua no combate");
            _jogo.Combate.AtivarEscudo();
            Exigir(_jogo.Combate.Escudo > 0, "Extensao de escudo ativa");
            await Neutralizar();
            await Ate(() => _jogo.Sequencia.Atual.Id == "urano", 700);
            await Capturar("urano");
            await Ate(() => _jogo.Gelo.Regra?.Fase == FaseDoGelo.Alerta, 800);
            await Capturar("gelo_alerta");
            Input.ActionPress("mira_direita");
            await Ate(() => _jogo.Gelo.Regra?.Fase == FaseDoGelo.Evasao, 100);
            Input.ActionRelease("mira_direita");
            await Quadros(30);
            Exigir(Mathf.Abs(_jogo.Nave.Desvio) > 2, "Evasao desloca a nave automaticamente");
            await Capturar("gelo_evasao");
            var menorDistancia = 1000f;
            while (_jogo.Sequencia.Atual.Id == "gelo")
            {
                var objeto = _jogo.Cenarios.CenarioAtual!.GetNode<Node3D>("Mundo/Objetos/Gelo");
                menorDistancia = Mathf.Min(menorDistancia, objeto.GlobalPosition.DistanceTo(_jogo.Nave.GlobalPosition));
                await Quadros(1);
            }
            Exigir(menorDistancia > 9, "Corpo gelado passa fora da cabine");
            await Ate(() => _jogo.Sequencia.Atual.Id == "ana_branca", 700);
            Exigir(Mathf.Abs(_jogo.Nave.Desvio) < 0.01f, "Evasao retorna a rota");
            await Capturar("ana_branca");
            await Ate(() => _jogo.Sequencia.Terminou, 700);
            await Quadros(3);
            await Capturar("chegada");
            Exigir(visitados.SequenceEqual(_jogo.Sequencia.Etapas.Select(e => e.Id)), "Todas as etapas na ordem, sem escolhas");
            Exigir(ids.SequenceEqual(new[] { _jogo.Camera.GetInstanceId(), _jogo.Nave.GetInstanceId(), _jogo.Interface.GetInstanceId(), _jogo.GetNode("SistemasPersistentes/AudioDaViagem").GetInstanceId() }), "Sistemas persistentes preservados");
            Exigir(_jogo.Cenarios.Recipiente.GetChildCount() == 1, "Um cenario ativo");
            var reiniciar = (Button)_jogo.Interface.FindChild("ReiniciarViagem", true, false);
            Exigir(reiniciar.IsVisibleInTree(), "Reinicio disponivel na chegada");
            reiniciar.EmitSignal(Button.SignalName.Pressed);
            Exigir(_jogo.Sequencia.Atual.Id == "terra", "Botao reinicia a viagem");
        }
        catch (InvalidCastException erro) { GD.PushError(erro.ToString()); GetTree().Quit(1); return; }
        catch (Exception erro) { GD.PushError(erro.ToString()); GetTree().Quit(1); return; }
        try
        {
            _jogo.Reiniciar();
            Exigir(_jogo.Sequencia.Atual.Id == "terra" && _jogo.Nave.Distancia == 0, "Reinicio limpo");
            _jogo.IrParaEtapa("piratas");
            for (var i = 0; i < 600; i++) { _jogo.Nave.Avancar(1, 0.35f); _jogo.Combate.Atualizar(1, _jogo.Nave.GlobalPosition); }
            Exigir(_jogo.Combate.Alvos.All(alvo => !_jogo.Camera.IsPositionBehind(alvo.GlobalPosition)), "Alvos permanecem acessiveis em combate longo");
            await Neutralizar();
            await TestarDomo();
            GD.Print($"[ROBUSTO] {_verificacoes} verificacoes aprovadas. Zero falhas.");
            _jogo.QueueFree();
            await Quadros(3);
            GetTree().Quit();
        }
        catch (Exception erro) { GD.PushError(erro.ToString()); GetTree().Quit(1); }
    }
    private async Task TestarDomo()
    {
        _jogo.Exibicao.DefinirModo(true);
        Exigir(_jogo.Exibicao.SaidaDomo.Size == new Vector2I(1024, 1024), "Saida quadrada 1024");
        foreach (var id in new[] { "terra", "piratas", "urano", "gelo", "ana_branca", "salto_final" })
        {
            _jogo.IrParaEtapa(id);
            await Quadros(id == "salto_final" ? 40 : 5);
            await Capturar("domo_" + id);
            var ponto = _jogo.Camera.GlobalPosition + _jogo.Camera.GlobalBasis * new Vector3(12, 8, -70);
            var pixel = _jogo.Exibicao.ProjetarPonto(ponto);
            var raio = _jogo.Exibicao.DirecaoNaTela(pixel);
            Exigir(raio.Dot((ponto - _jogo.Camera.GlobalPosition).Normalized()) > 0.9999f, "Mira corresponde a projecao " + id);
            if (id == "piratas") await Neutralizar();
        }
        _jogo.IrParaEtapa("terra");
        _jogo.Exibicao.Configuracao.EspelharHorizontal = true;
        _jogo.Exibicao.Configuracao.OrientacaoDomo = 30;
        _jogo.Exibicao.Configuracao.RotacaoCameraGraus = new Vector3(25, 10, 0);
        _jogo.Exibicao.AplicarConfiguracao();
        var pontoOrientado = _jogo.Camera.GlobalPosition + _jogo.Camera.GlobalBasis * new Vector3(10, 25, -90);
        var raioOrientado = _jogo.Exibicao.DirecaoNaTela(_jogo.Exibicao.ProjetarPonto(pontoOrientado));
        Exigir(raioOrientado.Dot((pontoOrientado - _jogo.Camera.GlobalPosition).Normalized()) > 0.9999f, "Espelho e rotacao preservam mira");
        _jogo.Exibicao.Calibracao = true;
        await Quadros(5);
        await Capturar("domo_calibracao");
        _jogo.Exibicao.Configuracao.ResolucaoSaida = 2048;
        _jogo.Exibicao.AplicarConfiguracao();
        await Quadros(5);
        Exigir(_jogo.Exibicao.SaidaDomo.Size == new Vector2I(2048, 2048), "Resolucao configuravel");
        await Capturar("domo_2048");
        _jogo.Exibicao.DefinirModo(false);
        Exigir(!GetViewport().Disable3D, "Retorno a camera normal");
        foreach (var tamanho in new[] { new Vector2I(960, 540), new Vector2I(1600, 900), new Vector2I(1024, 1024) })
        {
            GetWindow().Mode = Window.ModeEnum.Windowed;
            GetWindow().Size = tamanho;
            await Quadros(5);
            Exigir(GetViewport().GetVisibleRect().Encloses(_jogo.Exibicao.ObterAreaLogica()), "UI dentro da saida");
            await Capturar($"janela_{tamanho.X}_{tamanho.Y}");
        }
        if (DisplayServer.GetName() != "headless")
        {
            GetWindow().Mode = Window.ModeEnum.Fullscreen;
            await Quadros(10);
            Exigir(GetWindow().Mode == Window.ModeEnum.Fullscreen, "Tela cheia ativa");
            await Capturar("fullscreen");
            GetWindow().Mode = Window.ModeEnum.Windowed;
        }
        var cenario = _jogo.Cenarios.CenarioAtual;
        Exigir(!_jogo.Cenarios.CarregarCenario("inexistente") && _jogo.Cenarios.CenarioAtual == cenario, "Falha preserva ambiente");
    }
    private async Task Neutralizar()
    {
        var tentativas = 0;
        while (_jogo.Combate.Restantes > 0 && tentativas++ < 20)
        {
            var alvo = _jogo.Combate.Alvos[0];
            var malha = (MeshInstance3D)alvo.FindChildren("*", "MeshInstance3D", true, false)[0];
            var centroVisual = malha.ToGlobal(malha.GetAabb().GetCenter());
            Exigir(centroVisual.DistanceTo(alvo.GlobalPosition) < _jogo.Combate.RaioAcerto, "Volume de acerto corresponde ao modelo visivel");
            GetViewport().PushInput(new InputEventMouseMotion { Position = _jogo.Exibicao.ProjetarPonto(centroVisual) }, true);
            Input.ActionPress("disparar");
            await Quadros(5);
            Input.ActionRelease("disparar");
        }
        Exigir(_jogo.Combate.Restantes == 0, "Mira e disparo neutralizam piratas");
    }
    private async Task Capturar(string nome)
    {
        if (DisplayServer.GetName() == "headless") return;
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        using var imagem = GetViewport().GetTexture().GetImage();
        var diferentes = 0;
        for (var y = imagem.GetHeight() / 4; y < imagem.GetHeight() * 3 / 4; y += 12)
            for (var x = imagem.GetWidth() / 4; x < imagem.GetWidth() * 3 / 4; x += 12)
                if (imagem.GetPixel(x, y).R + imagem.GetPixel(x, y).G + imagem.GetPixel(x, y).B > 0.15f) diferentes++;
        Exigir(diferentes > 10, "Renderizacao nao vazia: " + nome);
        DirAccess.MakeDirRecursiveAbsolute("res://.godot/validacao_robusta");
        imagem.SavePng("res://.godot/validacao_robusta/" + nome + ".png");
    }
    private async Task Quadros(int quantidade) { for (var i = 0; i < quantidade; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
    private async Task Ate(Func<bool> condicao, int limite) { for (var i = 0; i < limite && !condicao(); i++) await Quadros(1); Exigir(condicao(), "Tempo de transicao excedido: " + _jogo.Sequencia.Atual.Id); }
    private void Exigir(bool condicao, string mensagem) { _verificacoes++; if (!condicao) throw new InvalidOperationException(mensagem); }
}
