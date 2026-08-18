using ProjetoPlanetario.Entrada.Modelos;

namespace ProjetoPlanetario.Jogabilidade.Eventos;

/// <summary>
/// Evento cooperativo puro: uma estacao precisa acionar suporte e confirmar a calibracao.
/// </summary>
public sealed class EventoDeCalibracao
{
    public EstadoEventoDeCalibracao Estado { get; private set; } = EstadoEventoDeCalibracao.Inativo;
    public bool EstaAtivo => Estado is EstadoEventoDeCalibracao.AguardandoAcao1 or EstadoEventoDeCalibracao.AguardandoConfirmacao;

    public void Iniciar()
    {
        Estado = EstadoEventoDeCalibracao.AguardandoAcao1;
    }

    public bool ReceberAcao(AcaoLogicaDaEstacao acao)
    {
        if (Estado == EstadoEventoDeCalibracao.AguardandoAcao1 && acao == AcaoLogicaDaEstacao.Acao1)
        {
            Estado = EstadoEventoDeCalibracao.AguardandoConfirmacao;
            return false;
        }

        if (Estado == EstadoEventoDeCalibracao.AguardandoConfirmacao && acao == AcaoLogicaDaEstacao.Confirmar)
        {
            Estado = EstadoEventoDeCalibracao.Concluido;
            return true;
        }

        return false;
    }

    public void Reiniciar()
    {
        Estado = EstadoEventoDeCalibracao.Inativo;
    }
}
