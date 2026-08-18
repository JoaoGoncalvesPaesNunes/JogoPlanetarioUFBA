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
        var movimento = Input.GetVector(
            "piloto_mover_esquerda",
            "piloto_mover_direita",
            "piloto_mover_cima",
            "piloto_mover_baixo");

        return new EstadoEntradaPiloto(
            movimento,
            Input.IsActionPressed("piloto_acao_primaria"),
            Input.IsActionJustPressed("piloto_acao_primaria"),
            Input.IsActionJustPressed("piloto_confirmar"),
            Input.IsActionJustPressed("piloto_cancelar"));
    }
}
