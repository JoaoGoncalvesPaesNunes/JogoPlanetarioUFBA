using Godot;
using ProjetoPlanetario.Jogabilidade.Voo;
using ProjetoPlanetario.Nucleo.Estado;

namespace ProjetoPlanetario.Apresentacao.HUD;

/// <summary>
/// Camada visual temporaria do cockpit. Desenha formas simples para validar leitura e composicao.
/// </summary>
public partial class CockpitDoPiloto : Control
{
    private ControladorDeVoo? _controladorDeVoo;
    private EstadoDaNave? _estadoDaNave;
    private float _pulso;
    private string _rotaAtual = "Ponto de dobra";

    public void Configurar(ControladorDeVoo controladorDeVoo, EstadoDaNave estadoDaNave)
    {
        _controladorDeVoo = controladorDeVoo;
        _estadoDaNave = estadoDaNave;
    }

    public void DefinirRotaAtual(string rotaAtual)
    {
        _rotaAtual = rotaAtual;
    }

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public override void _Process(double delta)
    {
        _pulso += (float)delta;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_controladorDeVoo is null || _estadoDaNave is null)
        {
            return;
        }

        var tamanho = Size;
        var centro = tamanho * 0.5f;
        var margemLateral = tamanho.X * 0.11f;
        var topoJanela = tamanho.Y * 0.08f;
        var baseJanela = tamanho.Y * 0.78f;

        DesenharJanela(tamanho, margemLateral, topoJanela, baseJanela);
        DesenharMarcadoresDeRota(centro, tamanho);
        DesenharPainelInferior(tamanho);
        DesenharBarras(tamanho);
    }

    private void DesenharJanela(Vector2 tamanho, float margemLateral, float topoJanela, float baseJanela)
    {
        var corEstrutura = new Color(0.02f, 0.025f, 0.03f, 0.94f);
        var corBorda = new Color(0.0f, 0.75f, 0.95f, 0.38f);

        DrawRect(new Rect2(Vector2.Zero, new Vector2(tamanho.X, topoJanela)), corEstrutura);
        DrawRect(new Rect2(Vector2.Zero, new Vector2(margemLateral, tamanho.Y)), corEstrutura);
        DrawRect(new Rect2(new Vector2(tamanho.X - margemLateral, 0f), new Vector2(margemLateral, tamanho.Y)), corEstrutura);
        DrawRect(new Rect2(new Vector2(0f, baseJanela), new Vector2(tamanho.X, tamanho.Y - baseJanela)), corEstrutura);

        DrawLine(new Vector2(margemLateral, topoJanela), new Vector2(tamanho.X - margemLateral, topoJanela), corBorda, 2f);
        DrawLine(new Vector2(margemLateral, baseJanela), new Vector2(tamanho.X - margemLateral, baseJanela), corBorda, 2f);
        DrawLine(new Vector2(margemLateral, topoJanela), new Vector2(margemLateral, baseJanela), corBorda, 2f);
        DrawLine(new Vector2(tamanho.X - margemLateral, topoJanela), new Vector2(tamanho.X - margemLateral, baseJanela), corBorda, 2f);

        DrawLine(new Vector2(tamanho.X * 0.32f, baseJanela), new Vector2(tamanho.X * 0.45f, tamanho.Y), corEstrutura, 26f);
        DrawLine(new Vector2(tamanho.X * 0.68f, baseJanela), new Vector2(tamanho.X * 0.55f, tamanho.Y), corEstrutura, 26f);
    }

    private void DesenharMarcadoresDeRota(Vector2 centro, Vector2 tamanho)
    {
        if (_controladorDeVoo is null)
        {
            return;
        }

        var escala = new Vector2(tamanho.X * 0.22f, tamanho.Y * 0.22f);
        var posicaoReticulo = centro + new Vector2(_controladorDeVoo.ApontamentoDoPiloto.X * escala.X, -_controladorDeVoo.ApontamentoDoPiloto.Y * escala.Y);
        var posicaoDestino = centro + new Vector2(_controladorDeVoo.PontoDeDobra.X * escala.X, -_controladorDeVoo.PontoDeDobra.Y * escala.Y);
        var corReticulo = new Color(0.85f, 0.98f, 1f, 0.92f);
        var corDestino = _controladorDeVoo.AlinhamentoDaRota > 0.76f
            ? new Color(0.1f, 1f, 0.52f, 0.95f)
            : new Color(1f, 0.72f, 0.18f, 0.95f);

        DrawLine(posicaoReticulo + new Vector2(-20f, 0f), posicaoReticulo + new Vector2(20f, 0f), corReticulo, 2f);
        DrawLine(posicaoReticulo + new Vector2(0f, -20f), posicaoReticulo + new Vector2(0f, 20f), corReticulo, 2f);
        DrawCircle(posicaoReticulo, 8f, new Color(0.85f, 0.98f, 1f, 0.24f));

        var raio = 19f + Mathf.Sin(_pulso * 5f) * 2f;
        DrawCircle(posicaoDestino, raio, new Color(corDestino.R, corDestino.G, corDestino.B, 0.14f));
        DrawCircle(posicaoDestino, 5f, corDestino);
        DrawLine(posicaoReticulo, posicaoDestino, new Color(0.45f, 0.95f, 1f, 0.22f), 1f);
    }

    private void DesenharPainelInferior(Vector2 tamanho)
    {
        var y = tamanho.Y * 0.78f;
        DrawRect(new Rect2(new Vector2(0f, y), new Vector2(tamanho.X, tamanho.Y - y)), new Color(0.015f, 0.018f, 0.02f, 0.98f));
        DrawRect(new Rect2(new Vector2(tamanho.X * 0.12f, y + 16f), new Vector2(tamanho.X * 0.76f, 4f)), new Color(0f, 0.8f, 0.95f, 0.45f));
        DrawRect(new Rect2(new Vector2(tamanho.X * 0.38f, y + 44f), new Vector2(tamanho.X * 0.24f, 84f)), new Color(0.05f, 0.08f, 0.09f, 0.96f));
        DrawRect(new Rect2(new Vector2(tamanho.X * 0.08f, y + 54f), new Vector2(tamanho.X * 0.22f, 96f)), new Color(0.04f, 0.05f, 0.055f, 0.96f));
        DrawRect(new Rect2(new Vector2(tamanho.X * 0.70f, y + 54f), new Vector2(tamanho.X * 0.22f, 96f)), new Color(0.04f, 0.05f, 0.055f, 0.96f));
    }

    private void DesenharBarras(Vector2 tamanho)
    {
        if (_controladorDeVoo is null || _estadoDaNave is null)
        {
            return;
        }

        var y = tamanho.Y * 0.835f;
        DesenharBarra(new Vector2(tamanho.X * 0.1f, y), new Vector2(tamanho.X * 0.18f, 18f), _estadoDaNave.Energia / 100f, new Color(0.15f, 0.75f, 1f, 0.95f));
        DesenharBarra(new Vector2(tamanho.X * 0.41f, y), new Vector2(tamanho.X * 0.18f, 18f), _controladorDeVoo.AlinhamentoDaRota, new Color(0.95f, 0.85f, 0.25f, 0.95f));
        DesenharBarra(new Vector2(tamanho.X * 0.72f, y), new Vector2(tamanho.X * 0.18f, 18f), _estadoDaNave.EstabilidadeDaDobra / 100f, new Color(0.2f, 1f, 0.55f, 0.95f));
    }

    private void DesenharBarra(Vector2 posicao, Vector2 tamanho, float proporcao, Color cor)
    {
        var valor = Mathf.Clamp(proporcao, 0f, 1f);
        DrawRect(new Rect2(posicao, tamanho), new Color(0.0f, 0.0f, 0.0f, 0.7f));
        DrawRect(new Rect2(posicao + new Vector2(2f, 2f), new Vector2((tamanho.X - 4f) * valor, tamanho.Y - 4f)), cor);
        DrawRect(new Rect2(posicao, tamanho), new Color(0.8f, 0.95f, 1f, 0.35f), false, 1f);
    }
}
