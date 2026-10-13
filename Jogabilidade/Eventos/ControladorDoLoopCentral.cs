using System;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Jogabilidade.Eventos;

/// <summary>
/// Controla dois ciclos de navegacao + calibracao cooperativa sem depender de UI ou hardware.
/// </summary>
public sealed class ControladorDoLoopCentral
{
	private const float AlinhamentoMinimoParaAproximar = 0.72f;
	private const float GanhoProgressoPorSegundo = 42f;
	private const float PerdaProgressoPorSegundo = 18f;
	private const float CargaPorCalibracao = 50f;

	private readonly EstadoDaNave _estadoDaNave;
	private readonly BarramentoDeEventos _barramentoDeEventos;
	private readonly EventoDeCalibracao _eventoDeCalibracao = new();
	private float _ultimoProgressoPublicado = -1f;

	public EstadoDoLoopCentral Estado { get; private set; } = EstadoDoLoopCentral.Inicializando;
	public int NumeroDoWaypoint { get; private set; } = 1;
	public float ProgressoDoWaypoint { get; private set; }
	public string ObjetivoAtual { get; private set; } = "Inicializando sistemas.";

	public ControladorDoLoopCentral(EstadoDaNave estadoDaNave, BarramentoDeEventos barramentoDeEventos)
	{
		_estadoDaNave = estadoDaNave;
		_barramentoDeEventos = barramentoDeEventos;
	}

	public void Iniciar()
	{
		_estadoDaNave.ReiniciarParaTeste();
		NumeroDoWaypoint = 1;
		ProgressoDoWaypoint = 0f;
		_ultimoProgressoPublicado = -1f;
		_eventoDeCalibracao.Reiniciar();
		Estado = EstadoDoLoopCentral.Navegando;

		_barramentoDeEventos.Publicar(new LoopCentralReiniciado(DateTime.UtcNow));
		_barramentoDeEventos.Publicar(new SessaoDeTesteIniciada(DateTime.UtcNow));
		DefinirObjetivo("Alcance o ponto de calibracao 1.");
		_barramentoDeEventos.Publicar(new CargaDeDobraAlterada(_estadoDaNave.CargaDeDobra, DateTime.UtcNow));
	}

	public void AtualizarNavegacao(float alinhamentoDaRota, float velocidadeFrontal, float delta)
	{
		if (Estado != EstadoDoLoopCentral.Navegando)
		{
			return;
		}

		var velocidadeUtil = Math.Clamp(velocidadeFrontal, 0f, 1f);
		var variacao = alinhamentoDaRota >= AlinhamentoMinimoParaAproximar && velocidadeUtil > 0.05f
			? GanhoProgressoPorSegundo * velocidadeUtil * delta
			: -PerdaProgressoPorSegundo * (1f - alinhamentoDaRota) * delta;

		ProgressoDoWaypoint = Math.Clamp(ProgressoDoWaypoint + variacao, 0f, 100f);
		PublicarProgressoSeNecessario();

		if (ProgressoDoWaypoint >= 100f)
		{
			IniciarCalibracao();
		}
	}

	public void ReceberAcaoDaEstacao(AcaoLogicaDaEstacao acao)
	{
		if (Estado == EstadoDoLoopCentral.Concluido && acao == AcaoLogicaDaEstacao.Confirmar)
		{
			Iniciar();
			return;
		}

		if (Estado != EstadoDoLoopCentral.AguardandoPassageiro)
		{
			return;
		}

		var concluido = _eventoDeCalibracao.ReceberAcao(acao);

		if (_eventoDeCalibracao.Estado == EstadoEventoDeCalibracao.AguardandoConfirmacao)
		{
			DefinirObjetivo("Suporte recebido. Passageiro deve confirmar.");
		}

		if (concluido)
		{
			ConcluirCalibracao();
		}
	}

	private void IniciarCalibracao()
	{
		Estado = EstadoDoLoopCentral.AguardandoPassageiro;
		_eventoDeCalibracao.Iniciar();
		_barramentoDeEventos.Publicar(new WaypointAlcancado(NumeroDoWaypoint, DateTime.UtcNow));
		_barramentoDeEventos.Publicar(new CalibracaoIniciada(NumeroDoWaypoint, DateTime.UtcNow));
		DefinirObjetivo("Aguardando suporte da tripulacao: pressione ACAO 1 na estacao.");
	}

	private void ConcluirCalibracao()
	{
		if (_estadoDaNave.ConsumirEnergia(8f))
		{
			_barramentoDeEventos.Publicar(new EnergiaAlterada(_estadoDaNave.Energia, "CalibracaoCooperativa", DateTime.UtcNow));
		}

		if (_estadoDaNave.AdicionarCargaDeDobra(CargaPorCalibracao))
		{
			_barramentoDeEventos.Publicar(new CargaDeDobraAlterada(_estadoDaNave.CargaDeDobra, DateTime.UtcNow));
		}

		if (_estadoDaNave.AlterarEstabilidadeDaDobra(12f))
		{
			_barramentoDeEventos.Publicar(new EstabilidadeDaDobraAlterada(_estadoDaNave.EstabilidadeDaDobra, "CalibracaoCooperativa", DateTime.UtcNow));
		}

		_barramentoDeEventos.Publicar(new CalibracaoConcluida(NumeroDoWaypoint, DateTime.UtcNow));
		_eventoDeCalibracao.Reiniciar();

		if (_estadoDaNave.CargaDeDobra >= 100f)
		{
			Estado = EstadoDoLoopCentral.Concluido;
			DefinirObjetivo("LOOP CENTRAL CONCLUIDO. Pressione CONFIRMAR na estacao para reiniciar.");
			_barramentoDeEventos.Publicar(new LoopCentralConcluido(DateTime.UtcNow));
			return;
		}

		NumeroDoWaypoint++;
		ProgressoDoWaypoint = 0f;
		_ultimoProgressoPublicado = -1f;
		Estado = EstadoDoLoopCentral.Navegando;
		DefinirObjetivo($"Alcance o ponto de calibracao {NumeroDoWaypoint}.");
		_barramentoDeEventos.Publicar(new ProgressoDoWaypointAlterado(NumeroDoWaypoint, ProgressoDoWaypoint, DateTime.UtcNow));
	}

	private void DefinirObjetivo(string texto)
	{
		ObjetivoAtual = texto;
		_barramentoDeEventos.Publicar(new ObjetivoAlterado(texto, DateTime.UtcNow));
	}

	private void PublicarProgressoSeNecessario()
	{
		if (Math.Abs(ProgressoDoWaypoint - _ultimoProgressoPublicado) < 5f && ProgressoDoWaypoint < 100f)
		{
			return;
		}

		_ultimoProgressoPublicado = ProgressoDoWaypoint;
		_barramentoDeEventos.Publicar(new ProgressoDoWaypointAlterado(NumeroDoWaypoint, ProgressoDoWaypoint, DateTime.UtcNow));
	}
}
