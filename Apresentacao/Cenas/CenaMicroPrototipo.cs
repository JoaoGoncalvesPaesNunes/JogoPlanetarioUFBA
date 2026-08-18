using System;
using Godot;
using ProjetoPlanetario.Apresentacao.HUD;
using ProjetoPlanetario.Dados.Nave;
using ProjetoPlanetario.Entrada.Adaptadores;
using ProjetoPlanetario.Entrada.Modelos;
using ProjetoPlanetario.Entrada.Roteamento;
using ProjetoPlanetario.Jogabilidade.Voo;
using ProjetoPlanetario.Narrativa.Diretor;
using ProjetoPlanetario.Nucleo.Configuracao;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;
using ProjetoPlanetario.Projecao.Adaptadores;
using ProjetoPlanetario.Publico.Estacoes;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public partial class CenaMicroPrototipo : Node3D
{
	private readonly BarramentoDeEventos _barramentoDeEventos = new();
	private readonly EstadoDaNave _estadoDaNave = new();
	private readonly ConfiguracaoDeExecucao _configuracaoDeExecucao = ConfiguracaoDeExecucao.CriarDesktopDev();
	private readonly ConfiguracaoDaNave _configuracaoDaNave = new();
	private CockpitDoPiloto? _cockpitDoPiloto;

	public override void _Ready()
	{
		ConfiguradorInputMap.GarantirAcoesDesktopDev();

		var diretorNarrativo = new DiretorNarrativo(_barramentoDeEventos);
		var portaDeProjecao = new AdaptadorProjecaoDesktop(new Rect2(Vector2.Zero, GetViewport().GetVisibleRect().Size));
		portaDeProjecao.AplicarEmViewport(GetViewport());

		CriarLuz();
		CriarFundoEspacial();
		var controladorDeVoo = CriarNave();
		CriarCenarioDeRota();
		CriarCockpit(controladorDeVoo);
		CriarHud(diretorNarrativo);
		CriarEstacaoVirtual();
		_barramentoDeEventos.EventoPublicado += AplicarRespostaDaEstacaoDeTeste;

		_barramentoDeEventos.Publicar(new PerfilDeExecucaoCarregado(_configuracaoDeExecucao.Perfil.ToString(), DateTime.UtcNow));
	}

	private void CriarLuz()
	{
		var luz = new DirectionalLight3D
		{
			Name = "LuzDeTeste",
			Rotation = new Vector3(-0.8f, -0.4f, 0f),
			LightEnergy = 1.8f
		};
		AddChild(luz);
	}

	private void CriarFundoEspacial()
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

		var material = new StandardMaterial3D
		{
			AlbedoColor = new Color(0.8f, 0.95f, 1f)
		};

		for (var indice = 0; indice < 70; indice++)
		{
			var estrela = new MeshInstance3D
			{
				Name = $"Estrela{indice:00}",
				Mesh = new SphereMesh
				{
					Radius = 0.025f,
					Height = 0.05f
				},
				Position = new Vector3(
					GD.Randf() * 44f - 22f,
					GD.Randf() * 22f - 11f,
					-GD.Randf() * 30f - 6f),
				MaterialOverride = material
			};

			AddChild(estrela);
		}
	}

	private ControladorDeVoo CriarNave()
	{
		var entradaDoPiloto = new AdaptadorTecladoMouse();
		var nave = new ControladorDeVoo
		{
			Name = "CockpitDoPiloto3D"
		};

		var camera = new Camera3D
		{
			Name = "CameraPrimeiraPessoa",
			Current = true,
			Fov = 72f,
			Near = 0.05f
		};

		nave.AddChild(camera);
		nave.Configurar(entradaDoPiloto, _estadoDaNave, _barramentoDeEventos, _configuracaoDaNave);
		AddChild(nave);
		return nave;
	}

	private void CriarCenarioDeRota()
	{
		var planeta = new MeshInstance3D
		{
			Name = "PlanetaPlaceholder",
			Position = new Vector3(-6.8f, 2.1f, -18f),
			Mesh = new SphereMesh
			{
				Radius = 2.2f,
				Height = 4.4f
			},
			MaterialOverride = new StandardMaterial3D
			{
				AlbedoColor = new Color(0.95f, 0.48f, 0.22f),
				Roughness = 0.8f
			}
		};
		AddChild(planeta);

		var pontoDeDobra = new MeshInstance3D
		{
			Name = "PontoDeDobraPlaceholder",
			Position = new Vector3(2.4f, -0.9f, -9f),
			Mesh = new TorusMesh
			{
				InnerRadius = 0.38f,
				OuterRadius = 0.46f
			},
			MaterialOverride = new StandardMaterial3D
			{
				AlbedoColor = new Color(0.1f, 0.95f, 1f),
				EmissionEnabled = true,
				Emission = new Color(0.1f, 0.7f, 1f),
				EmissionEnergyMultiplier = 1.8f
			}
		};
		AddChild(pontoDeDobra);

		var materialAsteroide = new StandardMaterial3D
		{
			AlbedoColor = new Color(0.45f, 0.42f, 0.37f),
			Roughness = 1f
		};

		for (var indice = 0; indice < 12; indice++)
		{
			var asteroide = new MeshInstance3D
			{
				Name = $"Asteroide{indice:00}",
				Position = new Vector3(
					GD.Randf() * 18f - 9f,
					GD.Randf() * 7f - 3.5f,
					-GD.Randf() * 16f - 7f),
				Rotation = new Vector3(GD.Randf() * 3f, GD.Randf() * 3f, GD.Randf() * 3f),
				Scale = Vector3.One * (0.18f + GD.Randf() * 0.45f),
				Mesh = new BoxMesh(),
				MaterialOverride = materialAsteroide
			};
			AddChild(asteroide);
		}
	}

	private void CriarCockpit(ControladorDeVoo controladorDeVoo)
	{
		_cockpitDoPiloto = new CockpitDoPiloto
		{
			Name = "CockpitVisual"
		};
		_cockpitDoPiloto.Configurar(controladorDeVoo, _estadoDaNave);
		AddChild(_cockpitDoPiloto);
	}

	private void CriarHud(DiretorNarrativo diretorNarrativo)
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

	private void AplicarRespostaDaEstacaoDeTeste(EventoDeDominio eventoDeDominio)
	{
		if (eventoDeDominio is not EstacaoAcaoExecutada evento)
		{
			return;
		}

		if (evento.NomeDaAcao == AcaoLogicaDaEstacao.Esquerda.ToString())
		{
			_cockpitDoPiloto?.DefinirRotaAtual("Marte");
			_barramentoDeEventos.Publicar(new RotaSelecionada("Marte", DateTime.UtcNow));
		}

		if (evento.NomeDaAcao == AcaoLogicaDaEstacao.Direita.ToString())
		{
			_cockpitDoPiloto?.DefinirRotaAtual("Jupiter");
			_barramentoDeEventos.Publicar(new RotaSelecionada("Jupiter", DateTime.UtcNow));
		}

		if (evento.NomeDaAcao == AcaoLogicaDaEstacao.Acao1.ToString() && _estadoDaNave.ConsumirEnergia(2f))
		{
			_barramentoDeEventos.Publicar(new EnergiaAlterada(_estadoDaNave.Energia, "EstacaoVirtualAcao1", DateTime.UtcNow));
			if (_estadoDaNave.AlterarEstabilidadeDaDobra(8f))
			{
				_barramentoDeEventos.Publicar(new EstabilidadeDaDobraAlterada(_estadoDaNave.EstabilidadeDaDobra, "TripulacaoAuxiliouDobra", DateTime.UtcNow));
			}
		}

		if (evento.NomeDaAcao == AcaoLogicaDaEstacao.Acao2.ToString())
		{
			if (_estadoDaNave.RestaurarEnergia(5f))
			{
				_barramentoDeEventos.Publicar(new EnergiaAlterada(_estadoDaNave.Energia, "RedistribuicaoDaTripulacao", DateTime.UtcNow));
			}

			if (_estadoDaNave.AlterarEstabilidadeDaDobra(-3f))
			{
				_barramentoDeEventos.Publicar(new EstabilidadeDaDobraAlterada(_estadoDaNave.EstabilidadeDaDobra, "RedistribuicaoDaTripulacao", DateTime.UtcNow));
			}
		}
	}
}
