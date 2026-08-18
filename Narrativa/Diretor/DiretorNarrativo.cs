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
            case SessaoDeTesteIniciada:
                SolicitarDialogo("ORION", "Sistemas de teste operacionais. Dirija-se ao ponto de calibracao.");
                break;
            case SegmentoDeViagemIniciado:
                SolicitarDialogo("ORION", "Trajetoria de teste carregada. Controle apenas o avanco e a observacao.");
                break;
            case AproximacaoCometa:
                SolicitarDialogo("ORION", "Objeto cometario detectado. Passagem proxima em instantes.");
                break;
            case PassagemCometa:
                SolicitarDialogo("ORION", "Cometa em passagem. Observe pela lateral da cabine.");
                break;
            case AvisoDeSalto:
                SolicitarDialogo("ORION", "Anomalia gravitacional a frente. Preparando salto.");
                break;
            case SaltoGravitacionalIniciado:
                SolicitarDialogo("ORION", "Estabilizando campo gravitacional.");
                break;
            case SaltoGravitacionalConcluido:
                SolicitarDialogo("ORION", "Salto concluido. Segmento de teste encerrado.");
                break;
            case WaypointAlcancado evento:
                SolicitarDialogo("ORION", $"Ponto de calibracao {evento.NumeroDoWaypoint} alcancado.");
                break;
            case CalibracaoIniciada:
                SolicitarDialogo("ORION", "Instabilidade detectada. Uma estacao de suporte deve confirmar a calibracao.");
                break;
            case CalibracaoConcluida evento:
                SolicitarDialogo("ORION", $"Calibracao {evento.NumeroDoWaypoint} concluida. Nucleo de dobra parcialmente carregado.");
                break;
            case CargaDeDobraAlterada evento when evento.CargaAtual >= 100f:
                SolicitarDialogo("ORION", "Nucleo de dobra carregado. Sequencia de teste concluida.");
                break;
            case LoopCentralConcluido:
                SolicitarDialogo("Gatinho", "Missao de teste completada. Eu chamaria isso de ciencia confortavel.");
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
