using Godot;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public partial class CeuEstelar : Node3D
{
    private const ulong SementeDoCeu = 407204;
    private const int QuantidadeEstrelasProfundas = 620;
    private const int QuantidadeEstrelasFaixa = 260;
    private const float RaioDoDomo = 115f;

    public override void _Ready()
    {
        CriarDomoEscuro();
        CriarEstrelasProfundas();
        CriarFaixaGalactica();
    }

    public override void _Process(double delta)
    {
        RotateY((float)delta * 0.0025f);
    }

    private void CriarDomoEscuro()
    {
        AddChild(new MeshInstance3D
        {
            Name = "DomoEscuroEspacial",
            Mesh = new SphereMesh
            {
                Radius = RaioDoDomo,
                Height = RaioDoDomo * 2f,
                RadialSegments = 96,
                Rings = 48
            },
            MaterialOverride = new StandardMaterial3D
            {
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
                CullMode = BaseMaterial3D.CullModeEnum.Disabled,
                AlbedoColor = new Color(0.0015f, 0.003f, 0.009f),
                EmissionEnabled = true,
                Emission = new Color(0.0015f, 0.003f, 0.009f),
                EmissionEnergyMultiplier = 1f
            }
        });
    }

    private void CriarEstrelasProfundas()
    {
        var rng = new RandomNumberGenerator { Seed = SementeDoCeu };
        var multimesh = CriarMultimeshDeEstrelas(QuantidadeEstrelasProfundas);

        for (var indice = 0; indice < QuantidadeEstrelasProfundas; indice++)
        {
            var direcao = CriarDirecaoAleatoria(rng);
            var distancia = rng.RandfRange(RaioDoDomo * 0.72f, RaioDoDomo * 0.98f);
            var escala = rng.RandfRange(0.035f, 0.105f);
            var brilho = rng.RandfRange(0.62f, 1f);

            multimesh.SetInstanceTransform(indice, CriarTransformacaoEstrela(direcao * distancia, escala));
            multimesh.SetInstanceColor(indice, CriarCorEstrela(rng, brilho));
        }

        AddChild(new MultiMeshInstance3D
        {
            Name = "EstrelasProfundas",
            Multimesh = multimesh
        });
    }

    private void CriarFaixaGalactica()
    {
        var rng = new RandomNumberGenerator { Seed = SementeDoCeu + 99 };
        var multimesh = CriarMultimeshDeEstrelas(QuantidadeEstrelasFaixa);

        for (var indice = 0; indice < QuantidadeEstrelasFaixa; indice++)
        {
            var direcao = CriarDirecaoNaFaixaGalactica(rng);
            var distancia = rng.RandfRange(RaioDoDomo * 0.74f, RaioDoDomo * 0.96f);
            var escala = rng.RandfRange(0.055f, 0.16f);
            var brilho = rng.RandfRange(0.18f, 0.72f);

            multimesh.SetInstanceTransform(indice, CriarTransformacaoEstrela(direcao * distancia, escala));
            multimesh.SetInstanceColor(indice, new Color(0.48f * brilho, 0.62f * brilho, 0.95f * brilho, 0.76f));
        }

        var faixa = new MultiMeshInstance3D
        {
            Name = "FaixaGalacticaSutil",
            Rotation = new Vector3(0.18f, 0.32f, -0.24f),
            Multimesh = multimesh
        };
        AddChild(faixa);
    }

    private static MultiMesh CriarMultimeshDeEstrelas(int quantidade)
    {
        var estrela = new SphereMesh
        {
            Radius = 1f,
            Height = 2f,
            RadialSegments = 8,
            Rings = 4
        };

        var material = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            VertexColorUseAsAlbedo = true,
            EmissionEnabled = true,
            Emission = Colors.White,
            EmissionEnergyMultiplier = 1.4f
        };
        estrela.Material = material;

        var multimesh = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            UseColors = true,
            Mesh = estrela,
            InstanceCount = quantidade
        };

        return multimesh;
    }

    private static Transform3D CriarTransformacaoEstrela(Vector3 posicao, float escala)
    {
        return new Transform3D(Basis.Identity.Scaled(Vector3.One * escala), posicao);
    }

    private static Vector3 CriarDirecaoAleatoria(RandomNumberGenerator rng)
    {
        var altura = rng.RandfRange(-0.88f, 0.88f);
        var angulo = rng.RandfRange(0f, Mathf.Tau);
        var raioHorizontal = Mathf.Sqrt(1f - altura * altura);

        return new Vector3(
            raioHorizontal * Mathf.Cos(angulo),
            altura,
            raioHorizontal * Mathf.Sin(angulo));
    }

    private static Vector3 CriarDirecaoNaFaixaGalactica(RandomNumberGenerator rng)
    {
        var altura = Mathf.Clamp(rng.RandfRange(-0.18f, 0.18f) + rng.RandfRange(-0.18f, 0.18f), -0.3f, 0.3f);
        var angulo = rng.RandfRange(0f, Mathf.Tau);
        var raioHorizontal = Mathf.Sqrt(1f - altura * altura);

        return new Vector3(
            raioHorizontal * Mathf.Cos(angulo),
            altura,
            raioHorizontal * Mathf.Sin(angulo));
    }

    private static Color CriarCorEstrela(RandomNumberGenerator rng, float brilho)
    {
        var sorteio = rng.Randf();
        if (sorteio < 0.18f)
        {
            return new Color(0.58f * brilho, 0.72f * brilho, 1f * brilho, 1f);
        }

        if (sorteio > 0.9f)
        {
            return new Color(1f * brilho, 0.82f * brilho, 0.56f * brilho, 1f);
        }

        return new Color(0.88f * brilho, 0.94f * brilho, 1f * brilho, 1f);
    }
}
