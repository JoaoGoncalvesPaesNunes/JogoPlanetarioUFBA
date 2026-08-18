using Godot;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Apresentacao.HUD;

/// <summary>
/// Overlay visual simples para o salto gravitacional, acionado por eventos.
/// </summary>
public partial class ApresentacaoDoSalto : CanvasLayer
{
    private ColorRect? _flash;
    private Label? _titulo;
    private float _tempo;
    private bool _ativo;

    public void Configurar(BarramentoDeEventos barramentoDeEventos)
    {
        barramentoDeEventos.EventoPublicado += AoEventoPublicado;
    }

    public override void _Ready()
    {
        _flash = new ColorRect
        {
            Color = new Color(1f, 1f, 1f, 0f)
        };
        _flash.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(_flash);

        _titulo = new Label
        {
            Text = string.Empty,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        _titulo.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _titulo.AddThemeFontSizeOverride("font_size", 40);
        AddChild(_titulo);
    }

    public override void _Process(double delta)
    {
        if (!_ativo || _flash is null || _titulo is null)
        {
            return;
        }

        _tempo += (float)delta;
        var alpha = _tempo switch
        {
            < 0.35f => _tempo / 0.35f,
            < 0.9f => 1f,
            < 1.8f => 1f - ((_tempo - 0.9f) / 0.9f),
            _ => 0f
        };

        _flash.Color = new Color(1f, 1f, 1f, Mathf.Clamp(alpha, 0f, 1f));
        _titulo.Text = _tempo < 1.2f ? "SALTO GRAVITACIONAL" : "SALTO CONCLUIDO";
        _titulo.Modulate = new Color(0.05f, 0.08f, 0.1f, Mathf.Clamp(alpha + 0.15f, 0f, 1f));

        if (_tempo >= 2.3f)
        {
            _ativo = false;
            _flash.Color = new Color(1f, 1f, 1f, 0f);
            _titulo.Text = string.Empty;
        }
    }

    public void Ocultar()
    {
        _ativo = false;
        _tempo = 0f;

        if (_flash is not null)
        {
            _flash.Color = new Color(1f, 1f, 1f, 0f);
        }

        if (_titulo is not null)
        {
            _titulo.Text = string.Empty;
        }
    }

    private void AoEventoPublicado(EventoDeDominio eventoDeDominio)
    {
        if (eventoDeDominio is SaltoGravitacionalIniciado)
        {
            _ativo = true;
            _tempo = 0f;
        }

        if (eventoDeDominio is SegmentoDeViagemReiniciado)
        {
            Ocultar();
        }
    }
}
