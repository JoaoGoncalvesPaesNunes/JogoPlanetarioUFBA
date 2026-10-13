using Godot;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public partial class SistemaPlanetario : Node3D
{
    private MeshInstance3D _planeta = null!;
    private MeshInstance3D _gigante = null!;

    public override void _Ready()
    {
        _planeta = CriarPlaneta("PlanetaOceanico", new(-31, 12, -125), 19f, false);
        _gigante = CriarPlaneta("GiganteComAneis", new(62, 23, -210), 21f, true);
        var atmosfera = new ShaderMaterial { Shader = GD.Load<Shader>("res://Apresentacao/Shaders/Atmosfera.gdshader") };
        _planeta.AddChild(new MeshInstance3D
        {
            Name = "Atmosfera", Mesh = Esfera(19.35f), MaterialOverride = atmosfera,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        });
        var aneis = new MeshInstance3D
        {
            Name = "Aneis", Mesh = CriarAneis(27f, 43f), RotationDegrees = new(18, 0, -24),
            MaterialOverride = new ShaderMaterial { Shader = GD.Load<Shader>("res://Apresentacao/Shaders/Aneis.gdshader") },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        };
        _gigante.AddChild(aneis);
        var lua = new MeshInstance3D
        {
            Name = "Lua", Position = new(-5, 20, -105), Mesh = Esfera(2.8f),
            MaterialOverride = new StandardMaterial3D { AlbedoColor = new Color("9b9e9b"), Roughness = 1f }
        };
        AddChild(lua);
    }

    public override void _Process(double delta)
    {
        _planeta.RotateY((float)delta * 0.012f);
        // O plano dos aneis nao acompanha a rotacao superficial do gigante.
    }

    private MeshInstance3D CriarPlaneta(string nome, Vector3 posicao, float raio, bool gasoso)
    {
        var material = new ShaderMaterial { Shader = GD.Load<Shader>("res://Apresentacao/Shaders/Planeta.gdshader") };
        material.SetShaderParameter("gasoso", gasoso);
        var planeta = new MeshInstance3D { Name = nome, Position = posicao, Mesh = Esfera(raio), MaterialOverride = material };
        AddChild(planeta);
        return planeta;
    }

    private static SphereMesh Esfera(float raio) => new() { Radius = raio, Height = raio * 2, RadialSegments = 96, Rings = 64 };

    private static ArrayMesh CriarAneis(float interno, float externo)
    {
        var superficie = new SurfaceTool();
        superficie.Begin(Mesh.PrimitiveType.Triangles);
        for (var indice = 0; indice < 192; indice++)
        {
            var a = indice * Mathf.Tau / 192;
            var b = (indice + 1) * Mathf.Tau / 192;
            foreach (var ponto in new[] { new Vector2(a, 0), new Vector2(b, 1), new Vector2(a, 1), new Vector2(a, 0), new Vector2(b, 0), new Vector2(b, 1) })
            {
                var raio = Mathf.Lerp(interno, externo, ponto.Y);
                superficie.SetNormal(Vector3.Up);
                superficie.SetUV(new Vector2(ponto.Y, ponto.X / Mathf.Tau));
                superficie.AddVertex(new Vector3(Mathf.Cos(ponto.X) * raio, 0, Mathf.Sin(ponto.X) * raio));
            }
        }
        return superficie.Commit();
    }
}
