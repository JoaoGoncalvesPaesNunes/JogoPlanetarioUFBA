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
        AddChild(new WorldEnvironment
        {
            Name = "AmbienteOnRails",
            Environment = new Godot.Environment
            {
                BackgroundMode = Godot.Environment.BGMode.Color,
                BackgroundColor = new Color(0.003f, 0.005f, 0.012f),
                AmbientLightSource = Godot.Environment.AmbientSource.Color,
                AmbientLightColor = new Color(0.08f, 0.1f, 0.14f),
                AmbientLightEnergy = 0.5f
            }
        });

        AddChild(new DirectionalLight3D
        {
            Name = "LuzPrincipal",
            Rotation = new Vector3(-0.7f, -0.65f, 0.1f),
            LightEnergy = 2.1f
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
        var materialEstrela = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.86f, 0.94f, 1f),
            EmissionEnabled = true,
            Emission = new Color(0.45f, 0.65f, 0.9f),
            EmissionEnergyMultiplier = 0.7f
        };

        for (var indice = 0; indice < 140; indice++)
        {
            var estrela = new MeshInstance3D
            {
                Name = $"EstrelaOnRails{indice:000}",
                Mesh = new SphereMesh { Radius = 0.018f + GD.Randf() * 0.018f, Height = 0.04f },
                Position = new Vector3(GD.Randf() * 46f - 23f, GD.Randf() * 22f - 11f, -GD.Randf() * 46f - 4f),
                MaterialOverride = materialEstrela
            };
            AddChild(estrela);
            _objetosDeCenario.Add(estrela);
        }

        var planeta = new MeshInstance3D
        {
            Name = "PlanetaDistanteGraybox",
            Position = new Vector3(-8f, 4f, -34f),
            Mesh = new SphereMesh { Radius = 3.2f, Height = 6.4f },
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = new Color(0.2f, 0.38f, 0.78f),
                Roughness = 0.9f
            }
        };
        AddChild(planeta);
    }

    private void CriarCometa()
    {
        var nucleo = new MeshInstance3D
        {
            Name = "CometaPlaceholder",
            Position = new Vector3(4.2f, 1.1f, -20f),
            Scale = new Vector3(1.3f, 0.85f, 1f),
            Mesh = new SphereMesh { Radius = 0.75f, Height = 1.5f },
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = new Color(0.72f, 0.72f, 0.68f),
                Roughness = 1f
            }
        };
        AddChild(nucleo);

        var cauda = new MeshInstance3D
        {
            Name = "CaudaCometaPlaceholder",
            Position = new Vector3(6.1f, 1.1f, -20.5f),
            Rotation = new Vector3(0f, 0.2f, Mathf.Pi / 2f),
            Mesh = new CylinderMesh
            {
                TopRadius = 0.08f,
                BottomRadius = 0.72f,
                Height = 4.2f
            },
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = new Color(0.55f, 0.88f, 1f, 0.46f),
                Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
                EmissionEnabled = true,
                Emission = new Color(0.25f, 0.7f, 1f),
                EmissionEnergyMultiplier = 0.65f
            }
        };
        AddChild(cauda);
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
