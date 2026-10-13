using Godot;
using ProjetoPlanetario.Robusto.Dominio;

namespace ProjetoPlanetario.Robusto.Viagem;

public partial class ControladorDoOlhar : Node3D
{
    [Export(PropertyHint.Range, "0.01,0.5,0.01")] public float SensibilidadeMouse { get; set; } = 0.08f;
    [Export(PropertyHint.Range, "1,15")] public float Suavizacao { get; set; } = 7;
    [Export(PropertyHint.Range, "10,60")] public float VelocidadeAnalogico { get; set; } = 30;
    [Export(PropertyHint.Range, "10,60")] public float VelocidadeMaxima { get; set; } = 40;
    [Export(PropertyHint.Range, "45,120")] public float LimiteYaw { get; set; } = 115;
    [Export(PropertyHint.Range, "-60,0")] public float PitchMinimo { get; set; } = -50;
    [Export(PropertyHint.Range, "0,75")] public float PitchMaximo { get; set; } = 70;
    [Export] public bool InverterY { get; set; }
    [Export] public bool RecentralizarAutomaticamente { get; set; }
    [Export(PropertyHint.Range, "2,30")] public float EsperaRecentralizacao { get; set; } = 8;
    [Export(PropertyHint.Range, "10,50")] public float VelocidadeRecentralizacao { get; set; } = 32;
    public ContextoDeEntrada Contexto { get; private set; } = ContextoDeEntrada.Bloqueado;
    public EstadoDoOlhar Estado { get; } = new();
    private float _inatividade;

    public void DefinirContexto(ContextoDeEntrada contexto)
    {
        if (Contexto == contexto) return;
        Contexto = contexto;
        _inatividade = 0;
        if (contexto is ContextoDeEntrada.Combate or ContextoDeEntrada.Gelo or ContextoDeEntrada.Bloqueado) Recentralizar();
        Input.MouseMode = contexto == ContextoDeEntrada.Olhar ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
    }
    public void ReceberMouse(Vector2 deslocamento)
    {
        if (Contexto != ContextoDeEntrada.Olhar || deslocamento.IsZeroApprox()) return;
        ConfigurarLimites();
        Estado.Mover(deslocamento.X * SensibilidadeMouse, deslocamento.Y * SensibilidadeMouse * (InverterY ? 1 : -1));
        _inatividade = 0;
    }
    public void Recentralizar() { Estado.Recentralizar(); _inatividade = 0; }
    public void Atualizar(float delta, Vector2 eixo)
    {
        ConfigurarLimites();
        if (Contexto == ContextoDeEntrada.Menu) return;
        if (Contexto == ContextoDeEntrada.Olhar)
        {
            _inatividade += delta;
            if (eixo.LengthSquared() > 0.001f)
            {
                Estado.Mover(eixo.X * VelocidadeAnalogico * delta, -eixo.Y * VelocidadeAnalogico * delta * (InverterY ? -1 : 1));
                _inatividade = 0;
            }
            if (RecentralizarAutomaticamente && _inatividade >= EsperaRecentralizacao) Estado.Recentralizar();
        }
        Estado.Atualizar(delta);
        // Yaw positivo significa olhar para a direita; nenhuma escrita no transform do pai.
        RotationDegrees = new Vector3(Estado.Pitch, -Estado.Yaw, 0);
    }
    private void ConfigurarLimites()
    {
        Estado.LimiteYaw = Mathf.Clamp(LimiteYaw, 45, 120);
        Estado.PitchMinimo = Mathf.Clamp(PitchMinimo, -60, 0);
        Estado.PitchMaximo = Mathf.Clamp(PitchMaximo, 0, 75);
        Estado.Suavizacao = Suavizacao;
        Estado.VelocidadeMaxima = VelocidadeMaxima;
        Estado.VelocidadeRecentralizacao = VelocidadeRecentralizacao;
    }
    public override void _ExitTree() { Input.MouseMode = Input.MouseModeEnum.Visible; }
}
