using System;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Jogabilidade.Eventos;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

var estadoDaNave = new EstadoDaNave();
Garantir(estadoDaNave.Energia == 100f, "Energia inicial deve ser 100.");
Garantir(estadoDaNave.ConsumirEnergia(10f), "Consumo de energia positivo deve alterar estado.");
Garantir(Math.Abs(estadoDaNave.Energia - 90f) < 0.001f, "Energia deve cair para 90.");
Garantir(!estadoDaNave.ConsumirEnergia(0f), "Consumo zero nao deve alterar estado.");
Garantir(estadoDaNave.AlterarEstabilidadeDaDobra(50f), "Estabilidade deve aceitar variacao positiva.");
Garantir(Math.Abs(estadoDaNave.EstabilidadeDaDobra - 100f) < 0.001f, "Estabilidade deve respeitar limite superior.");
Garantir(estadoDaNave.AlterarEstabilidadeDaDobra(-120f), "Estabilidade deve aceitar variacao negativa.");
Garantir(Math.Abs(estadoDaNave.EstabilidadeDaDobra) < 0.001f, "Estabilidade deve respeitar limite inferior.");
Garantir(estadoDaNave.AdicionarCargaDeDobra(50f), "Carga de dobra deve aceitar incremento.");
Garantir(Math.Abs(estadoDaNave.CargaDeDobra - 50f) < 0.001f, "Carga de dobra deve chegar a 50.");
Garantir(estadoDaNave.AdicionarCargaDeDobra(80f), "Carga de dobra deve aceitar incremento ate o limite.");
Garantir(Math.Abs(estadoDaNave.CargaDeDobra - 100f) < 0.001f, "Carga de dobra deve respeitar limite superior.");

var barramento = new BarramentoDeEventos();
EventoDeDominio? eventoRecebido = null;
barramento.EventoPublicado += evento => eventoRecebido = evento;
barramento.Publicar(new EnergiaAlterada(estadoDaNave.Energia, "Teste", DateTime.UtcNow));
Garantir(eventoRecebido is EnergiaAlterada, "Barramento deve entregar evento publicado.");

var eventoDeCalibracao = new EventoDeCalibracao();
eventoDeCalibracao.Iniciar();
Garantir(eventoDeCalibracao.Estado == EstadoEventoDeCalibracao.AguardandoAcao1, "Calibracao deve aguardar ACAO 1.");
Garantir(!eventoDeCalibracao.ReceberAcao(AcaoLogicaDaEstacao.Confirmar), "Confirmar cedo nao deve concluir calibracao.");
Garantir(!eventoDeCalibracao.ReceberAcao(AcaoLogicaDaEstacao.Acao1), "ACAO 1 prepara confirmacao, mas nao conclui sozinha.");
Garantir(eventoDeCalibracao.Estado == EstadoEventoDeCalibracao.AguardandoConfirmacao, "Calibracao deve aguardar confirmacao.");
Garantir(eventoDeCalibracao.ReceberAcao(AcaoLogicaDaEstacao.Confirmar), "Confirmar depois de ACAO 1 deve concluir calibracao.");

var estadoDoLoop = new EstadoDaNave();
var barramentoDoLoop = new BarramentoDeEventos();
var loop = new ControladorDoLoopCentral(estadoDoLoop, barramentoDoLoop);
loop.Iniciar();
Garantir(loop.Estado == EstadoDoLoopCentral.Navegando, "Loop deve iniciar navegando.");
loop.AtualizarNavegacao(1f, 3f);
Garantir(loop.Estado == EstadoDoLoopCentral.AguardandoPassageiro, "Waypoint 1 deve iniciar evento cooperativo.");
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Acao1);
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Confirmar);
Garantir(Math.Abs(estadoDoLoop.CargaDeDobra - 50f) < 0.001f, "Primeira calibracao deve carregar dobra para 50.");
Garantir(loop.Estado == EstadoDoLoopCentral.Navegando, "Loop deve seguir para o segundo waypoint.");
loop.AtualizarNavegacao(1f, 3f);
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Acao1);
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Confirmar);
Garantir(Math.Abs(estadoDoLoop.CargaDeDobra - 100f) < 0.001f, "Segunda calibracao deve carregar dobra para 100.");
Garantir(loop.Estado == EstadoDoLoopCentral.Concluido, "Loop deve concluir apos dois ciclos.");
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Confirmar);
Garantir(loop.Estado == EstadoDoLoopCentral.Navegando, "Confirmar ao final deve reiniciar o loop.");
Garantir(Math.Abs(estadoDoLoop.CargaDeDobra) < 0.001f, "Reinicio deve zerar carga de dobra.");

Console.WriteLine("Testes de dominio concluidos com sucesso.");

static void Garantir(bool condicao, string mensagem)
{
    if (!condicao)
    {
        throw new InvalidOperationException(mensagem);
    }
}
