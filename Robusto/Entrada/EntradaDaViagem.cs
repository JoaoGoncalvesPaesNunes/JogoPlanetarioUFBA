using Godot;

namespace ProjetoPlanetario.Robusto.Entrada;

public static class EntradaDaViagem
{
    public static void Configurar()
    {
        Acao("mira_esquerda", JoyAxis.LeftX, -1, Key.A, Key.Left);
        Acao("mira_direita", JoyAxis.LeftX, 1, Key.D, Key.Right);
        Acao("mira_cima", JoyAxis.LeftY, -1, Key.W, Key.Up);
        Acao("mira_baixo", JoyAxis.LeftY, 1, Key.S, Key.Down);
        Acao("disparar", null, 0, Key.Space);
        Acao("escudo", null, 0, Key.E);
        Acao("recentralizar_visao", null, 0, Key.R);
        InputMap.ActionAddEvent("recentralizar_visao", new InputEventJoypadButton { ButtonIndex = JoyButton.RightStick });
        InputMap.ActionAddEvent("disparar", new InputEventMouseButton { ButtonIndex = MouseButton.Left });
        InputMap.ActionAddEvent("disparar", new InputEventJoypadButton { ButtonIndex = JoyButton.A });
        InputMap.ActionAddEvent("escudo", new InputEventJoypadButton { ButtonIndex = JoyButton.B });
    }
    public static Vector2 LerMira() => Input.GetVector("mira_esquerda", "mira_direita", "mira_cima", "mira_baixo");
    private static void Acao(string nome, JoyAxis? eixo, float sentido, params Key[] teclas)
    {
        if (InputMap.HasAction(nome)) return;
        InputMap.AddAction(nome, 0.25f);
        foreach (var tecla in teclas) InputMap.ActionAddEvent(nome, new InputEventKey { PhysicalKeycode = tecla });
        if (eixo.HasValue) InputMap.ActionAddEvent(nome, new InputEventJoypadMotion { Axis = eixo.Value, AxisValue = sentido });
    }
}
