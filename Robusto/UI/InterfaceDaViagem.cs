using System;
using Godot;
using ProjetoPlanetario.Robusto.Projecao;

namespace ProjetoPlanetario.Robusto.UI;

public partial class InterfaceDaViagem : Control
{
    public event Action? ReiniciarSolicitado;
    public event Action? ContinuarSolicitado;
    public event Action? SairSolicitado;
    public event Action<bool>? MenuAlterado;
    public bool MenuAberto => _menu.Visible;
    public Vector2 Mira { get; set; }
    public bool MiraVisivel { get; set; }
    public bool ExibirDebug { get; set; }
    public string InformacaoDebug { get; set; } = "";
    private GerenciadorExibicao _exibicao = null!;
    private Control _area = null!;
    private Label _titulo = null!, _narracao = null!, _estado = null!, _debug = null!;
    private VBoxContainer _rodape = null!;
    private ProgressBar _barra = null!;
    private Button _reiniciar = null!;
    private PanelContainer _menu = null!;
    public void Configurar(GerenciadorExibicao exibicao) { _exibicao = exibicao; }
    public override void _Ready()
    {
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        MouseFilter = MouseFilterEnum.Ignore;
        _area = new Control { MouseFilter = MouseFilterEnum.Ignore };
        AddChild(_area);
        _titulo = Texto(22, new Color("9ae1cd"));
        _area.AddChild(_titulo);
        _titulo.SetAnchorsAndOffsetsPreset(LayoutPreset.TopWide);
        _rodape = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore, Alignment = BoxContainer.AlignmentMode.End };
        _area.AddChild(_rodape);
        _rodape.SetAnchorsAndOffsetsPreset(LayoutPreset.BottomWide);
        _rodape.AnchorTop = 0.72f;
        _rodape.GrowVertical = GrowDirection.Begin;
        _rodape.AddThemeConstantOverride("separation", 10);
        _estado = Texto(18, new Color("f4ca7f"));
        _rodape.AddChild(_estado);
        _barra = new ProgressBar { MaxValue = 1, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 8), MouseFilter = MouseFilterEnum.Ignore };
        _rodape.AddChild(_barra);
        _narracao = Texto(19, Colors.White);
        _rodape.AddChild(_narracao);
        _reiniciar = new Button { Text = "REINICIAR VIAGEM", Name = "ReiniciarViagem", CustomMinimumSize = new Vector2(0, 42), Visible = false };
        _reiniciar.Pressed += () => ReiniciarSolicitado?.Invoke();
        _rodape.AddChild(_reiniciar);
        _debug = Texto(14, Colors.LightGreen);
        AddChild(_debug);
        _debug.Position = new Vector2(16, 16);
        CriarMenu();
        Mira = GetViewportRect().Size / 2;
    }
    private static Label Texto(int tamanho, Color cor)
    {
        var rotulo = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, MouseFilter = MouseFilterEnum.Ignore };
        rotulo.AddThemeFontSizeOverride("font_size", tamanho);
        rotulo.AddThemeColorOverride("font_color", cor);
        rotulo.AddThemeColorOverride("font_shadow_color", Colors.Black);
        rotulo.AddThemeConstantOverride("shadow_offset_x", 1);
        rotulo.AddThemeConstantOverride("shadow_offset_y", 2);
        return rotulo;
    }
    private void CriarMenu()
    {
        _menu = new PanelContainer { Visible = false, CustomMinimumSize = new Vector2(360, 380) };
        AddChild(_menu);
        _menu.SetAnchorsAndOffsetsPreset(LayoutPreset.Center);
        _menu.GrowHorizontal = GrowDirection.Both;
        _menu.GrowVertical = GrowDirection.Both;
        var fundo = new StyleBoxFlat { BgColor = new Color("101b1d"), ContentMarginLeft = 24, ContentMarginRight = 24, ContentMarginTop = 22, ContentMarginBottom = 22 };
        _menu.AddThemeStyleboxOverride("panel", fundo);
        var coluna = new VBoxContainer();
        coluna.AddThemeConstantOverride("separation", 12);
        _menu.AddChild(coluna);
        coluna.AddChild(new Label { Text = "EXIBICAO" });
        var modo = new OptionButton();
        modo.AddItem("Tela normal"); modo.AddItem("Fulldome Preview (experimental)");
        modo.Select(_exibicao.Fulldome ? 1 : 0);
        modo.ItemSelected += indice => _exibicao.DefinirModo(indice == 1);
        coluna.AddChild(modo);
        var resolucao = new OptionButton();
        foreach (var valor in new[] { 1024, 2048, 4096 }) resolucao.AddItem($"{valor} x {valor}", valor);
        resolucao.ItemSelected += indice => { _exibicao.Configuracao.ResolucaoSaida = resolucao.GetItemId((int)indice); _exibicao.AplicarConfiguracao(); };
        coluna.AddChild(resolucao);
        var cheia = new CheckButton { Text = "Tela cheia", ButtonPressed = GetWindow().Mode == Window.ModeEnum.Fullscreen };
        cheia.Toggled += ativo => GetWindow().Mode = ativo ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
        coluna.AddChild(cheia);
        var grade = new CheckButton { Text = "Calibracao do domo" };
        grade.Toggled += ativo => { _exibicao.Calibracao = ativo; if (ativo) { _exibicao.DefinirModo(true); modo.Select(1); } };
        coluna.AddChild(grade);
        var continuar = new Button { Text = "CONTINUAR", CustomMinimumSize = new Vector2(0, 40) };
        continuar.Pressed += () => ContinuarSolicitado?.Invoke();
        coluna.AddChild(continuar);
        var sair = new Button { Text = "SAIR" };
        sair.Pressed += () => SairSolicitado?.Invoke();
        coluna.AddChild(sair);
    }
    public void MostrarMenu(bool mostrar) { _menu.Visible = mostrar; MenuAlterado?.Invoke(mostrar); }
    public void Apresentar(string titulo, string narracao, string estado, float progresso, bool contemplativo, bool chegada)
    {
        _titulo.Text = titulo;
        _titulo.Visible = !contemplativo;
        _narracao.Text = narracao;
        _estado.Text = estado;
        _estado.Visible = estado.Length > 0 && !contemplativo;
        _barra.Visible = progresso >= 0 && !contemplativo;
        _barra.Value = progresso;
        _reiniciar.Visible = chegada;
    }
    public override void _Process(double delta)
    {
        var area = _exibicao.ObterAreaLogica();
        _area.Position = area.Position;
        _area.Size = area.Size;
        _debug.Visible = ExibirDebug;
        _debug.Text = InformacaoDebug;
        _debug.Size = new Vector2(600, 90);
        QueueRedraw();
    }
    public override void _Draw()
    {
        if (!MiraVisivel || MenuAberto) return;
        var cor = new Color("f4ca7f");
        DrawArc(Mira, 12, 0, Mathf.Tau, 32, cor, 1.5f, true);
        DrawLine(Mira + new Vector2(-20, 0), Mira + new Vector2(-8, 0), cor, 1.5f);
        DrawLine(Mira + new Vector2(8, 0), Mira + new Vector2(20, 0), cor, 1.5f);
        DrawLine(Mira + new Vector2(0, -20), Mira + new Vector2(0, -8), cor, 1.5f);
    }
}
