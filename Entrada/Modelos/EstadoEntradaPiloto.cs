using Godot;

namespace ProjetoPlanetario.Entrada.Modelos;

public readonly record struct EstadoEntradaPiloto(
    Vector2 MovimentoDaCamera,
    float MovimentoNaRota,
    bool AcaoPrimariaPressionada,
    bool AcaoPrimariaIniciada,
    bool ConfirmarIniciado,
    bool CancelarIniciado)
{
    public Vector2 Movimento => MovimentoDaCamera;
    public float ImpulsoFrontal => MovimentoNaRota;
}
