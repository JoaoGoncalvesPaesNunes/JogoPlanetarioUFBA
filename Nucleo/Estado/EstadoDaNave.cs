using System;

namespace ProjetoPlanetario.Nucleo.Estado;

/// <summary>
/// Estado puro e minimo da nave, sem dependencia direta da Godot.
/// </summary>
public sealed class EstadoDaNave
{
    public float Energia { get; private set; }
    public float Integridade { get; private set; }

    public EstadoDaNave(float energiaInicial = 100f, float integridadeInicial = 100f)
    {
        Energia = Math.Clamp(energiaInicial, 0f, 100f);
        Integridade = Math.Clamp(integridadeInicial, 0f, 100f);
    }

    public bool ConsumirEnergia(float quantidade)
    {
        if (quantidade <= 0f || Energia <= 0f)
        {
            return false;
        }

        var energiaAnterior = Energia;
        Energia = Math.Clamp(Energia - quantidade, 0f, 100f);
        return Math.Abs(energiaAnterior - Energia) > 0.001f;
    }

    public bool ReceberDano(float quantidade)
    {
        if (quantidade <= 0f || Integridade <= 0f)
        {
            return false;
        }

        var integridadeAnterior = Integridade;
        Integridade = Math.Clamp(Integridade - quantidade, 0f, 100f);
        return Math.Abs(integridadeAnterior - Integridade) > 0.001f;
    }
}
