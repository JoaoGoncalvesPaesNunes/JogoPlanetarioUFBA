using System;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Narrativa.Diretor;

/// <summary>
/// Diretor narrativo minimo que reage a eventos sem conhecer UI, audio ou animacao.
/// </summary>
public sealed class DiretorNarrativo
{
    private readonly BarramentoDeEventos _barramentoDeEventos;

    public string UltimaLinha { get; private set; } = "ORION: Perfil DESKTOP_DEV em inicializacao.";

    public DiretorNarrativo(BarramentoDeEventos barramentoDeEventos)
    {
        _barramentoDeEventos = barramentoDeEventos;
        _barramentoDeEventos.EventoPublicado += AoReceberEvento;
    }

    private void AoReceberEvento(EventoDeDominio eventoDeDominio)
    {
        switch (eventoDeDominio)
        {
            case PerfilDeExecucaoCarregado evento:
                SolicitarDialogo("ORION", $"Perfil {evento.NomeDoPerfil} carregado. Sistemas em modo de teste.");
                break;
            case EnergiaAlterada evento when evento.EnergiaAtual <= 50f:
                SolicitarDialogo("Gatinho", "Energia abaixo da metade. Hora de pilotar com carinho.");
                break;
            case AcaoDoPilotoExecutada:
                SolicitarDialogo("ORION", "Pulso de teste executado pela acao primaria.");
                break;
            case EstacaoAcaoExecutada evento:
                SolicitarDialogo("Gatinho", $"{evento.IdentificadorDaEstacao} enviou {evento.NomeDaAcao}.");
                break;
        }
    }

    private void SolicitarDialogo(string personagem, string texto)
    {
        UltimaLinha = $"{personagem}: {texto}";
        _barramentoDeEventos.Publicar(new DialogoSolicitado(personagem, texto, DateTime.UtcNow));
    }
}
