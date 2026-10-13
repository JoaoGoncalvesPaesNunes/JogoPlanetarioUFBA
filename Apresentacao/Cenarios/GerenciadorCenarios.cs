using Godot;
using ProjetoPlanetario.Dados.Cenarios;

namespace ProjetoPlanetario.Apresentacao.Cenarios;

/// <summary>Gerencia somente o ciclo de vida do ambiente; nao altera nave, missao ou camera.</summary>
public partial class GerenciadorCenarios : Node
{
    [Export] public Node3D Recipiente { get; set; } = null!;
    [Export] public CatalogoCenarios Catalogo { get; set; } = null!;
    [Signal] public delegate void CenarioCarregadoEventHandler(string identificador);
    [Signal] public delegate void CenarioDescarregadoEventHandler(string identificador);

    public CenarioBase? CenarioAtual { get; private set; }
    public string UltimoErro { get; private set; } = "";
    private bool _emTransicao;

    public string ObterIdentificadorAtual() => CenarioAtual?.Identificador ?? "";

    public bool CarregarCenario(string identificador)
    {
        if (_emTransicao) return Falhar("Ja existe uma transicao em andamento.");
        if (!IsInsideTree() || !IsInstanceValid(Recipiente) || !Recipiente.IsInsideTree())
            return Falhar("O recipiente deve estar na arvore antes do carregamento.");
        if (Catalogo is null || string.IsNullOrWhiteSpace(identificador) ||
            !Catalogo.Caminhos.TryGetValue(identificador, out var caminho))
            return Falhar($"Cenario nao registrado: {identificador}");
        if (!ResourceLoader.Exists(caminho, "PackedScene")) return Falhar($"Cena inexistente: {caminho}");
        var cena = GD.Load<PackedScene>(caminho);
        if (cena is null || !cena.CanInstantiate()) return Falhar($"Cena invalida: {caminho}");

        // Valida antes de retirar o ambiente atual. Falhas de cadastro preservam a experiencia.
        var instancia = cena.Instantiate();
        if (instancia is not CenarioBase candidato)
        {
            instancia.Free();
            return Falhar("A raiz da cena deve utilizar CenarioBase.");
        }
        if (candidato.Identificador != identificador || !candidato.ValidarEstrutura(out _))
        {
            candidato.Free();
            return Falhar("Identificador ou estrutura do cenario nao corresponde ao cadastro.");
        }

        _emTransicao = true;
        try
        {
            RetirarAtual();
            CenarioAtual = candidato;
            Recipiente.AddChild(candidato);
            UltimoErro = "";
            EmitSignal(SignalName.CenarioCarregado, identificador);
            return true;
        }
        finally { _emTransicao = false; }
    }

    public bool DescarregarCenarioAtual()
    {
        if (_emTransicao) return Falhar("Ja existe uma transicao em andamento.");
        _emTransicao = true;
        try
        {
            RetirarAtual();
            UltimoErro = "";
            return true;
        }
        finally { _emTransicao = false; }
    }

    private void RetirarAtual()
    {
        if (CenarioAtual is null) return;
        var anterior = CenarioAtual;
        var identificador = anterior.Identificador;
        CenarioAtual = null;
        // Remove imediatamente luzes, audio e processamento; a liberacao ocorre ao fim do quadro.
        Recipiente.RemoveChild(anterior);
        anterior.QueueFree();
        EmitSignal(SignalName.CenarioDescarregado, identificador);
    }

    private bool Falhar(string erro)
    {
        UltimoErro = erro;
        return false;
    }
}
