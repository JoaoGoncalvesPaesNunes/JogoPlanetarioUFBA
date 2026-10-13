using Godot;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Portas;

namespace ProjetoPlanetario.Publico.Estacoes;

public partial class EstacaoVirtual : Control
{
    private IEntradaDeEstacao? _entradaDeEstacao;

    public void Configurar(IEntradaDeEstacao entradaDeEstacao)
    {
        _entradaDeEstacao = entradaDeEstacao;
    }

    public override void _Ready()
    {
        AnchorLeft = 1f;
        AnchorRight = 1f;
        OffsetLeft = -360f;
        OffsetRight = -24f;
        OffsetTop = 112f;
        OffsetBottom = 328f;

        var painel = new PanelContainer();
        painel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color("172623"), ContentMarginLeft = 10, ContentMarginRight = 10,
            ContentMarginTop = 10, ContentMarginBottom = 10
        });
        painel.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(painel);

        var grade = new GridContainer
        {
            Columns = 3
        };
        painel.AddChild(grade);

        AdicionarEspaco(grade);
        AdicionarBotao(grade, "CIMA", AcaoLogicaDaEstacao.Cima);
        AdicionarEspaco(grade);
        AdicionarBotao(grade, "ESQ", AcaoLogicaDaEstacao.Esquerda);
        AdicionarBotao(grade, "ACAO 1", AcaoLogicaDaEstacao.Acao1);
        AdicionarBotao(grade, "DIR", AcaoLogicaDaEstacao.Direita);
        AdicionarEspaco(grade);
        AdicionarBotao(grade, "BAIXO", AcaoLogicaDaEstacao.Baixo);
        AdicionarEspaco(grade);
        AdicionarBotao(grade, "ACAO 2", AcaoLogicaDaEstacao.Acao2);
        AdicionarBotao(grade, "CONFIRMAR", AcaoLogicaDaEstacao.Confirmar);
        AdicionarBotao(grade, "CANCELAR", AcaoLogicaDaEstacao.Cancelar);
    }

    private void AdicionarBotao(GridContainer grade, string texto, AcaoLogicaDaEstacao acao)
    {
        var botao = new Button
        {
            Text = texto,
            CustomMinimumSize = new Vector2(96, 44),
            FocusMode = FocusModeEnum.None
        };

        botao.Pressed += () => _entradaDeEstacao?.EnviarAcao(acao);
        grade.AddChild(botao);
    }

    private static void AdicionarEspaco(GridContainer grade)
    {
        grade.AddChild(new Control
        {
            CustomMinimumSize = new Vector2(96, 44)
        });
    }
}
