# Guia de desenvolvimento da versao robusta

Comece pelo [README](../README.md). Os caminhos deste guia sao relativos a raiz
clonada; nenhuma pasta E: ou C: especifica e necessaria. Documentos historicos
podem citar o computador original, mas esses caminhos nao sao dependencias.

## Ambiente e estrutura

Godot 4.7.1 .NET, Godot.NET.Sdk/4.7.1, alvo net8.0, SDK testado 10.0.400.
O teste de console roda em Microsoft.NETCore.App 8 (localmente 8.0.13).
`JogoPlanetarioUFBA.sln` inclui o jogo e `Testes/TestesDominio.csproj`.
O build restaura pacotes .NET; nao copie DLLs ou .godot de outro computador.

| Pasta | Responsabilidade |
| --- | --- |
| Robusto/Cenas | Experiencia persistente, cinco ambientes e prefab Pirata |
| Robusto/Dominio | SequenciaDaViagem, EstadoDoOlhar, ManobraDoGelo: regras C# puras |
| Robusto/Viagem | DiretorDaViagem, PilotoAutomatico, ControladorDoOlhar, AudioDaViagem |
| Robusto/Entrada | EntradaDaViagem: bindings e leitura logica |
| Robusto/Eventos | CombateDeMarte e EventoDoGelo |
| Robusto/Projecao | GerenciadorExibicao, configuracao, grade e shaders |
| Robusto/UI | InterfaceDaViagem: HUD e menu |
| Robusto/Cenarios | CenarioDaViagem e AparenciaAnaBranca |
| Robusto/Dados | Catalogo, Exibicao e roteiro JSON |
| Apresentacao/Cenarios | GerenciadorCenarios, CenarioBase, ambiente e legado |
| Apresentacao/Cenas | InteriorDaNave, TrajetoriaSuave e cenas antigas |
| Apresentacao/HUD | ApresentacaoDoSalto e componentes herdados |
| Jogabilidade/Viagem | ControladorDoSalto e regras herdadas |
| Projecao | IPortaDeProjecao e adaptadores legados |
| Entrada, Nucleo, Dados, Narrativa, Publico | Contratos e sistemas herdados |
| assets | Modelos, mapas e panorama, com creditos |
| Testes | Regras puras, integracao C# e validador visual legado GDScript |

Nao ha Autoloads registrados em project.godot nem plugins/addons requeridos.
A linguagem principal e C#; efeitos usam Godot Shader Language e um teste
legado usa GDScript. Renderer gl_compatibility, viewport base1280x720, minimo960x540,
stretch canvas_items. O gerenciador de exibicao define fullscreen em runtime.

## Fluxo e sistemas persistentes

`DiretorDaViagem` e o controlador da viagem, nao uma classe chamada JourneyController.
Instanciado por Experiencia.tscn, carrega roteiro, configura entrada e integra as
regras. `SequenciaDaViagem` valida IDs unicos, duracoes positivas e tipos:
observacao, salto, combate, gelo, chegada. A ordem e a ordem do JSON.

`GerenciadorCenarios` e o gerente real, reaproveitado de Apresentacao/Cenarios.
Troca o ambiente em RecipienteCenarios e valida o catalogo antes de retirar o
anterior. Nave, CameraRig, audio, HUD e sistemas continuam vivos. Catalogo ativo:
Robusto/Dados/Catalogo.tres. Nao confundir com Dados/Cenarios/CatalogoCenarios.tres,
utilizado pelo legado. Nao duplique gerentes ou mova regras para Autoloads.

## Camera e cockpit

Hierarquia: Experiencia/Nave/CameraRig/CameraPiloto. ControladorDoOlhar altera
apenas a orientacao local do rig, sem dirigir a nave. EstadoDoOlhar limita yaw/pitch,
velocidade angular e suavizacao. Campos exportados ficam no CameraRig:
SensibilidadeMouse, Suavizacao, VelocidadeAnalogico, VelocidadeMaxima, LimiteYaw,
PitchMinimo, PitchMaximo, InverterY e opcoes de recentralizacao.

DiretorDaViagem escolhe ContextoDeEntrada: Olhar, Combate, Gelo, Menu ou Bloqueado.
Nao leia o stick em um sistema novo sem respeitar esse contexto. Olhar usa tempo
real, nao Ritmo acelerado de testes. Menu libera cursor; chegada tambem.

InteriorDaNave gera meshes em runtime. CabineCompacta=true na experiencia conserva
painel/vidros e fecha a traseira, sem bancos. Nao e um modelo externo editavel por
partes no editor. Nao reintroduza a cabine antiga ao ajustar a camera.

## Piloto automatico, rotas e salto

PilotoAutomatico amostra a Curve3D de Rota por distancia e usa TrajetoriaSuave
para orientacao. Desacelera perto do fim e interpola a base. Desvio permite a
manobra lateral do gelo. Nenhuma tecla controla diretamente a curva da nave.

Para editar: selecione Rota no ambiente, mova pontos e alcas, ajuste o marcador
InicioEvento em Marte/Urano e teste a etapa inteira. Velocidade fica no JSON.
Nao reutilize os controles W/S de avancar/recuar do prototipo legado.

ControladorDoSalto mantem as fases; ApresentacaoDoSalto renderiza os rastros em
MultiMesh. O cenario e carregado durante o tunel. O carregamento e sincrono;
se falhar, o diretor exibe o erro e permite tentar novamente por CONTINUAR.
Nao troque manualmente o cenario no meio do salto.

## Eventos

**Marte:** CombateDeMarte instancia Pirata.tscn nos marcadores em
Jogabilidade/PontosDeAparecimento. Disparo hitscan usa o raio de GerenciadorExibicao;
nao use Camera.ProjectRayNormal diretamente se o codigo tambem rodar em Fulldome.
QuantidadePiratas, RaioAcerto e IntervaloDisparo estao no Inspector. EscudoSolicitado,
EscudoAtivado e AtivarEscudo sao ganchos para expansao; nao existe rede de plateia.

**Urano:** EventoDoGelo adapta ManobraDoGelo (regra pura). Mundo/Objetos/Gelo
e os marcadores AlertaGelo/FimGelo definem a passagem. A correcao sustentada
desencadeia evasao e retorno automaticos. Parametros de tempo/desvio ficam na
raiz de Urano.tscn. Preservar a espera segura quando nao ha entrada.

**Ana Branca:** ancora Mundo/CorposCelestes/Planeta com Superficie, Halo,
LuzEstelar e CentroDoPlaneta. AparenciaAnaBranca permite Emissao, ForcaHalo,
EnergiaLuz no Inspector. SuperficieEstelar.gdshader gera ruido e fluxo;
HaloEstelar.gdshader e uma casca aditiva. RotacaoDoPlaneta na raiz e rad/s.
Nao e simulacao cientifica e nao ha textura externa dessa estrela.

## Exibicao e Fulldome

GerenciadorExibicao implementa IPortaDeProjecao. Normal renderiza a camera;
Fulldome usa cinco vistas com a mesma origem/orientacao de referencia e fisheye.
A mira usa a transformacao inversa. ConfiguracaoExibicao vem de Exibicao.tres;
menu altera uma copia da sessao. Nao presuma persistencia do menu no arquivo.

Fisheye ate180 graus, saida padrao1024, area segura de UI. F9 mostra grade sobre
a saida; nao e calibracao fisica. Leia FULLDOME.md antes de mexer em FOV,
espelhos, rotacao ou resolucao. Nenhum warp/blending esta implementado.

## Roteiro e audio

Edite Titulos/Textos no JSON sem mudar IDs/tipos para simples revisao de falas.
AudioDaViagem usa AudioStream indicado em Audio e encerra a fala anterior ao
trocar de etapa. Nao ha dublagem ou trilha fornecida. A duracao nao e calculada
pelo audio. Ao acrescentar recursos, use res:// e documente a licenca.

## Execucao e testes portaveis

No PowerShell, abra a raiz clonada. Substitua o caminho solicitado pelo executavel
da Godot .NET instalado na SUA maquina:

```powershell
$godot = Read-Host 'Caminho completo do executavel Godot 4.7.1 .NET'
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
& $godot --headless --editor --import --path .
& $godot --path . -- --janela
```

Execute os testes abaixo apos fechar o jogo interativo:

```powershell
& $godot --headless --path . --fixed-fps 60 res://Testes/Integracao/ValidarRobusto.tscn -- --janela
& $godot --headless --path . --fixed-fps 60 res://Testes/Integracao/ValidarVisualCamera.tscn -- --janela
& $godot --path . --fixed-fps 60 --disable-vsync res://Testes/Integracao/ValidarVisualCamera.tscn -- --janela
& $godot --path . --fixed-fps 60 --disable-vsync res://Testes/Integracao/ValidarRobusto.tscn -- --janela
```

Testes graficos controlam janela/mouse e encerram sozinhos. Capturas ficam em
.godot/validacao_visual_camera e .godot/validacao_robusta. Nao medir desempenho
real com fixed-fps. ValidarCenarios.tscn/ValidarSalto.tscn cobrem componentes
legados. Os executaveis de testes nao sao um pacote exportado para usuarios finais.

Debug: -- --etapa=piratas inicia um evento; -- --fulldome --janela testa projecao;
-- --calibracao abre grade/menu. Atalhos so valem com foco no jogo, em debug.
Mantenha Ritmo=1 para apresentacoes. Controles completos no README/DEBUG_GUIDE.md.

## Git, assets e colaboracao

Versione .tscn, .tres, .cs, .gd, .gdshader, .uid, .import junto aos assets fonte,
JSON, csproj/sln e documentacao. Arquivos .import sao receitas, nao o cache .godot.
Nao envie .godot, bin/obj, builds, ZIPs de download, .env ou credenciais.
export_presets.cfg continua ignorado pela regra herdada; nao ha preset de
distribuicao validado. Ao criar um, revisar segredos/caminhos antes de versionar.

Nao incluir executavel Godot/.NET SDK no repositorio. Nao ha submodulos ou LFS.
Manter ASSET_SOURCES.md e licencas locais. Codigo sem licenca geral definida:
nao inferir que licencas dos assets se aplicam a todo o projeto.

Crie branches de trabalho, revise git diff/status e execute build/testes antes
de commit. Nao publique caches e nao sobrescreva main com force push.
Use identificadores C# em portugues sem acentos, regras puras testaveis e
hardware desacoplado, conforme AGENTS.md e CONTRIBUTING.md.
