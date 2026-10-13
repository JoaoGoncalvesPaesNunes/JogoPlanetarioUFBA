using Godot;

namespace ProjetoPlanetario.Robusto.Cenarios;

[Tool]
public partial class AparenciaAnaBranca : Node3D
{
    [Export(PropertyHint.Range, "0,1.5,0.05")] public float Emissao { get; set; } = 0.65f;
    [Export(PropertyHint.Range, "0,0.5,0.01")] public float ForcaHalo { get; set; } = 0.2f;
    [Export(PropertyHint.Range, "0,5,0.1")] public float EnergiaLuz { get; set; } = 1.8f;
    private ShaderMaterial? _superficie, _halo;
    private OmniLight3D? _luz;
    private Vector3 _ultimaConfiguracao = new(-1, -1, -1);
    public override void _Ready()
    {
        var superficie = GetNode<MeshInstance3D>("Superficie");
        var halo = GetNode<MeshInstance3D>("Halo");
        _superficie = (ShaderMaterial)superficie.Mesh.SurfaceGetMaterial(0);
        _halo = (ShaderMaterial)halo.Mesh.SurfaceGetMaterial(0);
        if (!Engine.IsEditorHint())
        {
            _superficie = (ShaderMaterial)_superficie.Duplicate();
            _halo = (ShaderMaterial)_halo.Duplicate();
            superficie.MaterialOverride = _superficie;
            halo.MaterialOverride = _halo;
        }
        _luz = GetNode<OmniLight3D>("LuzEstelar");
    }
    public override void _Process(double delta)
    {
        if (_superficie is null || _halo is null || _luz is null) return;
        var valores = new Vector3(Emissao, ForcaHalo, EnergiaLuz);
        if (valores == _ultimaConfiguracao) return;
        _ultimaConfiguracao = valores;
        _superficie.SetShaderParameter("emissao", Mathf.Clamp(Emissao, 0, 1.5f));
        _halo.SetShaderParameter("intensidade", Mathf.Clamp(ForcaHalo, 0, 0.5f));
        _luz.LightEnergy = Mathf.Clamp(EnergiaLuz, 0, 5);
    }
}
