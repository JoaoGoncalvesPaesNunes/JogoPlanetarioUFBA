using Godot;

namespace ProjetoPlanetario.Robusto.Projecao;

[GlobalClass]
public partial class ConfiguracaoExibicao : Resource
{
    [Export] public bool FulldomePreview { get; set; }
    [Export] public bool TelaCheia { get; set; } = true;
    [Export(PropertyHint.Range, "512,4096,512")] public int ResolucaoSaida { get; set; } = 1024;
    [Export(PropertyHint.Range, "90,180,1")] public float FisheyeFov { get; set; } = 180;
    [Export] public Vector3 RotacaoCameraGraus { get; set; } = Vector3.Zero;
    [Export(PropertyHint.Range, "-180,180")] public float OrientacaoDomo { get; set; }
    [Export(PropertyHint.Range, "0.5,1.5")] public float Escala { get; set; } = 1;
    [Export(PropertyHint.Range, "0.5,1")] public float Recorte { get; set; } = 1;
    [Export(PropertyHint.Range, "0.4,0.9")] public float RaioSeguroUI { get; set; } = 0.72f;
    [Export] public bool EspelharHorizontal { get; set; }
    [Export] public bool EspelharVertical { get; set; }
    // Reservado para adaptador de hardware documentado. Nao aplica deformacao ficticia.
    [Export] public bool WarpingHabilitado { get; set; }
}
