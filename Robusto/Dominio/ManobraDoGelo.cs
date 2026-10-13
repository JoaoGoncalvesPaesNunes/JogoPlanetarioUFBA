using System;

namespace ProjetoPlanetario.Robusto.Dominio;

public enum FaseDoGelo { Aproximacao, Alerta, Evasao, Concluido }

public sealed class ManobraDoGelo
{
    public FaseDoGelo Fase { get; private set; }
    public float Tempo { get; private set; }
    public float Correcao { get; private set; }
    public int Lado { get; private set; } = 1;
    public float Aproximacao { get; }
    public float TempoDeCorrecao { get; }
    public float DuracaoEvasao { get; }
    public ManobraDoGelo(float aproximacao, float correcao, float evasao)
    {
        Aproximacao = Math.Max(0.1f, aproximacao);
        TempoDeCorrecao = Math.Max(0.1f, correcao);
        DuracaoEvasao = Math.Max(0.1f, evasao);
    }
    public void Atualizar(float delta, float horizontal)
    {
        if (!float.IsFinite(delta) || delta <= 0 || Fase == FaseDoGelo.Concluido) return;
        Tempo += delta;
        switch (Fase)
        {
            case FaseDoGelo.Aproximacao when Tempo >= Aproximacao:
                Mudar(FaseDoGelo.Alerta); break;
            case FaseDoGelo.Alerta:
                if (Math.Abs(horizontal) > 0.4f)
                {
                    if (Math.Sign(horizontal) != Lado) Correcao = 0;
                    Lado = Math.Sign(horizontal);
                    Correcao = Math.Min(1, Correcao + delta / TempoDeCorrecao);
                }
                else Correcao = Math.Max(0, Correcao - delta * 0.2f);
                if (Correcao >= 1) Mudar(FaseDoGelo.Evasao);
                break;
            case FaseDoGelo.Evasao when Tempo >= DuracaoEvasao:
                Mudar(FaseDoGelo.Concluido); break;
        }
    }
    private void Mudar(FaseDoGelo fase) { Fase = fase; Tempo = 0; }
}
