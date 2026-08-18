using Godot;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Portas;

namespace ProjetoPlanetario.Entrada.Adaptadores;

/// <summary>
/// Traduz o Input Map da Godot para a entrada logica do piloto.
/// </summary>
public sealed class AdaptadorTecladoMouse : IEntradaDoPiloto
{
    public EstadoEntradaPiloto LerEstado()
    {
        var movimentoDaCamera = Input.GetVector(
            "camera_esquerda",
            "camera_direita",
            "camera_cima",
            "camera_baixo");
        var movimentoNaRota = Input.GetActionStrength("piloto_avancar") - Input.GetActionStrength("piloto_recuar");

        return new EstadoEntradaPiloto(
            movimentoDaCamera,
            movimentoNaRota,
            Input.IsActionPressed("piloto_acao_primaria"),
            Input.IsActionJustPressed("piloto_acao_primaria"),
            Input.IsActionJustPressed("piloto_confirmar"),
            Input.IsActionJustPressed("piloto_cancelar"));
    }
}
