using System;
using System.Linq;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Jogabilidade.Eventos;
using ProjetoPlanetario.Jogabilidade.Viagem;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

TestesRobustos.Executar();
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
loop.AtualizarNavegacao(1f, 0f, 3f);
Garantir(loop.Estado == EstadoDoLoopCentral.Navegando, "Waypoint nao deve completar sem velocidade frontal.");
Garantir(Math.Abs(loop.ProgressoDoWaypoint) < 0.001f, "Progresso deve continuar zero sem impulso.");
loop.AtualizarNavegacao(1f, 1f, 3f);
Garantir(loop.Estado == EstadoDoLoopCentral.AguardandoPassageiro, "Waypoint 1 deve iniciar evento cooperativo.");
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Acao1);
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Confirmar);
Garantir(Math.Abs(estadoDoLoop.CargaDeDobra - 50f) < 0.001f, "Primeira calibracao deve carregar dobra para 50.");
Garantir(loop.Estado == EstadoDoLoopCentral.Navegando, "Loop deve seguir para o segundo waypoint.");
loop.AtualizarNavegacao(1f, 1f, 3f);
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Acao1);
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Confirmar);
Garantir(Math.Abs(estadoDoLoop.CargaDeDobra - 100f) < 0.001f, "Segunda calibracao deve carregar dobra para 100.");
Garantir(loop.Estado == EstadoDoLoopCentral.Concluido, "Loop deve concluir apos dois ciclos.");
loop.ReceberAcaoDaEstacao(AcaoLogicaDaEstacao.Confirmar);
Garantir(loop.Estado == EstadoDoLoopCentral.Navegando, "Confirmar ao final deve reiniciar o loop.");
Garantir(Math.Abs(estadoDoLoop.CargaDeDobra) < 0.001f, "Reinicio deve zerar carga de dobra.");

var segmento = SegmentoDeViagem.CriarSegmentoCometa();
segmento.Iniciar();
Garantir(segmento.Estado == EstadoDoSegmentoDeViagem.EmViagem, "Segmento deve iniciar em viagem.");
segmento.Atualizar(-1f, 2f);
Garantir(Math.Abs(segmento.ProgressoNaRota) < 0.001f, "Progresso nao deve ficar abaixo do inicio.");

var resultadoAteCometa = segmento.Atualizar(0.2f, 2.8f);
Garantir(segmento.ProgressoNaRota > 0.5f && segmento.ProgressoNaRota < 0.6f, "Segmento deve avancar fisicamente pela rota.");
Garantir(resultadoAteCometa.MarcadoresDisparados.Any(marcador => marcador.Tipo == TipoMarcadorDeRota.AproximacaoCometa), "Aproximacao do cometa deve disparar.");
Garantir(resultadoAteCometa.MarcadoresDisparados.Any(marcador => marcador.Tipo == TipoMarcadorDeRota.PassagemCometa), "Passagem do cometa deve disparar.");

segmento.Atualizar(-0.2f, 1f);
var resultadoRepetido = segmento.Atualizar(0.2f, 1f);
Garantir(!resultadoRepetido.MarcadoresDisparados.Any(marcador => marcador.Tipo == TipoMarcadorDeRota.AproximacaoCometa), "Aproximacao nao deve repetir depois de dar re.");
Garantir(!resultadoRepetido.MarcadoresDisparados.Any(marcador => marcador.Tipo == TipoMarcadorDeRota.PassagemCometa), "Passagem nao deve repetir depois de dar re.");

var resultadoFim = segmento.Atualizar(1f, 2f);
Garantir(segmento.ProgressoNaRota <= 1f, "Progresso nao deve passar do fim.");
Garantir(resultadoFim.MarcadoresDisparados.Any(marcador => marcador.Tipo == TipoMarcadorDeRota.InicioSalto), "Inicio do salto deve disparar no fim.");
Garantir(segmento.Estado == EstadoDoSegmentoDeViagem.Saltando, "Segmento deve entrar em salto.");
segmento.ConcluirSalto();
Garantir(segmento.Estado == EstadoDoSegmentoDeViagem.Concluido, "Segmento deve concluir salto.");
segmento.ConcluirSalto();
Garantir(segmento.Estado == EstadoDoSegmentoDeViagem.Concluido, "Concluir salto novamente nao deve alterar estado.");

segmento.Iniciar();
var resultadoRearmado = segmento.Atualizar(0.2f, 2.8f);
Garantir(resultadoRearmado.MarcadoresDisparados.Any(marcador => marcador.Tipo == TipoMarcadorDeRota.AproximacaoCometa), "Reinicio deve rearmar marcadores.");

var salto = new ControladorDoSalto();
Garantir(!salto.EscolherDestino("marte"), "Nao deve aceitar destino antes do ponto de salto.");
salto.SolicitarDestino();
salto.Atualizar(100);
Garantir(salto.Fase == FaseDoSalto.AguardandoDestino, "Sem escolha nao ha salto automatico.");
Garantir(!salto.EscolherDestino("saturno"), "Somente Marte e Jupiter sao aceitos.");
Garantir(salto.EscolherDestino("marte"), "Escolha valida inicia preparacao.");
Garantir(!salto.EscolherDestino("jupiter") && salto.DestinoSelecionado == "marte", "Destino bloqueado durante o salto.");
salto.ConfirmarCarregamento(true);
Garantir(!salto.DestinoCarregado, "Carga nao pode ser confirmada antes do hiperespaço.");
salto.Atualizar(1);
Garantir(salto.Fase == FaseDoSalto.Acelerando && salto.Intensidade == 0, "Aceleracao inicia suave.");
salto.Atualizar(0.6f);
Garantir(Math.Abs(salto.Intensidade - 0.5f) < 0.01f, "Intensidade cresce gradualmente.");
salto.Atualizar(0.6f);
Garantir(salto.Fase == FaseDoSalto.Hiperespaco && !salto.PrecisaCarregar, "Tunel precisa ser apresentado antes da troca.");
salto.Atualizar(20);
Garantir(salto.Fase == FaseDoSalto.Hiperespaco && salto.PrecisaCarregar, "Carga lenta mantem o hiperespaço.");
salto.ConfirmarCarregamento(true);
Garantir(!salto.PrecisaCarregar, "Carga ocorre uma vez.");
salto.Atualizar(0.01f);
Garantir(salto.Fase == FaseDoSalto.Desacelerando, "Desacelera somente com o destino pronto.");
salto.Atualizar(1.2f);
Garantir(salto.Fase == FaseDoSalto.Chegada && salto.Intensidade == 0, "Chegada remove o efeito.");
salto.Reiniciar();
Garantir(salto.DestinoSelecionado == "" && !salto.DestinoCarregado, "Reinicio limpa destino e carga.");
salto.SolicitarDestino();
salto.EscolherDestino("jupiter");
salto.Atualizar(1);
salto.Atualizar(1.2f);
salto.Atualizar(0.2f);
salto.ConfirmarCarregamento(false);
Garantir(salto.Fase == FaseDoSalto.Falha && salto.AceitaEscolha, "Falha permite nova tentativa.");
Garantir(salto.EscolherDestino("marte"), "Outra escolha recupera falha.");
salto.Atualizar(float.NaN);
salto.Atualizar(-1);
Garantir(salto.TempoNaFase == 0, "Deltas invalidos nao corrompem o estado.");

Console.WriteLine("Testes de dominio e salto concluidos com sucesso.");

static void Garantir(bool condicao, string mensagem)
{
    if (!condicao)
    {
        throw new InvalidOperationException(mensagem);
    }
}
