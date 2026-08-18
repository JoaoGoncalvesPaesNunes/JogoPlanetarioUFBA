using System;

namespace ProjetoPlanetario.Nucleo.Eventos;

public abstract record EventoDeDominio(DateTime MomentoUtc);

public sealed record PerfilDeExecucaoCarregado(string NomeDoPerfil, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record MovimentoDoPilotoAlterado(float X, float Y, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record AcaoDoPilotoExecutada(string NomeDaAcao, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record EnergiaAlterada(float EnergiaAtual, string Motivo, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record EstacaoAcaoExecutada(string IdentificadorDaEstacao, string NomeDaAcao, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);

public sealed record DialogoSolicitado(string Personagem, string Texto, DateTime MomentoUtc) : EventoDeDominio(MomentoUtc);
