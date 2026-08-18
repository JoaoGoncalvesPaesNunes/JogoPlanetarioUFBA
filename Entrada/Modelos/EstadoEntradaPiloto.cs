using Godot;

namespace ProjetoPlanetario.Entrada.Modelos;

public readonly record struct EstadoEntradaPiloto(
    Vector2 Movimento,
    bool AcaoPrimariaPressionada,
    bool AcaoPrimariaIniciada,
    bool ConfirmarIniciado,
    bool CancelarIniciado);
