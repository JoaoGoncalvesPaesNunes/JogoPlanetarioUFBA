using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoPlanetario.Jogabilidade.Viagem;

/// <summary>
/// Modelo puro de um trecho on-rails: progresso fisico pode voltar, narrativa nao.
/// </summary>
public sealed class SegmentoDeViagem
{
    private readonly List<MarcadorDeRota> _marcadores;

    public EstadoDoSegmentoDeViagem Estado { get; private set; } = EstadoDoSegmentoDeViagem.NaoIniciado;
    public float ProgressoNaRota { get; private set; }
    public IReadOnlyList<MarcadorDeRota> Marcadores => _marcadores;

    public SegmentoDeViagem(IEnumerable<MarcadorDeRota> marcadores)
    {
        _marcadores = marcadores.OrderBy(marcador => marcador.Progresso).ToList();
    }

    public static SegmentoDeViagem CriarSegmentoCometa()
    {
        return new SegmentoDeViagem(new[]
        {
            new MarcadorDeRota("inicio_narrativa", 0.03f, TipoMarcadorDeRota.InicioNarrativa),
            new MarcadorDeRota("aproximacao_cometa", 0.32f, TipoMarcadorDeRota.AproximacaoCometa),
            new MarcadorDeRota("passagem_cometa", 0.52f, TipoMarcadorDeRota.PassagemCometa),
            new MarcadorDeRota("aviso_salto", 0.84f, TipoMarcadorDeRota.AvisoSalto),
            new MarcadorDeRota("inicio_salto", 0.98f, TipoMarcadorDeRota.InicioSalto)
        });
    }

    public void Iniciar()
    {
        Estado = EstadoDoSegmentoDeViagem.EmViagem;
        ProgressoNaRota = 0f;

        foreach (var marcador in _marcadores)
        {
            marcador.Rearmar();
        }
    }

    public ResultadoAtualizacaoSegmento Atualizar(float velocidadeNormalizada, float delta)
    {
        var resultado = new ResultadoAtualizacaoSegmento();

        if (Estado != EstadoDoSegmentoDeViagem.EmViagem)
        {
            return resultado;
        }

        var progressoAnterior = ProgressoNaRota;
        ProgressoNaRota = Math.Clamp(ProgressoNaRota + velocidadeNormalizada * delta, 0f, 1f);

        if (ProgressoNaRota <= progressoAnterior)
        {
            return resultado;
        }

        foreach (var marcador in _marcadores)
        {
            if (!marcador.DeveExecutar(progressoAnterior, ProgressoNaRota))
            {
                continue;
            }

            marcador.MarcarComoExecutado();
            resultado.AdicionarMarcador(marcador);

            if (marcador.Tipo == TipoMarcadorDeRota.InicioSalto)
            {
                Estado = EstadoDoSegmentoDeViagem.Saltando;
            }
        }

        return resultado;
    }

    public void ConcluirSalto()
    {
        if (Estado == EstadoDoSegmentoDeViagem.Saltando)
        {
            Estado = EstadoDoSegmentoDeViagem.Concluido;
        }
    }
}
