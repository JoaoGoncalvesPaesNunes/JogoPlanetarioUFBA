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

	public override void _Ready()
	{
		ConfiguradorInputMap.GarantirAcoesDesktopDev();

		var diretorNarrativo = new DiretorNarrativo(_barramentoDeEventos);
		var portaDeProjecao = new AdaptadorProjecaoDesktop(new Rect2(Vector2.Zero, GetViewport().GetVisibleRect().Size));
		portaDeProjecao.AplicarEmViewport(GetViewport());

		CriarCamera();
		CriarLuz();
		CriarFundoEspacial();
		CriarNave();
		CriarAlvoDeTeste();
		CriarHud(diretorNarrativo);
		CriarEstacaoVirtual();
		_barramentoDeEventos.EventoPublicado += AplicarRespostaDaEstacaoDeTeste;

		_barramentoDeEventos.Publicar(new PerfilDeExecucaoCarregado(_configuracaoDeExecucao.Perfil.ToString(), DateTime.UtcNow));
	}

	private void CriarCamera()
	{
		var camera = new Camera3D
		{
			Name = "CameraDesktop",
			Position = new Vector3(0f, 0f, 18f),
			Current = true,
			Fov = 60f
		};
		AddChild(camera);
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
					GD.Randf() * 26f - 13f,
					GD.Randf() * 14f - 7f,
					-GD.Randf() * 18f - 4f),
				MaterialOverride = material
			};

			AddChild(estrela);
		}
	}

	private void CriarNave()
	{
		var entradaDoPiloto = new AdaptadorTecladoMouse();
		var nave = new ControladorDeVoo
		{
			Name = "NaveDoPiloto"
		};

		var corpo = new MeshInstance3D
		{
			Name = "CorpoPlaceholder",
			Mesh = new BoxMesh
			{
				Size = new Vector3(1.2f, 0.45f, 1.8f)
			},
			MaterialOverride = new StandardMaterial3D
			{
				AlbedoColor = new Color(0.2f, 0.75f, 0.9f)
			}
		};

		var bico = new MeshInstance3D
		{
			Name = "BicoPlaceholder",
			Position = new Vector3(0f, 0f, -1.15f),
			Mesh = new PrismMesh
			{
				Size = new Vector3(0.9f, 0.5f, 0.9f)
			},
			MaterialOverride = new StandardMaterial3D
			{
				AlbedoColor = new Color(1f, 0.86f, 0.28f)
			}
		};

		nave.AddChild(corpo);
		nave.AddChild(bico);
		nave.Configurar(entradaDoPiloto, _estadoDaNave, _barramentoDeEventos, _configuracaoDaNave);
		AddChild(nave);
	}

	private void CriarAlvoDeTeste()
	{
		var alvo = new MeshInstance3D
		{
			Name = "AlvoDeTeste",
			Position = new Vector3(4.5f, -1.8f, -6f),
			Mesh = new SphereMesh
			{
				Radius = 0.55f,
				Height = 1.1f
			},
			MaterialOverride = new StandardMaterial3D
			{
				AlbedoColor = new Color(1f, 0.35f, 0.25f)
			}
		};

		AddChild(alvo);
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

		if (evento.NomeDaAcao == AcaoLogicaDaEstacao.Acao1.ToString() && _estadoDaNave.ConsumirEnergia(2f))
		{
			_barramentoDeEventos.Publicar(new EnergiaAlterada(_estadoDaNave.Energia, "EstacaoVirtualAcao1", DateTime.UtcNow));
		}
	}
}
