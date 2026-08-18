using System;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Narrativa.Diretor;

/// <summary>
/// Diretor narrativo minimo que reage a eventos sem conhecer UI, audio ou animacao.
/// </summary>
public sealed class DiretorNarrativo
{
    private readonly BarramentoDeEventos _barramentoDeEventos;
    private bool _alertaDeEstabilidadeCriticaEnviado;

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
            case EstabilidadeDaDobraAlterada evento when evento.EstabilidadeAtual <= 25f && !_alertaDeEstabilidadeCriticaEnviado:
                _alertaDeEstabilidadeCriticaEnviado = true;
                SolicitarDialogo("ORION", "Estabilidade de dobra critica. Realinhe a rota ou solicite apoio da tripulacao.");
                break;
            case EstabilidadeDaDobraAlterada evento when evento.EstabilidadeAtual >= 55f:
                _alertaDeEstabilidadeCriticaEnviado = false;
                break;
            case RotaSelecionada evento:
                SolicitarDialogo("ORION", $"Rota demonstrativa para {evento.NomeDaRota} marcada no painel.");
                break;
            case AcaoDoPilotoExecutada:
                SolicitarDialogo("ORION", "Estabilizador de dobra acionado.");
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
