using Godot;

namespace ProjetoPlanetario.Apresentacao.Cenas;

/// <summary>Cabine solidaria a nave; a camera pode olhar livremente dentro dela.</summary>
public partial class InteriorDaNave : Node3D
{
    [Export] public bool CabineCompacta { get; set; }
    public override void _Ready()
    {
        var casco = Material(new Color("353c42"), 0.65f);
        var juntas = Material(new Color("101518"), 0.35f);
        var acabamento = Material(new Color("89918f"), 0.55f);
        var luzFria = Material(new Color("64e5cd"), 0.1f, true);
        var luzQuente = Material(new Color("f0be68"), 0.1f, true);
        var tela = Material(new Color("12352f"), 0f, true);
        var vidro = new ShaderMaterial { Shader = GD.Load<Shader>("res://Apresentacao/Shaders/VidroPanoramico.gdshader") };
        var fim = CabineCompacta ? 1.45f : 5.5f;
        var centro = (fim - 2.6f) / 2;
        Caixa("Piso", new(0, -1.48f, centro), new(4.8f, 0.16f, fim + 2.7f), casco);
        Caixa("AnteparaTraseira", new(0, 0.25f, fim), new(4.8f, 3.55f, 0.18f), casco);
        if (!CabineCompacta) Caixa("PortaTraseira", new(0, -0.1f, 5.37f), new(1.15f, 2.5f, 0.12f), juntas);
        CriarCoberturaPanoramica(vidro, casco);
        Caixa("ConsoleFrontal", new(0, -1.02f, -1.65f), new(3.5f, 0.45f, 1.0f), casco);
        Caixa("BordaConsole", new(0, -0.765f, -1.3f), new(3.45f, 0.045f, 0.055f), acabamento);
        foreach (var lado in new[] { -1f, 1f })
        {
            Caixa("RodapeLateral", new(lado * 2.3f, -1.15f, centro), new(0.12f, 0.55f, fim + 2.6f), casco);
            Caixa("ConsoleLateral", new(lado * 1.95f, -1.02f, -0.65f), new(0.4f, 0.28f, 1.7f), casco);
            Caixa("LuzRodape", new(lado * 2.2f, -0.85f, centro), new(0.025f, 0.025f, fim + 2.4f), luzFria);
            if (!CabineCompacta) Caixa("TrilhoPiso", new(lado * 0.55f, -1.38f, 1.6f), new(0.035f, 0.025f, 7.5f), acabamento);
            // Vidros laterais separados pela porta dupla, sem parede opaca na altura do olhar.
            PainelVidro("JanelaLateralFrontal", new(lado * 2.3f, -0.87f, -2.25f), new(lado * 2.3f, 1.7f, -2.25f),
                new(lado * 2.3f, 1.7f, 0.8f), new(lado * 2.3f, -0.87f, 0.8f), vidro);
            if (CabineCompacta)
            {
                PainelVidro("JanelaLateralCabine", new(lado * 2.3f, -0.87f, 0.8f), new(lado * 2.3f, 1.7f, 0.8f),
                    new(lado * 2.3f, 1.7f, fim), new(lado * 2.3f, -0.87f, fim), vidro);
                Viga(new(lado * 2.3f, -1.4f, fim), new(lado * 2.3f, 1.95f, fim), 0.065f, casco);
                continue;
            }
            PainelVidro("JanelaSalaPassageiros", new(lado * 2.3f, -0.87f, 2.2f), new(lado * 2.3f, 1.7f, 2.2f),
                new(lado * 2.3f, 1.7f, 5.4f), new(lado * 2.3f, -0.87f, 5.4f), vidro);
            foreach (var z in new[] { 0.8f, 2.2f, 5.4f })
                Viga(new(lado * 2.3f, -1.4f, z), new(lado * 2.3f, 1.7f, z), 0.065f, casco);
            for (var folha = 0; folha < 2; folha++)
            {
                var z = 0.8f + folha * 0.7f;
                PainelVidro("PortaPanoramica", new(lado * 2.3f, -1.36f, z), new(lado * 2.3f, 1.7f, z),
                    new(lado * 2.3f, 1.7f, z + 0.7f), new(lado * 2.3f, -1.36f, z + 0.7f), vidro);
                Caixa("PuxadorPorta", new(lado * 2.24f, -0.28f, z + (folha == 0 ? 0.57f : 0.13f)), new(0.035f, 0.38f, 0.035f), acabamento);
            }
            Viga(new(lado * 2.3f, -1.4f, 1.5f), new(lado * 2.3f, 1.7f, 1.5f), 0.025f, juntas);
            Caixa("SoleiraEmbarque", new(lado * 2.1f, -1.36f, 1.5f), new(0.4f, 0.035f, 1.3f), luzQuente);
            for (var fileira = 0; fileira < 2; fileira++)
            {
                var z = 3.1f + fileira * 1.25f;
                Caixa("AssentoPassageiro", new(lado * 1.35f, -0.94f, z), new(0.75f, 0.16f, 0.65f), juntas);
                Caixa("EncostoPassageiro", new(lado * 1.35f, -0.46f, z + 0.3f), new(0.75f, 0.9f, 0.14f), casco);
                Caixa("BaseAssento", new(lado * 1.35f, -1.23f, z), new(0.1f, 0.45f, 0.4f), acabamento);
            }
        }
        Viga(new(-2.3f, -0.8f, -2.25f), new(2.3f, -0.8f, -2.25f), 0.07f, juntas);
        for (var indice = -1; indice <= 1; indice++)
        {
            var painel = new Node3D
            {
                Name = $"Monitor{indice + 1}", Position = new(indice * 1.02f, -0.68f, -1.53f),
                RotationDegrees = new(-18f, -indice * 10f, 0)
            };
            AddChild(painel);
            CaixaEm(painel, "Moldura", Vector3.Zero, new(0.89f, 0.40f, 0.07f), juntas);
            CaixaEm(painel, "Display", new(0, 0, 0.042f), new(0.81f, 0.32f, 0.012f), tela);
            painel.AddChild(new Label3D
            {
                Text = indice switch { -1 => "UFBA / ORION", 0 => "NAVEGACAO", _ => "SISTEMAS" },
                Position = new(0, 0.09f, 0.058f), FontSize = 28, PixelSize = 0.0014f,
                Modulate = new Color("9cead8"), OutlineSize = 0, NoDepthTest = false
            });
            for (var linha = 0; linha < 3; linha++)
            {
                var comprimento = 0.23f + ((linha + indice + 3) % 3) * 0.12f;
                CaixaEm(painel, "Indicador", new(-0.11f, 0.015f - linha * 0.055f, 0.058f), new(comprimento, 0.012f, 0.005f), luzFria);
                CaixaEm(painel, "Estado", new(0.3f, 0.015f - linha * 0.055f, 0.058f), new(0.025f, 0.017f, 0.006f), luzQuente);
            }
        }
        Caixa("BaseManche", new(0, -1.05f, -0.68f), new(0.32f, 0.2f, 0.34f), juntas);
        Viga(new(0, -1.02f, -0.68f), new(0, -0.74f, -0.62f), 0.055f, acabamento);
        Caixa("PunhoManche", new(0, -0.72f, -0.62f), new(0.35f, 0.06f, 0.065f), juntas);
        AddChild(new OmniLight3D
        {
            Name = "LuzInterna", Position = new(0, 0.6f, 0.45f),
            LightColor = new Color("adc7c2"), LightEnergy = 1.5f, OmniRange = 5f
        });
        AddChild(new OmniLight3D
        {
            Name = "ReflexoInstrumentos", Position = new(0, -0.35f, -1.2f),
            LightColor = new Color("46d9bd"), LightEnergy = 0.5f, OmniRange = 2.3f
        });
    }

    private void CriarCoberturaPanoramica(Material vidro, Material casco)
    {
        var fim = CabineCompacta ? 1.45f : 5.4f;
        // Uma faixa curva continua une o para-brisa ao teto, sem travessa sobre o piloto.
        var perfil = new Vector2[] { new(-2.25f, -0.8f), new(-2.25f, 0.65f), new(-2.05f, 1.23f),
            new(-1.55f, 1.7f), new(-0.85f, 1.95f), new(fim, 1.95f) };
        for (var i = 0; i < perfil.Length - 1; i++)
        {
            var a = perfil[i];
            var b = perfil[i + 1];
            PainelVidro("CoberturaPanoramica", new(-2.3f, a.Y, a.X), new(2.3f, a.Y, a.X),
                new(2.3f, b.Y, b.X), new(-2.3f, b.Y, b.X), vidro);
            foreach (var lado in new[] { -1f, 1f })
                Viga(new(lado * 2.3f, a.Y, a.X), new(lado * 2.3f, b.Y, b.X), 0.055f, casco);
        }
        foreach (var lado in new[] { -1f, 1f })
        {
            PainelVidro("OmbroDoTeto", new(lado * 2.3f, 1.7f, -0.85f), new(lado * 2.3f, 1.95f, -0.85f),
                new(lado * 2.3f, 1.95f, fim), new(lado * 2.3f, 1.7f, fim), vidro);
        }
    }

    private void PainelVidro(string nome, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Material material)
    {
        var superficie = new SurfaceTool();
        superficie.Begin(Mesh.PrimitiveType.Triangles);
        var normal = (b - a).Cross(c - a).Normalized();
        foreach (var ponto in new[] { a, b, c, a, c, d })
        {
            superficie.SetNormal(normal);
            superficie.AddVertex(ponto);
        }
        AddChild(new MeshInstance3D
        {
            Name = nome, Mesh = superficie.Commit(), MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
        }, true);
    }

    private static StandardMaterial3D Material(Color cor, float metalico, bool emissivo = false) => new()
    {
        AlbedoColor = cor, Metallic = metalico, Roughness = 0.52f,
        EmissionEnabled = emissivo, Emission = cor, EmissionEnergyMultiplier = 0.65f
    };

    private void Caixa(string nome, Vector3 posicao, Vector3 tamanho, Material material) => CaixaEm(this, nome, posicao, tamanho, material);

    private static void CaixaEm(Node3D pai, string nome, Vector3 posicao, Vector3 tamanho, Material material)
    {
        pai.AddChild(new MeshInstance3D
        {
            Name = nome, Position = posicao, Mesh = new BoxMesh { Size = tamanho }, MaterialOverride = material
        }, true);
    }

    private void Viga(Vector3 inicio, Vector3 fim, float largura, Material material)
    {
        var viga = new MeshInstance3D
        {
            Position = (inicio + fim) * 0.5f,
            Mesh = new BoxMesh { Size = new Vector3(largura, largura, inicio.DistanceTo(fim)) },
            MaterialOverride = material
        };
        AddChild(viga);
        var direcao = (fim - inicio).Normalized();
        viga.Basis = Basis.LookingAt(direcao, Mathf.Abs(direcao.Dot(Vector3.Up)) > 0.99f ? Vector3.Forward : Vector3.Up);
    }
}
