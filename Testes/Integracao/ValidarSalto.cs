using System;
using System.Threading.Tasks;
using Godot;
using ProjetoPlanetario.Apresentacao.Cenas;
using ProjetoPlanetario.Jogabilidade.Viagem;

namespace ProjetoPlanetario.Testes.Integracao;

public partial class ValidarSalto : Node
{
    private int _verificacoes;

    public override async void _Ready()
    {
        try
        {
            foreach (var destino in new[] { "marte", "jupiter" }) await TestarDestino(destino);
            GD.Print($"[SALTO TESTE] {_verificacoes} verificacoes aprovadas. Zero falhas.");
            GetTree().Quit();
        }
        catch (Exception erro)
        {
            Input.ActionRelease("piloto_avancar");
            GD.PushError(erro.ToString());
            GetTree().Quit(1);
        }
    }

    private async Task TestarDestino(string destino)
    {
        var jogo = GD.Load<PackedScene>("res://Apresentacao/Cenas/CenaNavegacaoOnRails.tscn").Instantiate<CenaNavegacaoOnRails>();
        AddChild(jogo);
        await Quadros(5);
        var gerenciador = jogo.GerenciadorDeCenarios;
        var nave = jogo.GetNode<Node3D>("TrajetoriaOnRails/SeguidorDaNave/OrientacaoSuave/ControladorDaNaveOnRails");
        var camera = nave.GetNode<Camera3D>("CameraPiloto");
        var cabine = nave.GetNode<Node3D>("InteriorDaNave");
        var hud = jogo.GetNode<Control>("InstrumentosDoPiloto");
        var voltar = hud.GetNode<Button>("VoltarRotaInicial");
        Garantir(!voltar.Visible, "Retorno oculto durante a rota inicial.");
        var idCamera = camera.GetInstanceId();
        var idCabine = cabine.GetInstanceId();
        var idHud = hud.GetInstanceId();
        var contagemCargas = 0;
        gerenciador.CenarioCarregado += _ => contagemCargas++;
        Garantir(!jogo.EscolherDestino(destino), "Escolha antecipada rejeitada.");
        Input.ActionPress("piloto_avancar");
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.AguardandoDestino, 2400);
        Input.ActionRelease("piloto_avancar");
        Garantir(hud.GetNode<Control>("EscolhaDestino").Visible, "Escolha visivel antes do salto.");
        await Quadros(90);
        Garantir(jogo.DestinoSelecionado == "" && contagemCargas == 0, "Nenhum destino automatico.");
        await Capturar(destino + "_escolha");
        if (destino == "marte")
        {
            var botao = (Button)hud.FindChild("marte", true, false);
            botao.EmitSignal(Button.SignalName.Pressed);
            ((Button)hud.FindChild("jupiter", true, false)).EmitSignal(Button.SignalName.Pressed);
        }
        else
        {
            foreach (var filho in jogo.GetNode("EstacaoVirtual").FindChildren("*", "Button", true, false))
                if (filho is Button { Text: "ACAO 2" } botao) botao.EmitSignal(Button.SignalName.Pressed);
        }
        Garantir(jogo.DestinoSelecionado == destino, "Destino unico confirmado pelo adaptador existente.");
        Garantir(!hud.GetNode<Control>("EscolhaDestino").Visible, "Escolha fecha apos confirmar.");
        Garantir(!jogo.EscolherDestino(destino), "Reativacao bloqueada.");
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Acelerando, 100);
        await Quadros(40);
        await Capturar(destino + "_aceleracao");
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Hiperespaco, 100);
        await Ate(() => gerenciador.ObterIdentificadorAtual() == destino, 50);
        Garantir(jogo.FaseAtualDoSalto == FaseDoSalto.Hiperespaco, "Troca acontece no hiperespaço.");
        Garantir(!jogo.EscolherDestino(destino == "marte" ? "jupiter" : "marte"), "Troca de destino no tunel rejeitada.");
        await Capturar(destino + "_hiperespaco");
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Desacelerando, 150);
        await Quadros(35);
        await Capturar(destino + "_desaceleracao");
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Chegada, 120);
        await Quadros(5);
        Garantir(gerenciador.ObterIdentificadorAtual() == destino && contagemCargas == 1, "Destino carregado uma unica vez.");
        Garantir(gerenciador.Recipiente.GetChildCount() == 1, "Apenas um cenario ativo.");
        Garantir(camera.GetInstanceId() == idCamera && cabine.GetInstanceId() == idCabine && hud.GetInstanceId() == idHud, "Sistemas persistentes preservados.");
        Garantir(Mathf.Abs(camera.Fov - 70) < 0.01f, "FOV preservado.");
        Garantir(!nave.GetNode<Node3D>("TunelDoSalto").Visible, "Efeito removido na chegada.");
        var planeta = (MeshInstance3D)gerenciador.CenarioAtual!.FindChildren("*Placeholder", "MeshInstance3D", true, false)[0];
        var tela = camera.UnprojectPosition(planeta.GlobalPosition);
        Garantir(!camera.IsPositionBehind(planeta.GlobalPosition) && GetViewport().GetVisibleRect().HasPoint(tela), "Planeta esta no campo de visao.");
        Garantir(camera.GlobalPosition.DistanceTo(planeta.GlobalPosition) > 44f, "Chegada fora do planeta.");
        await Capturar(destino + "_chegada");
        Garantir(voltar.IsVisibleInTree(), "Botao de retorno visivel na chegada.");
        voltar.EmitSignal(Button.SignalName.Pressed);
        await Quadros(3);
        Garantir(gerenciador.ObterIdentificadorAtual() == "prototipo" && jogo.DestinoSelecionado == "", "Botao restaura o ambiente inicial e limpa o destino.");
        Garantir(!voltar.Visible && jogo.FaseAtualDoSalto == FaseDoSalto.Normal, "Retorno encerra o salto e oculta o botao.");
        Garantir(jogo.GetNode<PathFollow3D>("TrajetoriaOnRails/SeguidorDaNave").ProgressRatio < 0.001f, "Retorno reposiciona a nave no inicio.");
        Input.ActionPress("piloto_avancar");
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.AguardandoDestino, 2400);
        Input.ActionRelease("piloto_avancar");
        var outroDestino = destino == "marte" ? "jupiter" : "marte";
        Garantir(hud.GetNode<Control>("EscolhaDestino").Visible, "Segunda viagem reabre a escolha.");
        ((Button)hud.FindChild(outroDestino, true, false)).EmitSignal(Button.SignalName.Pressed);
        await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Chegada, 400);
        await Quadros(5);
        Garantir(gerenciador.ObterIdentificadorAtual() == outroDestino && contagemCargas == 3, "Segunda viagem chega ao outro planeta sem cargas duplicadas.");
        Garantir(voltar.IsVisibleInTree(), "Retorno disponivel novamente na segunda chegada.");
        var tecla = new InputEventKey { PhysicalKeycode = Key.Enter, Pressed = true };
        Input.ParseInputEvent(tecla);
        await Quadros(3);
        tecla.Pressed = false;
        Input.ParseInputEvent(tecla);
        Garantir(gerenciador.ObterIdentificadorAtual() == "prototipo" && jogo.DestinoSelecionado == "", "Reinicio restaura o ambiente inicial.");
        if (destino == "jupiter")
        {
            Input.ActionPress("piloto_avancar");
            await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.AguardandoDestino, 2400);
            Input.ActionRelease("piloto_avancar");
            var caminhoOriginal = gerenciador.Catalogo.Caminhos[destino];
            try
            {
                gerenciador.Catalogo.Caminhos[destino] = "res://.godot/destino_ausente_teste.tscn";
                Garantir(jogo.EscolherDestino(destino), "Tentativa para testar erro de recurso.");
                await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Falha, 250);
                Garantir(gerenciador.ObterIdentificadorAtual() == "prototipo", "Falha mantem o ambiente anterior.");
                Garantir(hud.GetNode<Control>("EscolhaDestino").Visible, "Falha reabre a escolha.");
            }
            finally { gerenciador.Catalogo.Caminhos[destino] = caminhoOriginal; }
            Garantir(jogo.EscolherDestino(destino), "Nova tentativa aceita apos falha.");
            await Ate(() => jogo.FaseAtualDoSalto == FaseDoSalto.Chegada, 400);
            Garantir(gerenciador.ObterIdentificadorAtual() == destino, "Recuperacao termina no destino correto.");
        }
        jogo.QueueFree();
        await Quadros(3);
        GD.Print("[SALTO TESTE] Fluxo completo: " + destino);
    }

    private async Task Capturar(string nome)
    {
        if (DisplayServer.GetName() == "headless") return;
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        using var imagem = GetViewport().GetTexture().GetImage();
        Garantir(!imagem.IsEmpty(), "Renderizacao nao vazia.");
        DirAccess.MakeDirRecursiveAbsolute("res://.godot/validacao_salto");
        imagem.SavePng("res://.godot/validacao_salto/" + nome + ".png");
    }

    private async Task Quadros(int quantidade)
    {
        for (var i = 0; i < quantidade; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    private async Task Ate(Func<bool> condicao, int limite)
    {
        for (var i = 0; i < limite && !condicao(); i++) await Quadros(1);
        Garantir(condicao(), "Tempo limite de transicao excedido.");
    }

    private void Garantir(bool condicao, string mensagem)
    {
        _verificacoes++;
        if (!condicao) throw new InvalidOperationException(mensagem);
    }
}
