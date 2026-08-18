using Godot;
using ProjetoPlanetario.Projecao.Portas;

namespace ProjetoPlanetario.Projecao.Adaptadores;

public sealed class AdaptadorProjecaoDesktop : IPortaDeProjecao
{
    private readonly Rect2 _areaLogica;

    public AdaptadorProjecaoDesktop(Rect2 areaLogica)
    {
        _areaLogica = areaLogica;
    }

    public Rect2 ObterAreaLogica()
    {
        return _areaLogica;
    }

    public void AplicarEmViewport(Viewport viewport)
    {
        if (viewport is Window janela)
        {
            janela.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
            janela.ContentScaleAspect = Window.ContentScaleAspectEnum.Keep;
        }
    }
}
