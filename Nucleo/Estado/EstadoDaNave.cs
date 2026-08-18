using System;

namespace ProjetoPlanetario.Nucleo.Estado;

/// <summary>
/// Estado puro e minimo da nave, sem dependencia direta da Godot.
/// </summary>
public sealed class EstadoDaNave
{
    public float Energia { get; private set; }
    public float Integridade { get; private set; }
    public float EstabilidadeDaDobra { get; private set; }
    public float CargaDeDobra { get; private set; }

    public EstadoDaNave(float energiaInicial = 100f, float integridadeInicial = 100f, float estabilidadeDaDobraInicial = 62f, float cargaDeDobraInicial = 0f)
    {
        Energia = Math.Clamp(energiaInicial, 0f, 100f);
        Integridade = Math.Clamp(integridadeInicial, 0f, 100f);
        EstabilidadeDaDobra = Math.Clamp(estabilidadeDaDobraInicial, 0f, 100f);
        CargaDeDobra = Math.Clamp(cargaDeDobraInicial, 0f, 100f);
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

    public bool RestaurarEnergia(float quantidade)
    {
        if (quantidade <= 0f || Energia >= 100f)
        {
            return false;
        }

        var energiaAnterior = Energia;
        Energia = Math.Clamp(Energia + quantidade, 0f, 100f);
        return Math.Abs(energiaAnterior - Energia) > 0.001f;
    }

    public bool AlterarEstabilidadeDaDobra(float variacao)
    {
        if (Math.Abs(variacao) <= 0.001f)
        {
            return false;
        }

        var estabilidadeAnterior = EstabilidadeDaDobra;
        EstabilidadeDaDobra = Math.Clamp(EstabilidadeDaDobra + variacao, 0f, 100f);
        return Math.Abs(estabilidadeAnterior - EstabilidadeDaDobra) > 0.001f;
    }

    public bool AdicionarCargaDeDobra(float quantidade)
    {
        if (quantidade <= 0f || CargaDeDobra >= 100f)
        {
            return false;
        }

        var cargaAnterior = CargaDeDobra;
        CargaDeDobra = Math.Clamp(CargaDeDobra + quantidade, 0f, 100f);
        return Math.Abs(cargaAnterior - CargaDeDobra) > 0.001f;
    }

    public void ReiniciarParaTeste(float energiaInicial = 100f, float integridadeInicial = 100f, float estabilidadeDaDobraInicial = 62f)
    {
        Energia = Math.Clamp(energiaInicial, 0f, 100f);
        Integridade = Math.Clamp(integridadeInicial, 0f, 100f);
        EstabilidadeDaDobra = Math.Clamp(estabilidadeDaDobraInicial, 0f, 100f);
        CargaDeDobra = 0f;
    }
}
