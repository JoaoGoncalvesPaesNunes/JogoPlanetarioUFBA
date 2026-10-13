using Godot;
using ProjetoPlanetario.Robusto.Cenarios;
using ProjetoPlanetario.Robusto.Dominio;

namespace ProjetoPlanetario.Robusto.Eventos;

public partial class EventoDoGelo : Node
{
    public ManobraDoGelo? Regra { get; private set; }
    public float Desvio { get; private set; }
    public bool Concluido => Regra?.Fase == FaseDoGelo.Concluido;
    private Node3D? _objeto;
    private Vector3 _inicio, _alerta, _fim;
    private float _distanciaEvasao;
    public void Iniciar(CenarioDaViagem cenario)
    {
        Limpar();
        _objeto = cenario.GetNode<Node3D>("Mundo/Objetos/Gelo");
        _inicio = _objeto.GlobalPosition;
        _alerta = cenario.GetNode<Marker3D>("Jogabilidade/Gatilhos/AlertaGelo").GlobalPosition;
        _fim = cenario.GetNode<Marker3D>("Jogabilidade/Gatilhos/FimGelo").GlobalPosition;
        _distanciaEvasao = cenario.DistanciaEvasao;
        Regra = new ManobraDoGelo(cenario.TempoAproximacaoGelo, cenario.TempoCorrecaoGelo, cenario.DuracaoEvasao);
        _objeto.Visible = true;
    }
    public void Atualizar(float delta, float horizontal)
    {
        if (Regra is null || _objeto is null) return;
        Regra.Atualizar(delta, horizontal);
        switch (Regra.Fase)
        {
            case FaseDoGelo.Aproximacao:
                _objeto.GlobalPosition = _inicio.Lerp(_alerta, Mathf.SmoothStep(0, 1, Regra.Tempo / Regra.Aproximacao)); break;
            case FaseDoGelo.Alerta: _objeto.GlobalPosition = _alerta; break;
            case FaseDoGelo.Evasao:
                var t = Mathf.Clamp(Regra.Tempo / Regra.DuracaoEvasao, 0, 1);
                var suave = Mathf.SmoothStep(0, 1, t);
                _objeto.GlobalPosition = _alerta.Lerp(_fim, suave);
                Desvio = Regra.Lado * _distanciaEvasao * Mathf.Sin(Mathf.Pi * suave);
                break;
            case FaseDoGelo.Concluido: Desvio = 0; _objeto.Visible = false; break;
        }
    }
    public void Limpar()
    {
        if (_objeto is not null && IsInstanceValid(_objeto)) _objeto.Visible = false;
        _objeto = null; Regra = null; Desvio = 0;
    }
}
