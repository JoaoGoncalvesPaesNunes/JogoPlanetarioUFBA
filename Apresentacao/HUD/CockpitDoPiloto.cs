using System;
using Godot;
using ProjetoPlanetario.Jogabilidade.Eventos;
using ProjetoPlanetario.Jogabilidade.Voo;
using ProjetoPlanetario.Narrativa.Diretor;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Apresentacao.HUD;

public partial class CockpitDoPiloto : Control
{
	public event Action? EstacaoSolicitada;
	public event Action<string>? DestinoSolicitado;
	private PanelContainer _escolha = null!;
	private Label _tituloEscolha = null!;
	private ControladorDeVoo? _voo;
	private EstadoDaNave? _nave;
	private ControladorDoLoopCentral? _loop;
	private DiretorNarrativo? _orion;
	private BarramentoDeEventos? _eventos;
	private Label _objetivo = null!;
	private Label _narrativa = null!;
	private Label _destino = null!;
	private Button _estacao = null!;
	private Button _reiniciar = null!;
	private readonly Label[] _instrumentos = new Label[4];
	private float _progresso;
	private string _textoObjetivo = "Avance pela rota de teste.";
	private string _rota = "SETOR 01 / TRAVESSIA";
	private bool _concluido;
	private readonly Color _verde = new("80dfc9");
	private readonly Color _ambar = new("eac082");

	public void Configurar(ControladorDeVoo controladorDeVoo, EstadoDaNave estadoDaNave,
		ControladorDoLoopCentral? controladorDoLoopCentral = null, DiretorNarrativo? diretorNarrativo = null,
		BarramentoDeEventos? barramentoDeEventos = null)
	{
		_voo = controladorDeVoo;
		_nave = estadoDaNave;
		_loop = controladorDoLoopCentral;
		_orion = diretorNarrativo;
		_eventos = barramentoDeEventos;
		if (_eventos is not null) _eventos.EventoPublicado += AoEvento;
	}

	public void DefinirRotaAtual(string rotaAtual) => _rota = rotaAtual;

	public override void _Ready()
	{
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
		MouseFilter = MouseFilterEnum.Ignore;
		_destino = Texto(14, _verde);
		_objetivo = Texto(20, new Color("eff3ef"));
		_narrativa = Texto(15, new Color("c2ceca"));
		for (var indice = 0; indice < _instrumentos.Length; indice++)
			_instrumentos[indice] = Texto(16, indice == 1 ? _ambar : _verde);
		_estacao = Botao("ESTACAO", "Abrir ou fechar estacao de passageiro (Tab)");
		_estacao.Pressed += () => EstacaoSolicitada?.Invoke();
		_reiniciar = Botao("VOLTAR A ROTA INICIAL", "Voltar ao inicio e escolher outro destino (Enter)");
		_reiniciar.Name = "VoltarRotaInicial";
		_reiniciar.Visible = false;
		CriarEscolhaDestino();
		_reiniciar.Pressed += () => _eventos?.Publicar(new EstacaoAcaoExecutada("Estacao1", "Confirmar", DateTime.UtcNow));
		Resized += Organizar;
		Organizar();
	}

	private void CriarEscolhaDestino()
	{
		_escolha = new PanelContainer { Name = "EscolhaDestino", Visible = false };
		_escolha.AddThemeStyleboxOverride("panel", new StyleBoxFlat
		{
			BgColor = new Color("10221f"), ContentMarginLeft = 24, ContentMarginRight = 24,
			ContentMarginTop = 20, ContentMarginBottom = 20, BorderWidthTop = 2, BorderColor = _verde
		});
		AddChild(_escolha);
		var coluna = new VBoxContainer();
		coluna.AddThemeConstantOverride("separation", 16);
		_escolha.AddChild(coluna);
		_tituloEscolha = new Label { Text = "ESCOLHA O DESTINO", HorizontalAlignment = HorizontalAlignment.Center };
		coluna.AddChild(_tituloEscolha);
		var opcoes = new HBoxContainer();
		opcoes.AddThemeConstantOverride("separation", 12);
		coluna.AddChild(opcoes);
		foreach (var id in new[] { "marte", "jupiter" })
		{
			var botao = Botao(id.ToUpperInvariant(), id == "marte" ? "Confirmar Marte (1 / Acao 1)" : "Confirmar Jupiter (2 / Acao 2)");
			botao.Name = id;
			botao.Reparent(opcoes);
			botao.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			botao.CustomMinimumSize = new Vector2(160, 48);
			botao.Pressed += () => DestinoSolicitado?.Invoke(id);
		}
	}

	public void MostrarEscolhaDestino(bool visivel, string titulo = "ESCOLHA O DESTINO")
	{
		_tituloEscolha.Text = titulo;
		_escolha.Visible = visivel;
	}

	public override void _ExitTree()
	{
		if (_eventos is not null) _eventos.EventoPublicado -= AoEvento;
	}

	public override void _Process(double delta)
	{
		if (_voo is null || _nave is null) return;
		_destino.Text = $"UFBA / ORION     {_rota}";
		_objetivo.Text = _loop?.ObjetivoAtual ?? _textoObjetivo;
		_narrativa.Text = _orion?.UltimaLinha ?? "ORION: Sistemas nominais.";
		_instrumentos[0].Text = $"ENERGIA\n{_nave.Energia:0}%";
		_instrumentos[1].Text = $"PROPULSAO\n{_voo.VelocidadeFrontalAtual * 100f:+0;-0;0}%";
		_instrumentos[2].Text = $"CASCO\n{_nave.Integridade:0}%";
		_instrumentos[3].Text = $"TRAVESSIA\n{_progresso * 100f:0}%";
		_reiniciar.Visible = _concluido;
		QueueRedraw();
	}

	private void Organizar()
	{
		if (_objetivo is null) return;
		_destino.Position = new(28, 20);
		_destino.Size = new(Mathf.Max(200, Size.X - 235), 22);
		_objetivo.Position = new(28, 49);
		_objetivo.Size = new(Mathf.Max(200, Size.X - 235), 60);
		_estacao.Position = new(Size.X - 155, 22);
		_estacao.Size = new(127, 36);
		_escolha.Position = new(Size.X * 0.5f - 232, Size.Y * 0.42f - 60);
		_escolha.Size = new(464, 134);
		_reiniciar.Position = new(Size.X - 292, Size.Y - 202);
		_reiniciar.Size = new(264, 42);
		_narrativa.Position = new(28, Size.Y - 139);
		_narrativa.Size = new(Size.X - 56, 45);
		var largura = (Size.X - 56) / 4f;
		for (var indice = 0; indice < _instrumentos.Length; indice++)
		{
			_instrumentos[indice].Position = new(28 + largura * indice, Size.Y - 82);
			_instrumentos[indice].Size = new(largura - 18, 51);
		}
	}

	public override void _Draw()
	{
		if (_voo is null || _nave is null) return;
		DrawRect(new Rect2(0, 0, Size.X, 112), new Color(0.015f, 0.025f, 0.03f, 0.62f));
		DrawRect(new Rect2(0, Size.Y - 146, Size.X, 146), new Color(0.025f, 0.038f, 0.043f, 0.9f));
		DrawLine(new(28, Size.Y - 94), new(Size.X - 28, Size.Y - 94), new Color(0.5f, 0.7f, 0.65f, 0.28f));
		var valores = new[] { _nave.Energia / 100f, Mathf.Abs(_voo.VelocidadeFrontalAtual), _nave.Integridade / 100f, _progresso };
		var largura = (Size.X - 56) / 4f;
		for (var indice = 0; indice < 4; indice++)
		{
			var barra = new Rect2(28 + largura * indice, Size.Y - 21, largura - 26, 3);
			DrawRect(barra, new Color(0.2f, 0.28f, 0.27f));
			barra.Size = new(barra.Size.X * Mathf.Clamp(valores[indice], 0, 1), 3);
			DrawRect(barra, indice == 1 ? _ambar : _verde);
		}
		var centro = Size * 0.5f;
		var cor = new Color(0.7f, 0.9f, 0.85f, 0.55f);
		DrawArc(centro, 10, 0, Mathf.Tau, 32, cor, 1, true);
		DrawLine(centro + new Vector2(-23, 0), centro + new Vector2(-14, 0), cor, 1, true);
		DrawLine(centro + new Vector2(14, 0), centro + new Vector2(23, 0), cor, 1, true);
		DrawCircle(centro, 1.5f, cor);
	}

	private Label Texto(int fonte, Color cor)
	{
		var texto = new Label { MouseFilter = MouseFilterEnum.Ignore, AutowrapMode = TextServer.AutowrapMode.WordSmart };
		texto.AddThemeFontSizeOverride("font_size", fonte);
		texto.AddThemeColorOverride("font_color", cor);
		AddChild(texto);
		return texto;
	}

	private Button Botao(string texto, string dica)
	{
		var botao = new Button { Text = texto, TooltipText = dica, FocusMode = FocusModeEnum.None };
		foreach (var estado in new[] { "normal", "hover", "pressed" })
			botao.AddThemeStyleboxOverride(estado, new StyleBoxFlat
			{
				BgColor = estado == "normal" ? new Color("243632") : new Color("3a5850"),
				BorderColor = new Color("567c70"), BorderWidthBottom = 1,
				ContentMarginLeft = 12, ContentMarginRight = 12
			});
		botao.AddThemeFontSizeOverride("font_size", 13);
		AddChild(botao);
		return botao;
	}

	private void AoEvento(EventoDeDominio evento)
	{
		switch (evento)
		{
			case ObjetivoAlterado objetivo: _textoObjetivo = objetivo.TextoDoObjetivo; break;
			case ProgressoNaRotaAlterado progresso: _progresso = progresso.ProgressoAtual; break;
			case SegmentoDeViagemConcluido: _concluido = true; _progresso = 1f; break;
			case SegmentoDeViagemReiniciado: _concluido = false; _progresso = 0f; break;
		}
	}
}
