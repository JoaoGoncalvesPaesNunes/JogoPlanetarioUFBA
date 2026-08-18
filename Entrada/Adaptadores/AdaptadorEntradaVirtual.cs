using System;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Portas;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Entrada.Adaptadores;

/// <summary>
/// Adaptador de desenvolvimento para simular uma estacao fisica por UI.
/// </summary>
public sealed class AdaptadorEntradaVirtual : IEntradaDeEstacao
{
    private readonly BarramentoDeEventos _barramentoDeEventos;

    public string Identificador { get; }

    public AdaptadorEntradaVirtual(string identificador, BarramentoDeEventos barramentoDeEventos)
    {
        Identificador = identificador;
        _barramentoDeEventos = barramentoDeEventos;
    }

    public void EnviarAcao(AcaoLogicaDaEstacao acao)
    {
        _barramentoDeEventos.Publicar(new EstacaoAcaoExecutada(Identificador, acao.ToString(), DateTime.UtcNow));
    }
}
