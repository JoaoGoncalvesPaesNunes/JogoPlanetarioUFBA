using Godot;
using ProjetoPlanetario.Jogabilidade.Viagem;

namespace ProjetoPlanetario.Apresentacao.HUD;

/// <summary>Rastros em espaco 3D: a cabine permanece na frente do efeito.</summary>
public partial class ApresentacaoDoSalto : CanvasLayer
{
	private const int Quantidade = 320;
	private ControladorDoSalto _salto = null!;
	private Node3D _nave = null!;
	private Node3D _efeito = null!;
	private MultiMesh _rastros = null!;
	private StandardMaterial3D _materialFundo = null!;
	private readonly Vector3[] _sementes = new Vector3[Quantidade];
	private float _deslocamento;

	public void Configurar(ControladorDoSalto salto, Node3D nave)
	{
		_salto = salto;
		_nave = nave;
	}

	public override void _Ready()
	{
		_efeito = new Node3D { Name = "TunelDoSalto", Visible = false };
		_nave.AddChild(_efeito);
		_materialFundo = new StandardMaterial3D
		{
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
			CullMode = BaseMaterial3D.CullModeEnum.Front,
			RenderPriority = -10,
			AlbedoColor = new Color(0.002f, 0.004f, 0.009f, 0)
		};
		_efeito.AddChild(new MeshInstance3D
		{
			Name = "FundoDoTunel", Mesh = new SphereMesh { Radius = 4, Height = 8, RadialSegments = 32, Rings = 16 },
			MaterialOverride = _materialFundo, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
		});
		var material = new StandardMaterial3D
		{
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			VertexColorUseAsAlbedo = true,
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha
		};
		_rastros = new MultiMesh
		{
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D, UseColors = true,
			Mesh = new BoxMesh { Size = Vector3.One, Material = material }, InstanceCount = Quantidade
		};
		_efeito.AddChild(new MultiMeshInstance3D
		{
			Name = "RastrosEstelares", Multimesh = _rastros,
			CastShadow = GeometryInstance3D.ShadowCastingSetting.Off
		});
		var aleatorio = new RandomNumberGenerator { Seed = 9127 };
		for (var i = 0; i < Quantidade; i++)
		{
			var angulo = aleatorio.RandfRange(0, Mathf.Tau);
			var raio = aleatorio.RandfRange(2.5f, 34f);
			_sementes[i] = new Vector3(Mathf.Cos(angulo) * raio, Mathf.Sin(angulo) * raio, aleatorio.RandfRange(0, 58));
		}
	}

	public override void _Process(double delta)
	{
		var intensidade = _salto.Intensidade;
		_efeito.Visible = intensidade > 0.001f;
		if (!_efeito.Visible) return;
		_deslocamento = Mathf.PosMod(_deslocamento + (float)delta * Mathf.Lerp(3f, 46f, intensidade), 58f);
		_materialFundo.AlbedoColor = new Color(0.002f, 0.004f, 0.009f, Mathf.SmoothStep(0.35f, 0.95f, intensidade));
		var comprimento = Mathf.Lerp(0.045f, 9f, intensidade * intensidade);
		for (var i = 0; i < Quantidade; i++)
		{
			var origem = _sementes[i];
			var profundidade = Mathf.PosMod(origem.Z - _deslocamento, 58f);
			var posicao = new Vector3(origem.X, origem.Y, -8f - profundidade);
			var largura = 0.028f + (i % 4) * 0.008f;
			_rastros.SetInstanceTransform(i, new Transform3D(Basis.Identity.Scaled(new Vector3(largura, largura, comprimento)), posicao));
			var entrada = Mathf.SmoothStep(58f, 51f, profundidade);
			var saida = Mathf.SmoothStep(0f, 5f, profundidade);
			var cor = i % 5 == 0 ? new Color(0.65f, 0.59f, 0.43f) : new Color(0.32f, 0.61f, 0.68f);
			cor.A = intensidade * entrada * saida;
			_rastros.SetInstanceColor(i, cor);
		}
	}

	public void Ocultar()
	{
		_deslocamento = 0;
		if (_efeito is not null) _efeito.Visible = false;
	}

	public override void _ExitTree()
	{
		if (IsInstanceValid(_efeito)) _efeito.QueueFree();
	}
}
