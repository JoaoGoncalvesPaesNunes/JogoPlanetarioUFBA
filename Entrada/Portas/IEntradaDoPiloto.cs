using ProjetoPlanetario.Entrada.Modelos;

namespace ProjetoPlanetario.Entrada.Portas;

/// <summary>
/// Porta logica consumida pela jogabilidade do piloto.
/// </summary>
public interface IEntradaDoPiloto
{
    EstadoEntradaPiloto LerEstado();
}
