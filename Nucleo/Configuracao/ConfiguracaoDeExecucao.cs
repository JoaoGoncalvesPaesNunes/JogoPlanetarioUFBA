namespace ProjetoPlanetario.Nucleo.Configuracao;

/// <summary>
/// Configuracao minima do ambiente ativo.
/// </summary>
public sealed class ConfiguracaoDeExecucao
{
    public PerfilDeExecucao Perfil { get; }
    public int QuantidadeDeEstacoes { get; }
    public bool DepuracaoAtiva { get; }

    private ConfiguracaoDeExecucao(PerfilDeExecucao perfil, int quantidadeDeEstacoes, bool depuracaoAtiva)
    {
        Perfil = perfil;
        QuantidadeDeEstacoes = quantidadeDeEstacoes;
        DepuracaoAtiva = depuracaoAtiva;
    }

    public static ConfiguracaoDeExecucao CriarDesktopDev()
    {
        return new ConfiguracaoDeExecucao(PerfilDeExecucao.DesktopDev, quantidadeDeEstacoes: 1, depuracaoAtiva: true);
    }
}
