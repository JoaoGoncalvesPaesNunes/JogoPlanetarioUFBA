using System;

namespace ProjetoPlanetario.Robusto.Dominio;

public enum ContextoDeEntrada { Olhar, Combate, Gelo, Menu, Bloqueado }

/// <summary>Angulos locais da visao; nao possui referencia a nave ou a sua trajetoria.</summary>
public sealed class EstadoDoOlhar
{
    public float Yaw { get; private set; }
    public float Pitch { get; private set; }
    public float YawAlvo { get; private set; }
    public float PitchAlvo { get; private set; }
    public float LimiteYaw { get; set; } = 115;
    public float PitchMinimo { get; set; } = -50;
    public float PitchMaximo { get; set; } = 70;
    public float Suavizacao { get; set; } = 7;
    public float VelocidadeMaxima { get; set; } = 40;
    public float VelocidadeRecentralizacao { get; set; } = 32;
    public bool Centralizando { get; private set; }

    public void Mover(float yaw, float pitch)
    {
        if (!float.IsFinite(yaw) || !float.IsFinite(pitch)) return;
        YawAlvo = Math.Clamp(YawAlvo + yaw, -Math.Abs(LimiteYaw), Math.Abs(LimiteYaw));
        PitchAlvo = Math.Clamp(PitchAlvo + pitch, Math.Min(PitchMinimo, PitchMaximo), Math.Max(PitchMinimo, PitchMaximo));
        Centralizando = false;
    }
    public void Recentralizar() { YawAlvo = 0; PitchAlvo = 0; Centralizando = true; }
    public void Atualizar(float delta)
    {
        if (!float.IsFinite(delta) || delta <= 0) return;
        var fator = 1 - MathF.Exp(-Math.Max(0.1f, Suavizacao) * delta);
        var maximo = Math.Max(0.1f, Centralizando ? VelocidadeRecentralizacao : VelocidadeMaxima) * delta;
        var x = (YawAlvo - Yaw) * fator;
        var y = (PitchAlvo - Pitch) * fator;
        var comprimento = MathF.Sqrt(x * x + y * y);
        var escala = comprimento > maximo ? maximo / comprimento : 1;
        Yaw += x * escala;
        Pitch += y * escala;
    }
}
