using ProjetoPlanetario.Entrada.Modelos;

namespace ProjetoPlanetario.Entrada.Portas;

/// <summary>
/// Porta logica para estacoes de passageiros, sem referencia a hardware fisico.
/// </summary>
public interface IEntradaDeEstacao
{
    string Identificador { get; }
    void EnviarAcao(AcaoLogicaDaEstacao acao);
}
