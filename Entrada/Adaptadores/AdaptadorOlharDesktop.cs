using System;
using Godot;

namespace ProjetoPlanetario.Entrada.Adaptadores;

public partial class AdaptadorOlharDesktop : Node
{
    public event Action<Vector2>? MovimentoDoOlhar;
    public event Action? RecentralizacaoSolicitada;
    private bool _olhando;
    public bool Habilitado { get; private set; } = true;

    public void DefinirHabilitado(bool habilitado)
    {
        Habilitado = habilitado;
        if (!habilitado) LiberarMouse();
    }

    public override void _UnhandledInput(InputEvent evento)
    {
        if (!Habilitado) return;
        if (evento is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true })
        {
            _olhando = true;
            Input.MouseMode = Input.MouseModeEnum.Captured;
            GetViewport().SetInputAsHandled();
        }
        if (evento.IsActionPressed("camera_recentralizar"))
            RecentralizacaoSolicitada?.Invoke();
    }

    public override void _Input(InputEvent evento)
    {
        if (evento is InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: false }
            || evento.IsActionPressed("piloto_cancelar"))
            LiberarMouse();
        if (_olhando && evento is InputEventMouseMotion movimento)
            MovimentoDoOlhar?.Invoke(movimento.Relative * 0.12f);
    }

    public override void _Notification(int notificacao)
    {
        if (notificacao == NotificationApplicationFocusOut)
            LiberarMouse();
    }

    public override void _ExitTree() => LiberarMouse();

    private void LiberarMouse()
    {
        if (!_olhando) return;
        _olhando = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
}
