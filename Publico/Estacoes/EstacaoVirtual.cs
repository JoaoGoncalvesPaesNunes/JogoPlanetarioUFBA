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
        OffsetTop = 24f;
        OffsetBottom = 340f;

        var painel = new PanelContainer();
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
            CustomMinimumSize = new Vector2(104, 48)
        };

        botao.Pressed += () => _entradaDeEstacao?.EnviarAcao(acao);
        grade.AddChild(botao);
    }

    private static void AdicionarEspaco(GridContainer grade)
    {
        grade.AddChild(new Control
        {
            CustomMinimumSize = new Vector2(104, 48)
        });
    }
}
