using System.Collections.Generic;

namespace ProjetoPlanetario.Jogabilidade.Viagem;

public sealed class ResultadoAtualizacaoSegmento
{
    private readonly List<MarcadorDeRota> _marcadoresDisparados = new();

    public IReadOnlyList<MarcadorDeRota> MarcadoresDisparados => _marcadoresDisparados;

    public void AdicionarMarcador(MarcadorDeRota marcadorDeRota)
    {
        _marcadoresDisparados.Add(marcadorDeRota);
    }
}
