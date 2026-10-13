using System.Collections.Generic;
using Godot;

namespace ProjetoPlanetario.Apresentacao.Cenarios;

public partial class CenarioPrototipo : CenarioBase
{
    private const string CaminhoPanoramaEspacial = "res://assets/espaco/space_spheremaps/hazy_nebulae_1.png";
    private readonly List<Node3D> _objetosAnimaveis = new();

    public override void _Ready()
    {
        CriarIluminacao();
        foreach (var filho in FindChildren("*", "Node3D", true, false))
            if (filho is Node3D objeto && objeto.IsInGroup("cenario_animavel"))
                _objetosAnimaveis.Add(objeto);
    }

    public override void AtualizarAmbiente(float delta)
    {
        foreach (var objeto in _objetosAnimaveis)
            if (IsInstanceValid(objeto)) objeto.RotateY(delta * 0.08f);
    }

    private void CriarIluminacao()
    {
        var iluminacao = GetNode<Node3D>("Iluminacao");
        var texturaPanorama = GD.Load<Texture2D>(CaminhoPanoramaEspacial);
        if (texturaPanorama is null)
        {
            GD.PushError($"Nao foi possivel carregar o panorama espacial: {CaminhoPanoramaEspacial}");
        }

        iluminacao.AddChild(new WorldEnvironment
        {
            Name = "AmbienteOnRails",
            Environment = new Godot.Environment
            {
                BackgroundMode = texturaPanorama is null ? Godot.Environment.BGMode.Color : Godot.Environment.BGMode.Sky,
                BackgroundColor = new Color(0.003f, 0.005f, 0.012f),
                Sky = texturaPanorama is null
                    ? null
                    : new Sky
                    {
                        SkyMaterial = new PanoramaSkyMaterial
                        {
                            Panorama = texturaPanorama
                        }
                    },
                AmbientLightSource = Godot.Environment.AmbientSource.Color,
                AmbientLightColor = new Color(0.24f, 0.29f, 0.36f),
                AmbientLightEnergy = 0.4f,
                TonemapMode = Godot.Environment.ToneMapper.Filmic
            }
        });

        iluminacao.AddChild(new DirectionalLight3D
        {
            Name = "LuzPrincipal",
            Rotation = new Vector3(-0.7f, -0.65f, 0.1f),
            LightColor = new Color(1f, 0.91f, 0.79f),
            LightEnergy = 1.65f
        });

        iluminacao.AddChild(new OmniLight3D
        {
            Name = "LuzRimAzulCenario",
            Position = new Vector3(5f, 3f, -18f),
            LightColor = new Color(0.2f, 0.62f, 1f),
            LightEnergy = 1.15f,
            OmniRange = 22f
        });
    }
}
