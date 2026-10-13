using Godot;

namespace ProjetoPlanetario.Apresentacao.Cenarios;

public partial class CenarioBase : Node3D
{
    [Export] public string Identificador { get; set; } = "";
    [Export] public string NomeExibicao { get; set; } = "";
    [Export] public int IndiceDeEtapa { get; set; }
    [Export] public string[] ProximosCenarios { get; set; } = System.Array.Empty<string>();

    public bool ValidarEstrutura(out string erro)
    {
        foreach (var caminho in new[] { "Mundo", "Mundo/CorposCelestes", "Mundo/Geometria", "Mundo/Objetos",
                     "Mundo/EfeitosVisuais", "Jogabilidade", "Jogabilidade/Eventos", "Jogabilidade/Gatilhos",
                     "Jogabilidade/PontosDeAparecimento", "Iluminacao", "Audio", "AlvosDeCamera", "Debug" })
        {
            if (GetNodeOrNull<Node3D>(caminho) is not null) continue;
            erro = $"Container ausente ou invalido: {caminho}";
            return false;
        }
        erro = "";
        return true;
    }

    // Chamado pelo fluxo persistente de viagem, sem vincular o cenario ao hardware.
    public virtual void AtualizarAmbiente(float delta) { }
}
