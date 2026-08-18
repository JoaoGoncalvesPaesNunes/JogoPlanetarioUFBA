using Godot;
using ProjetoPlanetario.Projecao.Portas;

namespace ProjetoPlanetario.Projecao.Adaptadores;

/// <summary>
/// Stub para futura integracao com especificacoes reais do Planetario da UFBA.
/// </summary>
public sealed class AdaptadorProjecaoPlanetario : IPortaDeProjecao
{
    public Rect2 ObterAreaLogica()
    {
        return new Rect2(Vector2.Zero, new Vector2(1280, 720));
    }

    public void AplicarEmViewport(Viewport viewport)
    {
        GD.PushWarning("AdaptadorProjecaoPlanetario e apenas um stub ate haver especificacao real do domo.");
    }
}
