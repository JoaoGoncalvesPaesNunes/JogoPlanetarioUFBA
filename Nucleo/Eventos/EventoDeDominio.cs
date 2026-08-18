using System;

namespace ProjetoPlanetario.Nucleo.Eventos;

public abstract record EventoDeDominio(DateTime MomentoUtc);

public sealed record PerfilDeExecucaoCarregado(string NomeDoPerfil, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record SessaoDeTesteIniciada(DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record ObjetivoAlterado(string TextoDoObjetivo, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record MovimentoDoPilotoAlterado(float X, float Y, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record VelocidadeDaNaveAlterada(float VelocidadeAtual, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record AcaoDoPilotoExecutada(string NomeDaAcao, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record EnergiaAlterada(float EnergiaAtual, string Motivo, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record EstabilidadeDaDobraAlterada(float EstabilidadeAtual, string Motivo, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record AlinhamentoDaRotaAlterado(float AlinhamentoAtual, float DesvioX, float DesvioY, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record RotaSelecionada(string NomeDaRota, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record ProgressoDoWaypointAlterado(int NumeroDoWaypoint, float ProgressoAtual, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record WaypointAlcancado(int NumeroDoWaypoint, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record CalibracaoIniciada(int NumeroDoWaypoint, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record CalibracaoConcluida(int NumeroDoWaypoint, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record CargaDeDobraAlterada(float CargaAtual, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record LoopCentralConcluido(DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record LoopCentralReiniciado(DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record EstacaoAcaoExecutada(string IdentificadorDaEstacao, string NomeDaAcao, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record DialogoSolicitado(string Personagem, string Texto, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);
