using System;
using System.Collections.Generic;
using Godot;

namespace ProjetoPlanetario.Robusto.Eventos;

public partial class CombateDeMarte : Node3D
{
    [Export(PropertyHint.Range, "0.1,2")] public float IntervaloDisparo { get; set; } = 0.25f;
    [Export(PropertyHint.Range, "1,10")] public float RaioAcerto { get; set; } = 4;
    [Export(PropertyHint.Range, "1,8")] public int QuantidadePiratas { get; set; } = 3;
    [Signal] public delegate void EscudoSolicitadoEventHandler();
    [Signal] public delegate void EscudoAtivadoEventHandler();
    public bool Ativo { get; private set; }
    public bool Concluido => Ativo && _alvos.Count == 0;
    public int Restantes => _alvos.Count;
    public float Escudo { get; private set; }
    public int ImpactosBloqueados { get; private set; }
    public IReadOnlyList<Node3D> Alvos => _alvos;
    private readonly List<Node3D> _alvos = new();
    private readonly Dictionary<Node3D, Vector3> _origens = new();
    private float _tempo, _recarga, _tempoRastro, _proximoAtaque;
    private MeshInstance3D _rastro = null!;
    private Node3D? _destruindo;
    private Vector3 _referenciaNave;

    public override void _Ready()
    {
        _rastro = new MeshInstance3D { Visible = false, Mesh = new CylinderMesh { TopRadius = 0.025f, BottomRadius = 0.025f, Height = 1 },
            MaterialOverride = new StandardMaterial3D { ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded, AlbedoColor = new Color("f6ca74") } };
        AddChild(_rastro);
    }

    public void Iniciar(Node3D pontos, Vector3 origemNave)
    {
        Limpar();
        Ativo = true;
        _referenciaNave = origemNave;
        var modelo = GD.Load<PackedScene>("res://Robusto/Cenas/Pirata.tscn");
        foreach (var filho in pontos.GetChildren())
        {
            if (filho is not Marker3D ponto || _alvos.Count >= QuantidadePiratas) continue;
            var alvo = modelo.Instantiate<Node3D>();
            AddChild(alvo);
            alvo.GlobalTransform = ponto.GlobalTransform;
            _alvos.Add(alvo);
            _origens[alvo] = alvo.GlobalPosition;
        }
    }

    public void Atualizar(float delta, Vector3 origemNave)
    {
        _tempo += delta;
        _recarga = Mathf.Max(0, _recarga - delta);
        Escudo = Mathf.Max(0, Escudo - delta);
        _tempoRastro -= delta;
        _rastro.Visible = _tempoRastro > 0;
        if (_destruindo is not null && IsInstanceValid(_destruindo))
        {
            _destruindo.Scale *= Mathf.Exp(-delta * 12);
            if (_destruindo.Scale.Length() < 0.1f) { _destruindo.QueueFree(); _destruindo = null; }
        }
        for (var i = 0; i < _alvos.Count; i++)
            _alvos[i].GlobalPosition = _origens[_alvos[i]] + origemNave - _referenciaNave + new Vector3(Mathf.Sin(_tempo * 0.45f + i) * 3, Mathf.Cos(_tempo * 0.5f + i) * 1.5f, 0);
        if (!Ativo || Restantes == 0 || _tempo < _proximoAtaque || _recarga > 0) return;
        _proximoAtaque = _tempo + 5;
        // Ataque cenico: sem dano acumulativo ou bloqueio permanente da sessao.
        MostrarRastro(_alvos[0].GlobalPosition, origemNave + new Vector3(6, 2, -12));
        if (Escudo > 0) ImpactosBloqueados++;
    }

    public bool Disparar(Vector3 origem, Vector3 direcao)
    {
        if (!Ativo || Concluido || _recarga > 0) return false;
        _recarga = IntervaloDisparo;
        Node3D? atingido = null;
        var distancia = 200f;
        direcao = direcao.Normalized();
        foreach (var alvo in _alvos)
        {
            var vetor = alvo.GlobalPosition - origem;
            var projecao = vetor.Dot(direcao);
            if (projecao <= 0 || projecao >= distancia || (vetor - direcao * projecao).Length() > RaioAcerto) continue;
            atingido = alvo;
            distancia = projecao;
        }
        MostrarRastro(origem + new Vector3(0.4f, -0.3f, -1), origem + direcao * distancia);
        if (atingido is null) return false;
        if (_destruindo is not null && IsInstanceValid(_destruindo)) _destruindo.QueueFree();
        _destruindo = atingido;
        _alvos.Remove(atingido);
        _origens.Remove(atingido);
        return true;
    }

    public void AtivarEscudo()
    {
        if (!Ativo || Concluido) return;
        EmitSignal(SignalName.EscudoSolicitado);
        Escudo = 3;
        EmitSignal(SignalName.EscudoAtivado);
    }

    private void MostrarRastro(Vector3 inicio, Vector3 fim)
    {
        var direcao = fim - inicio;
        if (direcao.LengthSquared() < 0.01f) return;
        var eixoY = direcao.Normalized();
        var eixoX = eixoY.Cross(Vector3.Right).Normalized();
        if (eixoX.LengthSquared() < 0.1f) eixoX = Vector3.Forward;
        _rastro.GlobalTransform = new Transform3D(new Basis(eixoX, eixoY * direcao.Length(), eixoX.Cross(eixoY)), (inicio + fim) / 2);
        _tempoRastro = 0.12f;
    }

    public void Limpar()
    {
        foreach (var alvo in _alvos) alvo.QueueFree();
        _alvos.Clear(); _origens.Clear();
        if (_destruindo is not null && IsInstanceValid(_destruindo)) _destruindo.QueueFree();
        _destruindo = null;
        Ativo = false; Escudo = 0; _tempo = 0; _recarga = 0; _tempoRastro = 0; _proximoAtaque = 5;
    }
}
