using Godot;

namespace ProjetoPlanetario.Entrada.Modelos;

public readonly record struct EstadoEntradaPiloto(
    Vector2 Movimento,
    float ImpulsoFrontal,
    bool AcaoPrimariaPressionada,
    bool AcaoPrimariaIniciada,
    bool ConfirmarIniciado,
    bool CancelarIniciado);
