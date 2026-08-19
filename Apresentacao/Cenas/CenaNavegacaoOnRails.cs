using System;
using System.Collections.Generic;
using Godot;
using ProjetoPlanetario.Apresentacao.HUD;
using ProjetoPlanetario.Dados.Nave;
using ProjetoPlanetario.Entrada.Adaptadores;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Roteamento;
using ProjetoPlanetario.Jogabilidade.Viagem;
using ProjetoPlanetario.Jogabilidade.Voo;
using ProjetoPlanetario.Narrativa.Diretor;
using ProjetoPlanetario.Nucleo.Configuracao;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;
using ProjetoPlanetario.Projecao.Adaptadores;
using ProjetoPlanetario.Publico.Estacoes;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public partial class CenaNavegacaoOnRails : Node3D
{
    private const float VelocidadeProgressoPorSegundo = 0.085f;
    private const float DuracaoSalto = 2.35f;
    private const string CaminhoPanoramaEspacial = "res://assets/espaco/space_spheremaps/hazy_nebulae_1.png";

    private readonly BarramentoDeEventos _barramentoDeEventos = new();
    private readonly EstadoDaNave _estadoDaNave = new();
    private readonly ConfiguracaoDeExecucao _configuracaoDeExecucao = ConfiguracaoDeExecucao.CriarDesktopDev();
    private readonly ConfiguracaoDaNave _configuracaoDaNave = new();
    private readonly SegmentoDeViagem _segmentoDeViagem = SegmentoDeViagem.CriarSegmentoCometa();
    private readonly List<Node3D> _objetosDeCenario = new();

    private ControladorDeVoo? _controladorDeVoo;
    private PathFollow3D? _seguidorDaNave;
    private CockpitDoPiloto? _cockpitDoPiloto;
    private HUDDeDebug? _hudDeDebug;
    private ApresentacaoDoSalto? _apresentacaoDoSalto;
    private float _tempoDeSalto;
    private bool _saltoConcluidoPublicado;

    public override void _Ready()
    {
        ConfiguradorInputMap.GarantirAcoesDesktopDev();

        var diretorNarrativo = new DiretorNarrativo(_barramentoDeEventos);
        new AdaptadorProjecaoDesktop(new Rect2(Vector2.Zero, GetViewport().GetVisibleRect().Size))
            .AplicarEmViewport(GetViewport());

        _barramentoDeEventos.EventoPublicado += AoEventoPublicado;

        CriarIluminacao();
        CriarRota();
        CriarCenario();
        CriarCometa();
        CriarCockpitVisual(diretorNarrativo);
        CriarHudDebug(diretorNarrativo);
        CriarEstacaoVirtual();
        CriarOverlayDeSalto();

        ReiniciarSegmento();
    }

    public override void _Process(double delta)
    {
        if (_controladorDeVoo is null || _seguidorDaNave is null)
        {
            return;
        }

        var deltaFloat = (float)delta;

        if (_segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.EmViagem)
        {
            var resultado = _segmentoDeViagem.Atualizar(
                _controladorDeVoo.VelocidadeFrontalAtual * VelocidadeProgressoPorSegundo,
                deltaFloat);

            _seguidorDaNave.ProgressRatio = _segmentoDeViagem.ProgressoNaRota;
            _barramentoDeEventos.Publicar(new ProgressoNaRotaAlterado(
                _segmentoDeViagem.ProgressoNaRota,
                _controladorDeVoo.VelocidadeFrontalAtual,
                DateTime.UtcNow));

            foreach (var marcador in resultado.MarcadoresDisparados)
            {
                PublicarMarcador(marcador);
            }

            AnimarCenario(deltaFloat);
        }

        if (_segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.Saltando)
        {
            AtualizarSalto(deltaFloat);
        }
    }

    private void CriarIluminacao()
    {
        var texturaPanorama = GD.Load<Texture2D>(CaminhoPanoramaEspacial);
        if (texturaPanorama is null)
        {
            GD.PushError($"Nao foi possivel carregar o panorama espacial: {CaminhoPanoramaEspacial}");
        }

        AddChild(new WorldEnvironment
        {
            Name = "AmbienteOnRails",
            Environment = new Godot.Environment
            {
                BackgroundMode = texturaPanorama is null ? Godot.Environment.BGMode.Color : Godot.Environment.BGMode.Sky,
                BackgroundColor = new Color(0.003f, 0.005f, 0.012f),
                Sky = texturaPanorama is null
                    ? null
                    : new Sky
                    {
                        SkyMaterial = new PanoramaSkyMaterial
                        {
                            Panorama = texturaPanorama
                        }
                    },
                AmbientLightSource = Godot.Environment.AmbientSource.Color,
                AmbientLightColor = new Color(0.035f, 0.055f, 0.09f),
                AmbientLightEnergy = 0.35f
            }
        });

        AddChild(new DirectionalLight3D
        {
            Name = "LuzPrincipal",
            Rotation = new Vector3(-0.7f, -0.65f, 0.1f),
            LightColor = new Color(0.72f, 0.83f, 1f),
            LightEnergy = 2.35f
        });

        AddChild(new OmniLight3D
        {
            Name = "LuzRimAzulCenario",
            Position = new Vector3(5f, 3f, -18f),
            LightColor = new Color(0.2f, 0.62f, 1f),
            LightEnergy = 1.15f,
            OmniRange = 22f
        });
    }

    private void CriarRota()
    {
        var rota = new Path3D
        {
            Name = "TrajetoriaOnRails",
            Curve = new Curve3D()
        };
        rota.Curve.AddPoint(new Vector3(0f, 0f, 0f));
        rota.Curve.AddPoint(new Vector3(1.5f, 0.25f, -12f));
        rota.Curve.AddPoint(new Vector3(-1.2f, 0.55f, -26f));
        rota.Curve.AddPoint(new Vector3(0.5f, 0.1f, -40f));
        AddChild(rota);

        _seguidorDaNave = new PathFollow3D
        {
            Name = "SeguidorDaNave",
            Loop = false,
            RotationMode = PathFollow3D.RotationModeEnum.Oriented
        };
        rota.AddChild(_seguidorDaNave);

        _controladorDeVoo = new ControladorDeVoo
        {
            Name = "ControladorDaNaveOnRails"
        };
        _controladorDeVoo.Configurar(new AdaptadorTecladoMouse(), _estadoDaNave, _barramentoDeEventos, _configuracaoDaNave);

        _controladorDeVoo.AddChild(new Camera3D
        {
            Name = "CameraPiloto",
            Current = true,
            Fov = 74f,
            Near = 0.05f
        });
        _seguidorDaNave.AddChild(_controladorDeVoo);
    }

    private void CriarCenario()
    {
        var cenario = CarregarCenaNode3D("res://Apresentacao/Cenas/CenarioEspacialMarco04.tscn", "CenarioEspacialMarco04");
        AddChild(cenario);
        RegistrarObjetosAnimaveis(cenario);
    }

    private void CriarCometa()
    {
        var cometa = CarregarCenaNode3D("res://Apresentacao/Cenas/CometaMarco04.tscn", "CometaMarco04");
        AddChild(cometa);
        RegistrarObjetosAnimaveis(cometa);
    }

    private static Node3D CarregarCenaNode3D(string caminho, string nomeFallback)
    {
        var cena = GD.Load<PackedScene>(caminho);
        if (cena is null)
        {
            GD.PushError($"Nao foi possivel carregar a cena de asset: {caminho}");
            return new Node3D { Name = nomeFallback };
        }

        return cena.Instantiate<Node3D>();
    }

    private void RegistrarObjetosAnimaveis(Node raiz)
    {
        foreach (var filho in raiz.FindChildren("*", "Node3D", true, false))
        {
            if (filho is Node3D objetoAnimavel && objetoAnimavel.IsInGroup("cenario_animavel"))
            {
                _objetosDeCenario.Add(objetoAnimavel);
            }
        }
    }

    private void CriarCockpitVisual(DiretorNarrativo diretorNarrativo)
    {
        if (_controladorDeVoo is null)
        {
            return;
        }

        _cockpitDoPiloto = new CockpitDoPiloto { Name = "CockpitGrayboxOnRails" };
        _cockpitDoPiloto.Configurar(_controladorDeVoo, _estadoDaNave, null, diretorNarrativo, _barramentoDeEventos);
        AddChild(_cockpitDoPiloto);
    }

    private void CriarHudDebug(DiretorNarrativo diretorNarrativo)
    {
        _hudDeDebug = new HUDDeDebug { Name = "HUDDeDebug" };
        _hudDeDebug.Configurar(_estadoDaNave, _configuracaoDeExecucao, diretorNarrativo, _barramentoDeEventos);
        AddChild(_hudDeDebug);
    }

    private void CriarEstacaoVirtual()
    {
        var estacao = new EstacaoVirtual { Name = "EstacaoVirtual" };
        estacao.Configurar(new AdaptadorEntradaVirtual("Estacao1", _barramentoDeEventos));
        AddChild(estacao);
    }

    private void CriarOverlayDeSalto()
    {
        _apresentacaoDoSalto = new ApresentacaoDoSalto { Name = "ApresentacaoDoSalto" };
        _apresentacaoDoSalto.Configurar(_barramentoDeEventos);
        AddChild(_apresentacaoDoSalto);
    }

    private void AoEventoPublicado(EventoDeDominio eventoDeDominio)
    {
        RegistrarEventoImportante(eventoDeDominio);

        if (eventoDeDominio is EstacaoAcaoExecutada evento &&
            Enum.TryParse<AcaoLogicaDaEstacao>(evento.NomeDaAcao, out var acao))
        {
            if (_segmentoDeViagem.Estado == EstadoDoSegmentoDeViagem.Concluido && acao == AcaoLogicaDaEstacao.Confirmar)
            {
                ReiniciarSegmento();
            }
        }
    }

    private void PublicarMarcador(MarcadorDeRota marcador)
    {
        _barramentoDeEventos.Publicar(new MarcadorDeRotaAlcancado(marcador.Id, marcador.Tipo.ToString(), marcador.Progresso, DateTime.UtcNow));

        switch (marcador.Tipo)
        {
            case TipoMarcadorDeRota.InicioNarrativa:
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Avance pela rota de teste.", DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.AproximacaoCometa:
                _barramentoDeEventos.Publicar(new AproximacaoCometa(DateTime.UtcNow));
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Observe o cometa a direita da rota.", DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.PassagemCometa:
                _barramentoDeEventos.Publicar(new PassagemCometa(DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.AvisoSalto:
                _barramentoDeEventos.Publicar(new AvisoDeSalto(DateTime.UtcNow));
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Prossiga ate a anomalia gravitacional.", DateTime.UtcNow));
                break;
            case TipoMarcadorDeRota.InicioSalto:
                _controladorDeVoo?.ZerarVelocidade();
                _barramentoDeEventos.Publicar(new SaltoGravitacionalIniciado(DateTime.UtcNow));
                _barramentoDeEventos.Publicar(new ObjetivoAlterado("Salto em andamento.", DateTime.UtcNow));
                break;
        }
    }

    private void AtualizarSalto(float delta)
    {
        if (_saltoConcluidoPublicado)
        {
            return;
        }

        _tempoDeSalto += delta;
        if (_tempoDeSalto < DuracaoSalto)
        {
            return;
        }

        _saltoConcluidoPublicado = true;
        _segmentoDeViagem.ConcluirSalto();
        _barramentoDeEventos.Publicar(new SaltoGravitacionalConcluido(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new SegmentoDeViagemConcluido(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new ObjetivoAlterado("SEGMENTO CONCLUIDO. Pressione CONFIRMAR na estacao para reiniciar.", DateTime.UtcNow));
    }

    private void ReiniciarSegmento()
    {
        _tempoDeSalto = 0f;
        _saltoConcluidoPublicado = false;
        _estadoDaNave.ReiniciarParaTeste();
        _segmentoDeViagem.Iniciar();
        _controladorDeVoo?.ZerarVelocidade();
        _controladorDeVoo?.RecentralizarCamera();

        if (_seguidorDaNave is not null)
        {
            _seguidorDaNave.ProgressRatio = 0f;
        }

        _barramentoDeEventos.Publicar(new PerfilDeExecucaoCarregado(_configuracaoDeExecucao.Perfil.ToString(), DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new SegmentoDeViagemReiniciado(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new SegmentoDeViagemIniciado(DateTime.UtcNow));
        _barramentoDeEventos.Publicar(new ObjetivoAlterado("Avance pela rota de teste.", DateTime.UtcNow));
    }

    private void AnimarCenario(float delta)
    {
        foreach (var objeto in _objetosDeCenario)
        {
            objeto.RotateY(delta * 0.08f);
        }
    }

    private static void RegistrarEventoImportante(EventoDeDominio eventoDeDominio)
    {
        switch (eventoDeDominio)
        {
            case SegmentoDeViagemIniciado:
                GD.Print("[ROTA] Segmento iniciado");
                break;
            case AproximacaoCometa:
                GD.Print("[ROTA] AproximacaoCometa");
                break;
            case PassagemCometa:
                GD.Print("[ROTA] PassagemCometa");
                break;
            case AvisoDeSalto:
                GD.Print("[ROTA] AvisoSalto");
                break;
            case SaltoGravitacionalIniciado:
                GD.Print("[SALTO] Iniciado");
                break;
            case SaltoGravitacionalConcluido:
                GD.Print("[SALTO] Concluido");
                break;
            case SegmentoDeViagemReiniciado:
                GD.Print("[ROTA] Reiniciado");
                break;
        }
    }
}
