using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using ProjetoPlanetario.Apresentacao.Cenas;
using ProjetoPlanetario.Apresentacao.Cenarios;

namespace ProjetoPlanetario.Testes.Integracao;

public partial class ValidarCenarios : Node
{
    private int _verificacoes;

    public override async void _Ready()
    {
        try
        {
            await Executar();
            GD.Print($"[CENARIOS] {_verificacoes} verificacoes aprovadas. Zero falhas.");
            GetTree().Quit();
        }
        catch (Exception erro)
        {
            GD.PushError(erro.ToString());
            GetTree().Quit(1);
        }
    }

    private async Task Executar()
    {
        var jogo = GD.Load<PackedScene>("res://Apresentacao/Cenas/CenaNavegacaoOnRails.tscn").Instantiate<CenaNavegacaoOnRails>();
        AddChild(jogo);
        await Quadro();
        jogo.SetProcess(false);
        var gerenciador = jogo.GerenciadorDeCenarios;
        Garantir(gerenciador.ObterIdentificadorAtual() == "prototipo", "Inicializacao preserva o prototipo.");
        Garantir(gerenciador.Catalogo.Caminhos.Count == 8, "Sete regioes e um prototipo tecnico, sem Chegada.");
        var persistentes = new Dictionary<string, ulong>();
        foreach (var caminho in new[] { "TrajetoriaOnRails", "TrajetoriaOnRails/SeguidorDaNave/OrientacaoSuave/ControladorDaNaveOnRails",
                     "TrajetoriaOnRails/SeguidorDaNave/OrientacaoSuave/ControladorDaNaveOnRails/CameraPiloto",
                     "TrajetoriaOnRails/SeguidorDaNave/OrientacaoSuave/ControladorDaNaveOnRails/InteriorDaNave",
                     "InstrumentosDoPiloto", "HUDDeDebug", "EstacaoVirtual", "OlharDesktop", "ApresentacaoDoSalto" })
            persistentes.Add(caminho, jogo.GetNode(caminho).GetInstanceId());
        var seguidor = jogo.GetNode<PathFollow3D>("TrajetoriaOnRails/SeguidorDaNave");
        seguidor.ProgressRatio = 0.27f;

        var esperados = new Dictionary<string, string[]>
        {
            ["terra_saida"] = new[] { "marte", "jupiter" },
            ["marte"] = new[] { "saturno", "urano" }, ["jupiter"] = new[] { "saturno", "urano" },
            ["saturno"] = new[] { "ana_branca", "buraco_negro" }, ["urano"] = new[] { "ana_branca", "buraco_negro" },
            ["ana_branca"] = Array.Empty<string>(), ["buraco_negro"] = Array.Empty<string>()
        };
        foreach (var (id, proximos) in esperados)
        {
            var independente = GD.Load<PackedScene>(gerenciador.Catalogo.Caminhos[id]).Instantiate<CenarioBase>();
            var vistaIndependente = new SubViewport { OwnWorld3D = true };
            AddChild(vistaIndependente);
            vistaIndependente.AddChild(independente);
            Garantir(independente.Identificador == id && independente.ValidarEstrutura(out _), $"Cena independente valida: {id}");
            Garantir(string.Join(',', independente.ProximosCenarios) == string.Join(',', proximos), $"Ramificacoes corretas: {id}");
            Garantir(independente.FindChildren("*", "Camera3D", true, false).Count == 0, "Sem camera duplicada.");
            var quantidadeEsperada = id is "marte" or "jupiter" ? 1 : 0;
            Garantir(independente.FindChildren("*", "MeshInstance3D", true, false).Count == quantidadeEsperada, "Somente os destinos autorizados possuem planeta provisório.");
            vistaIndependente.QueueFree();
            await Quadro();
        }

        var sinais = new List<string>();
        gerenciador.CenarioCarregado += id => sinais.Add("+" + id);
        gerenciador.CenarioDescarregado += id => sinais.Add("-" + id);
        foreach (var id in new[] { "terra_saida", "marte", "saturno", "ana_branca", "terra_saida", "jupiter", "urano", "buraco_negro" })
        {
            var anterior = gerenciador.CenarioAtual!;
            var idAnterior = gerenciador.ObterIdentificadorAtual();
            sinais.Clear();
            Garantir(gerenciador.CarregarCenario(id), gerenciador.UltimoErro);
            Garantir(gerenciador.ObterIdentificadorAtual() == id, "Identificador atualizado.");
            Garantir(gerenciador.Recipiente.GetChildCount() == 1, "Somente um ambiente no recipiente.");
            Garantir(!anterior.IsInsideTree(), "Anterior sai da arvore imediatamente.");
            Garantir(string.Join(',', sinais) == $"-{idAnterior},+{id}", "Sinais em ordem.");
            foreach (var (caminho, instancia) in persistentes)
                Garantir(jogo.GetNode(caminho).GetInstanceId() == instancia, $"Sistema persistente preservado: {caminho}");
            Garantir(Mathf.Abs(seguidor.ProgressRatio - 0.27f) < 0.001f, "Troca nao reinicia a rota.");
            await Quadro();
            Garantir(!IsInstanceValid(anterior), "Instancia antiga liberada.");
            GD.Print($"[CENARIOS] Carregado: {id}");
        }

        var atual = gerenciador.CenarioAtual;
        sinais.Clear();
        Garantir(!gerenciador.CarregarCenario("desconhecido"), "ID invalido rejeitado.");
        gerenciador.Catalogo.Caminhos["ausente"] = "res://.godot/nao_existe.tscn";
        Garantir(!gerenciador.CarregarCenario("ausente"), "Recurso ausente rejeitado.");
        gerenciador.Catalogo.Caminhos.Remove("ausente");
        gerenciador.Catalogo.Caminhos["id_divergente"] = gerenciador.Catalogo.Caminhos["marte"];
        Garantir(!gerenciador.CarregarCenario("id_divergente"), "Metadados divergentes rejeitados.");
        gerenciador.Catalogo.Caminhos.Remove("id_divergente");
        Garantir(gerenciador.CenarioAtual == atual && sinais.Count == 0, "Falha preserva ambiente e nao publica troca.");

        var bloqueouReentrada = false;
        void TentarReentrada(string _) => bloqueouReentrada = !gerenciador.CarregarCenario("marte");
        gerenciador.CenarioDescarregado += TentarReentrada;
        Garantir(gerenciador.CarregarCenario("prototipo"), "Retorno ao prototipo.");
        gerenciador.CenarioDescarregado -= TentarReentrada;
        Garantir(bloqueouReentrada, "Callback nao pode iniciar uma troca dentro de outra.");
        Garantir(gerenciador.CenarioAtual!.HasNode("Mundo/CorposCelestes/SistemaPlanetario/PlanetaOceanico"), "Planetas preservados.");
        Garantir(gerenciador.CenarioAtual.HasNode("Mundo/EfeitosVisuais/CometaMarco04"), "Cometa preservado.");
        Garantir(gerenciador.CenarioAtual.HasNode("Iluminacao/AmbienteOnRails"), "Panorama e iluminacao pertencem ao ambiente.");
        Garantir(gerenciador.CarregarCenario("prototipo"), "Recarregar mesmo ID funciona.");
        Garantir(gerenciador.Recipiente.GetChildCount() == 1, "Recarregar nao duplica ambiente.");
        Garantir(gerenciador.DescarregarCenarioAtual(), "Descarregamento explicito.");
        Garantir(gerenciador.ObterIdentificadorAtual() == "" && gerenciador.Recipiente.GetChildCount() == 0, "Recipiente vazio.");
        Garantir(gerenciador.DescarregarCenarioAtual(), "Descarregamento repetido e seguro.");
        Garantir(gerenciador.CarregarCenario("prototipo"), "Inicializacao normal restaurada.");
        await Quadro();
        jogo.QueueFree();
        await Quadro();
    }

    private async Task Quadro() => await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

    private void Garantir(bool condicao, string mensagem)
    {
        _verificacoes++;
        if (!condicao) throw new InvalidOperationException(mensagem);
    }
}
