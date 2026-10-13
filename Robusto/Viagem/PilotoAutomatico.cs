using Godot;
using ProjetoPlanetario.Apresentacao.Cenas;

namespace ProjetoPlanetario.Robusto.Viagem;

public partial class PilotoAutomatico : Node3D
{
    [Export(PropertyHint.Range, "0.1,10")] public float Suavizacao { get; set; } = 2;
    public Path3D? Rota { get; private set; }
    public float Distancia { get; private set; }
    public float Desvio { get; set; }
    public void DefinirRota(Path3D rota)
    {
        Rota = rota;
        Distancia = 0;
        Desvio = 0;
        GlobalTransform = rota.GlobalTransform * new Transform3D(
            TrajetoriaSuave.Orientacao(rota.Curve, 0), rota.Curve.SampleBaked(0));
    }
    public void Avancar(float delta, float velocidade)
    {
        if (Rota?.Curve is not { } curva) return;
        // Aproximacao suave ao fim evita um salto ou uma volta inesperada durante eventos longos.
        var restante = curva.GetBakedLength() - Distancia;
        Distancia += Mathf.Min(delta * velocidade, restante * delta * 0.2f);
        var alvo = Rota.GlobalTransform * new Transform3D(TrajetoriaSuave.Orientacao(curva, Distancia), curva.SampleBaked(Distancia, true));
        GlobalPosition = alvo.Origin + alvo.Basis.X * Desvio;
        GlobalBasis = GlobalBasis.Slerp(alvo.Basis, 1 - Mathf.Exp(-delta * Suavizacao)).Orthonormalized();
    }
}
