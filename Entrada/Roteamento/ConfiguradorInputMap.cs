using Godot;

namespace ProjetoPlanetario.Entrada.Roteamento;

/// <summary>
/// Garante mapeamentos logicos iniciais para desktop sem espalhar teclas pela jogabilidade.
/// </summary>
public static class ConfiguradorInputMap
{
    public static void GarantirAcoesDesktopDev()
    {
        GarantirAcaoDeTeclas("piloto_avancar", Key.W);
        GarantirAcaoDeTeclas("piloto_recuar", Key.S);
        GarantirAcaoDeTeclas("camera_esquerda", Key.A, Key.Left);
        GarantirAcaoDeTeclas("camera_direita", Key.D, Key.Right);
        GarantirAcaoDeTeclas("camera_cima", Key.Up);
        GarantirAcaoDeTeclas("camera_baixo", Key.Down);
        GarantirAcaoDeTeclas("piloto_mover_esquerda", Key.A, Key.Left);
        GarantirAcaoDeTeclas("piloto_mover_direita", Key.D, Key.Right);
        GarantirAcaoDeTeclas("piloto_mover_cima", Key.Up);
        GarantirAcaoDeTeclas("piloto_mover_baixo", Key.Down);
        GarantirAcaoDeTeclas("piloto_acelerar", Key.W);
        GarantirAcaoDeTeclas("piloto_frear", Key.S);
        GarantirAcaoDeTeclas("piloto_acao_primaria", Key.Space);
        GarantirAcaoDeTeclas("piloto_confirmar", Key.Enter);
        GarantirAcaoDeTeclas("piloto_cancelar", Key.Escape);
    }

    private static void GarantirAcaoDeTeclas(string nomeDaAcao, params Key[] teclas)
    {
        if (!InputMap.HasAction(nomeDaAcao))
        {
            InputMap.AddAction(nomeDaAcao, deadzone: 0.5f);
        }

        foreach (var tecla in teclas)
        {
            if (AcaoJaPossuiTecla(nomeDaAcao, tecla))
            {
                continue;
            }

            var evento = new InputEventKey
            {
                PhysicalKeycode = tecla
            };

            InputMap.ActionAddEvent(nomeDaAcao, evento);
        }
    }

    private static bool AcaoJaPossuiTecla(string nomeDaAcao, Key tecla)
    {
        foreach (var evento in InputMap.ActionGetEvents(nomeDaAcao))
        {
            if (evento is InputEventKey eventoDeTecla && eventoDeTecla.PhysicalKeycode == tecla)
            {
                return true;
            }
        }

        return false;
    }
}
