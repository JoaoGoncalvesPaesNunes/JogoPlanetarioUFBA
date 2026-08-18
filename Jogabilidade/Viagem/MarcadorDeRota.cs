using System;

namespace ProjetoPlanetario.Jogabilidade.Viagem;

/// <summary>
/// Marcador narrativo disparado por progresso fisico na rota.
/// </summary>
public sealed class MarcadorDeRota
{
    public string Id { get; }
    public float Progresso { get; }
    public TipoMarcadorDeRota Tipo { get; }
    public bool ExecutarUmaVez { get; }
    public bool FoiExecutado { get; private set; }

    public MarcadorDeRota(string id, float progresso, TipoMarcadorDeRota tipo, bool executarUmaVez = true)
    {
        Id = id;
        Progresso = Math.Clamp(progresso, 0f, 1f);
        Tipo = tipo;
        ExecutarUmaVez = executarUmaVez;
    }

    public bool DeveExecutar(float progressoAnterior, float progressoAtual)
    {
        if (ExecutarUmaVez && FoiExecutado)
        {
            return false;
        }

        return progressoAnterior < Progresso && progressoAtual >= Progresso;
    }

    public void MarcarComoExecutado()
    {
        FoiExecutado = true;
    }

    public void Rearmar()
    {
        FoiExecutado = false;
    }
}
