using System;

namespace ProjetoPlanetario.Jogabilidade.Viagem;

public enum FaseDoSalto { Normal, AguardandoDestino, Preparando, Acelerando, Hiperespaco, Desacelerando, Chegada, Falha }

/// <summary>Fluxo puro: nao conhece UI, Godot, arquivos nem dispositivos.</summary>
public sealed class ControladorDoSalto
{
    public FaseDoSalto Fase { get; private set; }
    public string DestinoSelecionado { get; private set; } = "";
    public float TempoNaFase { get; private set; }
    public bool DestinoCarregado { get; private set; }
    public float EscalaDuracao { get; set; } = 1;
    private float EscalaSegura => float.IsFinite(EscalaDuracao) ? Math.Max(0.1f, EscalaDuracao) : 1;
    public bool AceitaEscolha => Fase is FaseDoSalto.AguardandoDestino or FaseDoSalto.Falha;
    public bool PrecisaCarregar => Fase == FaseDoSalto.Hiperespaco && !DestinoCarregado && TempoNaFase >= 0.2f;
    public event Action<FaseDoSalto>? FaseAlterada;

    public float Intensidade => Fase switch
    {
        FaseDoSalto.Acelerando => Suavizar(TempoNaFase / (1.2f * EscalaSegura)),
        FaseDoSalto.Hiperespaco => 1f,
        FaseDoSalto.Desacelerando => 1f - Suavizar(TempoNaFase / (1.2f * EscalaSegura)),
        _ => 0f
    };

    public void SolicitarDestino()
    {
        if (Fase == FaseDoSalto.Normal) MudarFase(FaseDoSalto.AguardandoDestino);
    }

    // A sequencia robusta escolhe internamente; nao abre escolhas para o publico.
    public bool IniciarAutomatico(string destino)
    {
        if (Fase != FaseDoSalto.Normal || string.IsNullOrWhiteSpace(destino)) return false;
        DestinoSelecionado = destino;
        DestinoCarregado = false;
        MudarFase(FaseDoSalto.Preparando);
        return true;
    }

    public bool EscolherDestino(string destino)
    {
        if (!AceitaEscolha || destino is not ("marte" or "jupiter")) return false;
        DestinoSelecionado = destino;
        DestinoCarregado = false;
        MudarFase(FaseDoSalto.Preparando);
        return true;
    }

    public void Atualizar(float delta)
    {
        if (!float.IsFinite(delta) || delta <= 0) return;
        TempoNaFase += delta;
        switch (Fase)
        {
            case FaseDoSalto.Preparando when TempoNaFase >= EscalaSegura: MudarFase(FaseDoSalto.Acelerando); break;
            case FaseDoSalto.Acelerando when TempoNaFase >= 1.2f * EscalaSegura: MudarFase(FaseDoSalto.Hiperespaco); break;
            case FaseDoSalto.Hiperespaco when TempoNaFase >= 1.6f * EscalaSegura && DestinoCarregado: MudarFase(FaseDoSalto.Desacelerando); break;
            case FaseDoSalto.Desacelerando when TempoNaFase >= 1.2f * EscalaSegura: MudarFase(FaseDoSalto.Chegada); break;
        }
    }

    public void ConfirmarCarregamento(bool sucesso)
    {
        if (!PrecisaCarregar) return;
        DestinoCarregado = sucesso;
        if (!sucesso) MudarFase(FaseDoSalto.Falha);
    }

    public void Reiniciar()
    {
        DestinoSelecionado = "";
        DestinoCarregado = false;
        MudarFase(FaseDoSalto.Normal);
    }

    private void MudarFase(FaseDoSalto fase)
    {
        Fase = fase;
        TempoNaFase = 0;
        FaseAlterada?.Invoke(fase);
    }

    private static float Suavizar(float valor)
    {
        var t = Math.Clamp(valor, 0f, 1f);
        return t * t * (3f - 2f * t);
    }
}
