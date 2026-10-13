using Godot;
using ProjetoPlanetario.Projecao.Portas;

namespace ProjetoPlanetario.Robusto.Projecao;

/// <summary>Cinco faces de um unico rig optico. Gameplay recebe somente raios do mundo.</summary>
public partial class GerenciadorExibicao : CanvasLayer, IPortaDeProjecao
{
    [Export] public ConfiguracaoExibicao Configuracao { get; set; } = null!;
    public bool Fulldome { get; private set; }
    public bool Calibracao { get => _grade?.Visible ?? false; set { if (_grade is not null) _grade.Visible = value; } }
    public SubViewport SaidaDomo { get; private set; } = null!;
    private readonly SubViewport[] _faces = new SubViewport[5];
    private readonly Camera3D[] _cameras = new Camera3D[5];
    private readonly Vector3[] _direcoes = { Vector3.Forward, Vector3.Right, Vector3.Left, Vector3.Up, Vector3.Down };
    private Camera3D _camera = null!;
    private TextureRect _saida = null!;
    private GradeCalibracao _grade = null!;
    private ShaderMaterial _material = null!;
    public void Configurar(Camera3D camera) { _camera = camera; }
    public override void _Ready()
    {
        Layer = 5;
        Configuracao ??= new ConfiguracaoExibicao();
        AplicarEmViewport(GetViewport());
        _material = new ShaderMaterial { Shader = GD.Load<Shader>("res://Robusto/Projecao/Fisheye.gdshader") };
        var nomes = new[] { "frente", "direita", "esquerda", "cima", "baixo" };
        for (var i = 0; i < 5; i++)
        {
            var face = new SubViewport { Name = "Face_" + nomes[i], World3D = _camera.GetWorld3D(), Size = new Vector2I(512, 512),
                RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled };
            AddChild(face);
            _faces[i] = face;
            var cam = new Camera3D { Fov = 90, Near = _camera.Near, Far = _camera.Far, Current = true };
            face.AddChild(cam);
            _cameras[i] = cam;
            _material.SetShaderParameter(nomes[i], face.GetTexture());
        }
        SaidaDomo = new SubViewport { Name = "SaidaDomeMaster", Disable3D = true, Size = new Vector2I(1024, 1024), RenderTargetUpdateMode = SubViewport.UpdateMode.Disabled };
        AddChild(SaidaDomo);
        var imagem = new ColorRect { Material = _material, MouseFilter = Control.MouseFilterEnum.Ignore };
        SaidaDomo.AddChild(imagem);
        imagem.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _grade = new GradeCalibracao { Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
        SaidaDomo.AddChild(_grade);
        _grade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _saida = new TextureRect { Texture = SaidaDomo.GetTexture(), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered, MouseFilter = Control.MouseFilterEnum.Ignore };
        AddChild(_saida);
        _saida.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        AplicarConfiguracao();
        DefinirModo(Configuracao.FulldomePreview);
    }
    public void AplicarConfiguracao()
    {
        var tamanho = Mathf.Clamp(Configuracao.ResolucaoSaida, 512, 4096);
        SaidaDomo.Size = new Vector2I(tamanho, tamanho);
        foreach (var face in _faces) face.Size = new Vector2I(tamanho / 2, tamanho / 2);
        _material.SetShaderParameter("fov", Mathf.DegToRad(Mathf.Clamp(Configuracao.FisheyeFov, 90, 180)));
        _material.SetShaderParameter("escala", Mathf.Clamp(Configuracao.Escala, 0.5f, 1.5f));
        _material.SetShaderParameter("recorte", Mathf.Clamp(Configuracao.Recorte, 0.5f, 1));
        _material.SetShaderParameter("orientacao", Mathf.DegToRad(Configuracao.OrientacaoDomo));
        _material.SetShaderParameter("espelho", new Vector2(Configuracao.EspelharHorizontal ? -1 : 1, Configuracao.EspelharVertical ? -1 : 1));
        _grade.CampoDeVisao = Mathf.Clamp(Configuracao.FisheyeFov, 90, 180);
        _grade.QueueRedraw();
        if (Configuracao.WarpingHabilitado) GD.PushWarning("Warping nao disponivel: requer especificacao do equipamento.");
    }
    public void DefinirModo(bool fulldome)
    {
        Fulldome = fulldome;
        _saida.Visible = fulldome;
        GetViewport().Disable3D = fulldome;
        foreach (var face in _faces) face.RenderTargetUpdateMode = fulldome ? SubViewport.UpdateMode.Always : SubViewport.UpdateMode.Disabled;
        SaidaDomo.RenderTargetUpdateMode = fulldome ? SubViewport.UpdateMode.Always : SubViewport.UpdateMode.Disabled;
    }
    private Basis OrientacaoOptica => _camera.GlobalBasis * Basis.FromEuler(Configuracao.RotacaoCameraGraus * (Mathf.Pi / 180));
    public override void _Process(double delta)
    {
        if (!Fulldome) return;
        for (var i = 0; i < 5; i++)
            _cameras[i].GlobalTransform = new Transform3D(OrientacaoOptica * Basis.LookingAt(_direcoes[i], i >= 3 ? (i == 3 ? Vector3.Back : Vector3.Forward) : Vector3.Up), _camera.GlobalPosition);
    }
    public Vector3 DirecaoNaTela(Vector2 tela)
    {
        if (!Fulldome) return _camera.ProjectRayNormal(tela);
        var tamanho = GetViewport().GetVisibleRect().Size;
        var p = (tela - tamanho / 2) / (Mathf.Min(tamanho.X, tamanho.Y) / 2);
        p *= new Vector2(Configuracao.EspelharHorizontal ? -1 : 1, Configuracao.EspelharVertical ? -1 : 1);
        p = (p / Mathf.Clamp(Configuracao.Escala, 0.5f, 1.5f)).Rotated(Mathf.DegToRad(Configuracao.OrientacaoDomo));
        var r = p.Length();
        if (r > Mathf.Clamp(Configuracao.Recorte, 0.5f, 1)) return Vector3.Zero;
        var t = r * Mathf.DegToRad(Mathf.Clamp(Configuracao.FisheyeFov, 90, 180)) / 2;
        return OrientacaoOptica * new Vector3(p.X * Mathf.Sin(t) / Mathf.Max(r, 0.00001f), -p.Y * Mathf.Sin(t) / Mathf.Max(r, 0.00001f), -Mathf.Cos(t));
    }
    public Rect2 ObterAreaLogica()
    {
        var tamanho = GetViewport().GetVisibleRect().Size;
        if (!Fulldome) return new Rect2(tamanho * 0.045f, tamanho * 0.91f);
        var lado = Mathf.Min(tamanho.X, tamanho.Y) * Mathf.Clamp(Configuracao.RaioSeguroUI, 0.4f, 0.9f) / Mathf.Sqrt(2);
        return new Rect2((tamanho - Vector2.One * lado) / 2, Vector2.One * lado);
    }
    public Vector2 ProjetarPonto(Vector3 ponto)
    {
        if (!Fulldome) return _camera.UnprojectPosition(ponto);
        var d = OrientacaoOptica.Inverse() * (ponto - _camera.GlobalPosition).Normalized();
        var angulo = Mathf.Acos(Mathf.Clamp(-d.Z, -1, 1));
        var r = angulo / (Mathf.DegToRad(Mathf.Clamp(Configuracao.FisheyeFov, 90, 180)) / 2);
        var p = new Vector2(d.X, -d.Y).Normalized() * r;
        p = p.Rotated(-Mathf.DegToRad(Configuracao.OrientacaoDomo)) * Mathf.Clamp(Configuracao.Escala, 0.5f, 1.5f);
        p *= new Vector2(Configuracao.EspelharHorizontal ? -1 : 1, Configuracao.EspelharVertical ? -1 : 1);
        var tamanho = GetViewport().GetVisibleRect().Size;
        return tamanho / 2 + p * Mathf.Min(tamanho.X, tamanho.Y) / 2;
    }
    public void AplicarEmViewport(Viewport viewport)
    {
        if (viewport is not Window janela) return;
        janela.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        janela.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;
    }
    public override void _ExitTree() { if (IsInstanceValid(_camera)) GetViewport().Disable3D = false; }
}
