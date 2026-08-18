using System;
using Godot;
using ProjetoPlanetario.Apresentacao.HUD;
using ProjetoPlanetario.Dados.Nave;
using ProjetoPlanetario.Entrada.Adaptadores;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Roteamento;
using ProjetoPlanetario.Jogabilidade.Eventos;
using ProjetoPlanetario.Jogabilidade.Voo;
using ProjetoPlanetario.Narrativa.Diretor;
using ProjetoPlanetario.Nucleo.Configuracao;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;
using ProjetoPlanetario.Projecao.Adaptadores;
using ProjetoPlanetario.Publico.Estacoes;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public partial class CenaCockpitLoopCentral : Node3D
{
    private readonly BarramentoDeEventos _barramentoDeEventos = new();
    private readonly EstadoDaNave _estadoDaNave = new();
    private readonly ConfiguracaoDeExecucao _configuracaoDeExecucao = ConfiguracaoDeExecucao.CriarDesktopDev();
    private readonly ConfiguracaoDaNave _configuracaoDaNave = new();
    private readonly Vector2[] _pontosDeDobra =
    {
        new(0.36f, 0.18f),
        new(-0.42f, -0.2f)
    };

    private ControladorDeVoo? _controladorDeVoo;
    private ControladorDoLoopCentral? _controladorDoLoopCentral;
    private CockpitDoPiloto? _cockpitDoPiloto;
    private MeshInstance3D? _marcadorDeWaypoint;
    private int _ultimoWaypointAplicado;

    public override void _Ready()
    {
        ConfiguradorInputMap.GarantirAcoesDesktopDev();

        var diretorNarrativo = new DiretorNarrativo(_barramentoDeEventos);
        var portaDeProjecao = new AdaptadorProjecaoDesktop(new Rect2(Vector2.Zero, GetViewport().GetVisibleRect().Size));
        portaDeProjecao.AplicarEmViewport(GetViewport());

        _controladorDoLoopCentral = new ControladorDoLoopCentral(_estadoDaNave, _barramentoDeEventos);
        _barramentoDeEventos.EventoPublicado += AoEventoPublicado;

        CriarIluminacao();
        CriarFundoEspacial();
        CriarObjetosDeTeste();
        _controladorDeVoo = CriarCockpit3D();
        CriarWaypointVisual();
        CriarCockpitVisual(diretorNarrativo);
        CriarHudDebug(diretorNarrativo);
        CriarEstacaoVirtual();

        _barramentoDeEventos.Publicar(new PerfilDeExecucaoCarregado(_configuracaoDeExecucao.Perfil.ToString(), DateTime.UtcNow));
        _controladorDoLoopCentral.Iniciar();
        AtualizarWaypointAtivo();
    }

    public override void _Process(double delta)
    {
        if (_controladorDeVoo is null || _controladorDoLoopCentral is null)
        {
            return;
        }

        _controladorDoLoopCentral.AtualizarNavegacao(_controladorDeVoo.AlinhamentoDaRota, (float)delta);

        if (_marcadorDeWaypoint is not null)
        {
            _marcadorDeWaypoint.RotateZ((float)delta * 1.2f);
            var pulso = 1f + Mathf.Sin(Time.GetTicksMsec() / 220.0f) * 0.08f;
            _marcadorDeWaypoint.Scale = Vector3.One * pulso;
        }
    }

    private void CriarIluminacao()
    {
        var ambiente = new WorldEnvironment
        {
            Name = "AmbienteEspacial",
            Environment = new Godot.Environment
            {
                BackgroundMode = Godot.Environment.BGMode.Color,
                BackgroundColor = new Color(0.005f, 0.007f, 0.015f),
                AmbientLightSource = Godot.Environment.AmbientSource.Color,
                AmbientLightColor = new Color(0.08f, 0.1f, 0.13f),
                AmbientLightEnergy = 0.55f
            }
        };
        AddChild(ambiente);

        var luz = new DirectionalLight3D
        {
            Name = "LuzSolarDeTeste",
            Rotation = new Vector3(-0.85f, -0.55f, 0f),
            LightEnergy = 1.9f
        };
        AddChild(luz);
    }

    private void CriarFundoEspacial()
    {
        var material = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.84f, 0.94f, 1f),
            EmissionEnabled = true,
            Emission = new Color(0.35f, 0.55f, 0.8f),
            EmissionEnergyMultiplier = 0.6f
        };

        for (var indice = 0; indice < 120; indice++)
        {
            var estrela = new MeshInstance3D
            {
                Name = $"EstrelaLoop{indice:000}",
                Mesh = new SphereMesh
                {
                    Radius = 0.018f + GD.Randf() * 0.018f,
                    Height = 0.04f
                },
                Position = new Vector3(
                    GD.Randf() * 52f - 26f,
                    GD.Randf() * 24f - 12f,
                    -GD.Randf() * 34f - 7f),
                MaterialOverride = material
            };

            AddChild(estrela);
        }
    }

    private void CriarObjetosDeTeste()
    {
        var planeta = new MeshInstance3D
        {
            Name = "PlanetaGraybox",
            Position = new Vector3(-7f, 2.4f, -19f),
            Mesh = new SphereMesh
            {
                Radius = 2.4f,
                Height = 4.8f
            },
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = new Color(0.92f, 0.42f, 0.22f),
                Roughness = 0.85f
            }
        };
        AddChild(planeta);

        var materialAsteroide = new StandardMaterial3D
        {
            AlbedoColor = new Color(0.42f, 0.4f, 0.36f),
            Roughness = 1f
        };

        for (var indice = 0; indice < 14; indice++)
        {
            var asteroide = new MeshInstance3D
            {
                Name = $"AsteroideGraybox{indice:00}",
                Position = new Vector3(
                    GD.Randf() * 18f - 9f,
                    GD.Randf() * 8f - 4f,
                    -GD.Randf() * 18f - 8f),
                Rotation = new Vector3(GD.Randf() * 3f, GD.Randf() * 3f, GD.Randf() * 3f),
                Scale = Vector3.One * (0.16f + GD.Randf() * 0.42f),
                Mesh = new BoxMesh(),
                MaterialOverride = materialAsteroide
            };
            AddChild(asteroide);
        }
    }

    private ControladorDeVoo CriarCockpit3D()
    {
        var controlador = new ControladorDeVoo
        {
            Name = "ControladorPrimeiraPessoa"
        };

        var camera = new Camera3D
        {
            Name = "CameraPiloto",
            Current = true,
            Fov = 74f,
            Near = 0.05f
        };

        controlador.AddChild(camera);
        controlador.Configurar(new AdaptadorTecladoMouse(), _estadoDaNave, _barramentoDeEventos, _configuracaoDaNave);
        AddChild(controlador);
        return controlador;
    }

    private void CriarWaypointVisual()
    {
        _marcadorDeWaypoint = new MeshInstance3D
        {
            Name = "WaypointAtivo",
            Mesh = new TorusMesh
            {
                InnerRadius = 0.34f,
                OuterRadius = 0.48f
            },
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = new Color(0.1f, 0.95f, 1f),
                EmissionEnabled = true,
                Emission = new Color(0.1f, 0.85f, 1f),
                EmissionEnergyMultiplier = 2.2f
            }
        };
        AddChild(_marcadorDeWaypoint);
    }

    private void CriarCockpitVisual(DiretorNarrativo diretorNarrativo)
    {
        if (_controladorDeVoo is null || _controladorDoLoopCentral is null)
        {
            return;
        }

        _cockpitDoPiloto = new CockpitDoPiloto
        {
            Name = "CockpitGraybox"
        };
        _cockpitDoPiloto.Configurar(_controladorDeVoo, _estadoDaNave, _controladorDoLoopCentral, diretorNarrativo);
        AddChild(_cockpitDoPiloto);
    }

    private void CriarHudDebug(DiretorNarrativo diretorNarrativo)
    {
        var hud = new HUDDeDebug
        {
            Name = "HUDDeDebug"
        };
        hud.Configurar(_estadoDaNave, _configuracaoDeExecucao, diretorNarrativo, _barramentoDeEventos);
        AddChild(hud);
    }

    private void CriarEstacaoVirtual()
    {
        var adaptador = new AdaptadorEntradaVirtual("Estacao1", _barramentoDeEventos);
        var estacao = new EstacaoVirtual
        {
            Name = "EstacaoVirtual"
        };
        estacao.Configurar(adaptador);
        AddChild(estacao);
    }

    private void AoEventoPublicado(EventoDeDominio eventoDeDominio)
    {
        if (_controladorDoLoopCentral is null)
        {
            return;
        }

        RegistrarEventoImportante(eventoDeDominio);

        if (eventoDeDominio is LoopCentralReiniciado)
        {
            _ultimoWaypointAplicado = 0;
            AtualizarWaypointAtivo();
        }

        if (eventoDeDominio is ProgressoDoWaypointAlterado progresso &&
            progresso.NumeroDoWaypoint != _ultimoWaypointAplicado)
        {
            AtualizarWaypointAtivo();
        }

        if (eventoDeDominio is EstacaoAcaoExecutada eventoDeEstacao &&
            Enum.TryParse<AcaoLogicaDaEstacao>(eventoDeEstacao.NomeDaAcao, out var acao))
        {
            _controladorDoLoopCentral.ReceberAcaoDaEstacao(acao);
        }
    }

    private void AtualizarWaypointAtivo()
    {
        if (_controladorDeVoo is null || _controladorDoLoopCentral is null || _marcadorDeWaypoint is null)
        {
            return;
        }

        var indice = Math.Clamp(_controladorDoLoopCentral.NumeroDoWaypoint - 1, 0, _pontosDeDobra.Length - 1);
        var ponto = _pontosDeDobra[indice];
        _ultimoWaypointAplicado = _controladorDoLoopCentral.NumeroDoWaypoint;
        _controladorDeVoo.DefinirPontoDeDobra(ponto);
        _marcadorDeWaypoint.Position = new Vector3(ponto.X * 8f, -ponto.Y * 5f, -10f);
    }

    private static void RegistrarEventoImportante(EventoDeDominio eventoDeDominio)
    {
        switch (eventoDeDominio)
        {
            case SessaoDeTesteIniciada:
                GD.Print("[LOOP] Sessao iniciada");
                break;
            case ObjetivoAlterado evento:
                GD.Print($"[OBJETIVO] {evento.TextoDoObjetivo}");
                break;
            case WaypointAlcancado evento:
                GD.Print($"[EVENTO] Waypoint {evento.NumeroDoWaypoint} alcancado");
                break;
            case CalibracaoIniciada evento:
                GD.Print($"[EVENTO] Calibracao {evento.NumeroDoWaypoint} iniciada");
                break;
            case EstacaoAcaoExecutada evento:
                GD.Print($"[ESTACAO] {evento.IdentificadorDaEstacao} enviou {evento.NomeDaAcao}");
                break;
            case CargaDeDobraAlterada evento:
                GD.Print($"[ESTADO] Carga de dobra {evento.CargaAtual:0}%");
                break;
            case LoopCentralConcluido:
                GD.Print("[LOOP] Concluido");
                break;
        }
    }
}
