using Godot;

namespace ProjetoPlanetario.Robusto.Projecao;

public partial class GradeCalibracao : Control
{
    public float CampoDeVisao { get; set; } = 180;
    public override void _Draw()
    {
        var centro = Size / 2;
        var raio = Mathf.Min(Size.X, Size.Y) * 0.49f;
        var cor = new Color(0.2f, 0.9f, 0.7f, 0.7f);
        for (var i = 1; i <= 3; i++) DrawArc(centro, raio * i / 3, 0, Mathf.Tau, 128, cor, 1.5f, true);
        for (var i = 0; i < 12; i++)
        {
            var direcao = Vector2.FromAngle(i * Mathf.Tau / 12);
            DrawLine(centro, centro + direcao * raio, cor, 1, true);
        }
        DrawCircle(centro, 4, Colors.White);
        var fonte = ThemeDB.FallbackFont;
        DrawString(fonte, centro + new Vector2(-25, -raio * 0.85f), "FRENTE", fontSize: 18);
        DrawString(fonte, centro + new Vector2(raio * 0.72f, 0), "DIR", fontSize: 18);
        DrawString(fonte, centro + new Vector2(-raio * 0.85f, 0), "ESQ", fontSize: 18);
        DrawString(fonte, centro + new Vector2(-20, raio * 0.88f), "TRAS", fontSize: 18);
        DrawString(fonte, centro + new Vector2(10, -10), "0 / EIXO OPTICO", fontSize: 15);
        for (var i = 1; i <= 3; i++) DrawString(fonte, centro + new Vector2(12, -raio * i / 3 + 20), $"{i * CampoDeVisao / 6:F0} graus", fontSize: 15);
    }
}
