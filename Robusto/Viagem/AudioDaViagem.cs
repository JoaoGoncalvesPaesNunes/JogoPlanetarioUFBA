using Godot;

namespace ProjetoPlanetario.Robusto.Viagem;

public partial class AudioDaViagem : Node
{
    private AudioStreamPlayer _narracao = null!;
    public override void _Ready()
    {
        _narracao = new AudioStreamPlayer { Name = "Narracao", VolumeDb = -6 };
        AddChild(_narracao);
    }
    public void Reproduzir(string caminho)
    {
        _narracao.Stop();
        if (string.IsNullOrWhiteSpace(caminho)) return;
        if (!ResourceLoader.Exists(caminho)) { GD.PushWarning("Audio ausente: " + caminho); return; }
        _narracao.Stream = GD.Load<AudioStream>(caminho);
        _narracao.Play();
    }
}
