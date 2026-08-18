using Godot;

namespace ProjetoPlanetario.Projecao.Portas;

/// <summary>
/// Porta para adaptar a apresentacao sem acoplar gameplay ao monitor ou ao domo.
/// </summary>
public interface IPortaDeProjecao
{
    Rect2 ObterAreaLogica();
    void AplicarEmViewport(Viewport viewport);
}
