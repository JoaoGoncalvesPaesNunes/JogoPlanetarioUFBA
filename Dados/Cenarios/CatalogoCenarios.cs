using Godot;

namespace ProjetoPlanetario.Dados.Cenarios;

[GlobalClass]
public partial class CatalogoCenarios : Resource
{
    [Export] public Godot.Collections.Dictionary<string, string> Caminhos { get; set; } = new();
}
