using Godot;

namespace ProjetoPlanetario.Dados.Nave;

/// <summary>
/// Resource com parametros ajustaveis da nave do microprototipo.
/// </summary>
[GlobalClass]
public partial class ConfiguracaoDaNave : Resource
{
    [Export] public float Velocidade { get; set; } = 8f;
    [Export] public float ConsumoEnergiaAcaoPrimaria { get; set; } = 10f;
    [Export] public float SuavizacaoRotacao { get; set; } = 8f;
    [Export] public float SensibilidadeDeMira { get; set; } = 1.8f;
    [Export] public float SensibilidadeDaCamera { get; set; } = 75f;
    [Export] public float LimiteCameraHorizontalGraus { get; set; } = 70f;
    [Export] public float LimiteCameraVerticalGraus { get; set; } = 35f;
    [Export] public float GanhoEstabilidadeAlinhado { get; set; } = 8f;
    [Export] public float PerdaEstabilidadeDesalinhado { get; set; } = 5f;
    [Export] public float AceleracaoFrontal { get; set; } = 1.7f;
    [Export] public float FreioFrontal { get; set; } = 2.4f;
    [Export] public float ArrastoFrontal { get; set; } = 0.45f;
    [Export] public float VelocidadeFrontalMaxima { get; set; } = 1f;
    [Export] public float VelocidadeReMaxima { get; set; } = 0.45f;
}
