using System;

namespace ProjetoPlanetario.Nucleo.Eventos;

/// <summary>
/// Barramento pequeno e tipado para eventos de dominio usados no microprototipo.
/// </summary>
public sealed class BarramentoDeEventos
{
    public event Action<EventoDeDominio>? EventoPublicado;

    public void Publicar(EventoDeDominio eventoDeDominio)
    {
        EventoPublicado?.Invoke(eventoDeDominio);
    }
}
