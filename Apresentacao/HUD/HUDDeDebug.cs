using System.Collections.Generic;
using Godot;
using ProjetoPlanetario.Narrativa.Diretor;
using ProjetoPlanetario.Nucleo.Configuracao;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Apresentacao.HUD;

public partial class HUDDeDebug : CanvasLayer
{
    private readonly Queue<string> _eventosRecentes = new();
    private string _ultimaAcao = "Aguardando entrada...";
    private string _rotaAtual = "Ponto de dobra";
    private float _alinhamentoDaRota;
    private float _velocidadeAtual;
    private float _progressoNaRota;
    private string _ultimoMarcador = "-";
    private string _estadoDoSegmento = "NaoIniciado";
    private Label? _texto;
    private EstadoDaNave? _estadoDaNave;
    private ConfiguracaoDeExecucao? _configuracaoDeExecucao;
    private DiretorNarrativo? _diretorNarrativo;

    public void Configurar(
        EstadoDaNave estadoDaNave,
        ConfiguracaoDeExecucao configuracaoDeExecucao,
        DiretorNarrativo diretorNarrativo,
        BarramentoDeEventos barramentoDeEventos)
    {
        _estadoDaNave = estadoDaNave;
        _configuracaoDeExecucao = configuracaoDeExecucao;
        _diretorNarrativo = diretorNarrativo;
        barramentoDeEventos.EventoPublicado += RegistrarEvento;
    }

    public override void _Ready()
    {
        _texto = new Label
        {
            Position = new Vector2(24, 20),
            Size = new Vector2(620, 260),
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        _texto.AddThemeFontSizeOverride("font_size", 18);

        AddChild(_texto);
    }

    public override void _Process(double delta)
    {
        if (_texto is null || _estadoDaNave is null || _configuracaoDeExecucao is null || _diretorNarrativo is null)
        {
            return;
        }

        _texto.Text =
            $"Perfil: {_configuracaoDeExecucao.Perfil}\n" +
            $"Energia: {_estadoDaNave.Energia:0}\n" +
            $"Integridade: {_estadoDaNave.Integridade:0}\n" +
            $"Estabilidade: {_estadoDaNave.EstabilidadeDaDobra:0}\n" +
            $"Carga de dobra: {_estadoDaNave.CargaDeDobra:0}\n" +
            $"Alinhamento: {_alinhamentoDaRota * 100f:0}%\n" +
            $"Movimento na rota: {_velocidadeAtual * 100f:0}%\n" +
            $"Progresso na rota: {_progressoNaRota * 100f:0}%\n" +
            $"Ultimo marcador: {_ultimoMarcador}\n" +
            $"Estado segmento: {_estadoDoSegmento}\n" +
            $"Rota: {_rotaAtual}\n" +
            $"Ultima acao: {_ultimaAcao}\n" +
            $"Narrativa: {_diretorNarrativo.UltimaLinha}\n\n" +
            "Eventos recentes:\n" +
            string.Join("\n", _eventosRecentes);
    }

    private void RegistrarEvento(EventoDeDominio eventoDeDominio)
    {
        var linha = eventoDeDominio switch
        {
            PerfilDeExecucaoCarregado evento => $"Perfil carregado: {evento.NomeDoPerfil}",
            MovimentoDoPilotoAlterado evento => $"Movimento piloto: {evento.X:0.00}, {evento.Y:0.00}",
            AcaoDoPilotoExecutada evento => $"Piloto: {evento.NomeDaAcao}",
            EnergiaAlterada evento => $"Energia alterada: {evento.EnergiaAtual:0}",
            EstabilidadeDaDobraAlterada evento => $"Estabilidade: {evento.EstabilidadeAtual:0} ({evento.Motivo})",
            AlinhamentoDaRotaAlterado evento => $"Alinhamento: {evento.AlinhamentoAtual * 100f:0}%",
            VelocidadeDaNaveAlterada evento => $"Velocidade: {evento.VelocidadeAtual * 100f:0}%",
            ProgressoNaRotaAlterado evento => $"Rota: {evento.ProgressoAtual * 100f:0}%",
            MarcadorDeRotaAlcancado evento => $"Marcador: {evento.Tipo}",
            SegmentoDeViagemIniciado => "Segmento iniciado",
            SegmentoDeViagemConcluido => "Segmento concluido",
            SaltoGravitacionalIniciado => "Salto iniciado",
            SaltoGravitacionalConcluido => "Salto concluido",
            AproximacaoCometa => "Aproximacao do cometa",
            PassagemCometa => "Passagem pelo cometa",
            AvisoDeSalto => "Aviso de salto",
            RotaSelecionada evento => $"Rota selecionada: {evento.NomeDaRota}",
            ObjetivoAlterado evento => $"Objetivo: {evento.TextoDoObjetivo}",
            ProgressoDoWaypointAlterado evento => $"WP {evento.NumeroDoWaypoint}: {evento.ProgressoAtual:0}%",
            WaypointAlcancado evento => $"Waypoint {evento.NumeroDoWaypoint} alcancado",
            CalibracaoIniciada evento => $"Calibracao {evento.NumeroDoWaypoint} iniciada",
            CalibracaoConcluida evento => $"Calibracao {evento.NumeroDoWaypoint} concluida",
            CargaDeDobraAlterada evento => $"Carga de dobra: {evento.CargaAtual:0}%",
            LoopCentralConcluido _ => "Loop central concluido",
            LoopCentralReiniciado _ => "Loop central reiniciado",
            EstacaoAcaoExecutada evento => $"{evento.IdentificadorDaEstacao}: {evento.NomeDaAcao}",
            DialogoSolicitado evento => $"Dialogo {evento.Personagem}",
            _ => eventoDeDominio.GetType().Name
        };

        if (eventoDeDominio is AlinhamentoDaRotaAlterado alinhamento)
        {
            _alinhamentoDaRota = alinhamento.AlinhamentoAtual;
        }

        if (eventoDeDominio is RotaSelecionada rota)
        {
            _rotaAtual = rota.NomeDaRota;
        }

        if (eventoDeDominio is VelocidadeDaNaveAlterada velocidade)
        {
            _velocidadeAtual = velocidade.VelocidadeAtual;
        }

        if (eventoDeDominio is ProgressoNaRotaAlterado progressoNaRota)
        {
            _progressoNaRota = progressoNaRota.ProgressoAtual;
        }

        if (eventoDeDominio is MarcadorDeRotaAlcancado marcador)
        {
            _ultimoMarcador = marcador.Tipo;
        }

        switch (eventoDeDominio)
        {
            case SegmentoDeViagemIniciado:
                _estadoDoSegmento = "EmViagem";
                break;
            case SaltoGravitacionalIniciado:
                _estadoDoSegmento = "Saltando";
                break;
            case SegmentoDeViagemConcluido:
                _estadoDoSegmento = "Concluido";
                break;
            case SegmentoDeViagemReiniciado:
                _estadoDoSegmento = "EmViagem";
                _progressoNaRota = 0f;
                _ultimoMarcador = "-";
                break;
        }

        _ultimaAcao = linha;
        _eventosRecentes.Enqueue(linha);

        while (_eventosRecentes.Count > 7)
        {
            _eventosRecentes.Dequeue();
        }
    }
}
