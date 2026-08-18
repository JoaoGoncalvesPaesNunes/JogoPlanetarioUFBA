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
    [Export] public float GanhoEstabilidadeAlinhado { get; set; } = 8f;
    [Export] public float PerdaEstabilidadeDesalinhado { get; set; } = 5f;
    [Export] public float AceleracaoFrontal { get; set; } = 1.7f;
    [Export] public float FreioFrontal { get; set; } = 2.4f;
    [Export] public float ArrastoFrontal { get; set; } = 0.45f;
    [Export] public float VelocidadeFrontalMaxima { get; set; } = 1f;
}
