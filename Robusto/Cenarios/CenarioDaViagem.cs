using Godot;
using ProjetoPlanetario.Apresentacao.Cenarios;

namespace ProjetoPlanetario.Robusto.Cenarios;

public partial class CenarioDaViagem : CenarioBase
{
    [Export] public float RotacaoDoPlaneta { get; set; } = 0.008f;
    [Export(PropertyHint.Range, "0.1,20")] public float TempoAproximacaoGelo { get; set; } = 6;
    [Export(PropertyHint.Range, "0.1,10")] public float TempoCorrecaoGelo { get; set; } = 2;
    [Export(PropertyHint.Range, "1,10")] public float DuracaoEvasao { get; set; } = 5;
    [Export(PropertyHint.Range, "5,30")] public float DistanciaEvasao { get; set; } = 16;
    [Export] public Material? MaterialGelo { get; set; }
    public Path3D Rota => GetNode<Path3D>("Rota");
    public override void _Ready()
    {
        if (MaterialGelo is null) return;
        foreach (var no in GetNode("Mundo/Objetos/Gelo").FindChildren("*", "MeshInstance3D", true, false))
            ((MeshInstance3D)no).MaterialOverride = MaterialGelo;
    }
    public override void AtualizarAmbiente(float delta)
    {
        var superficie = GetNodeOrNull<Node3D>("Mundo/CorposCelestes/Planeta/Superficie") ??
                         GetNodeOrNull<Node3D>("Mundo/CorposCelestes/Planeta");
        superficie?.RotateY(delta * RotacaoDoPlaneta);
    }
}
