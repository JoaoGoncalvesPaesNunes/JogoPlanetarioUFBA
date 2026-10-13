# Rodada visual e camera - 2026-10-07

## Projeto e preservacao

Pasta modificada: `E:\Godot\GAME_robusto`. Nao foi criada outra copia.
Abra `E:\Godot\GAME_robusto\project.godot` na Godot 4.7.1 .NET, compile e use F5.
Cena principal: `Robusto/Cenas/Experiencia.tscn`.

Nenhum arquivo de conteudo do original `E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA`
foi alterado por esta rodada. Comparacao com o manifesto de 891 arquivos da etapa
anterior encontrou apenas tres divergencias: `.godot/editor/editor_layout.cfg`,
`.godot/editor/filesystem_cache10` e um registro interno `.git/refs/codex/turn-diffs`.
Sao caches/metadados, nao foram restaurados ou editados por esta implementacao.
Nao se afirma identidade byte a byte desses caches. Codigo, cenas e assets
manifestados do original permanecem iguais.

Branch da copia: `codex/prototipo-robusto`. Alteracoes anteriores preservadas;
nenhum commit ou push nesta rodada. O roteiro, curvas, piloto automatico,
combate, gelo, salto e gerenciador de projecao nao foram reimplementados.

## Camera e entrada

O CameraRig existente ganhou ControladorDoOlhar. Apenas sua rotacao local muda;
a nave permanece on-rails, sem direcao, aceleracao ou translacao pelo jogador.
CameraPiloto continua no mesmo caminho; ponto de vista do rig: (0,0.12,0.1).
EstadoDoOlhar e C# puro e testavel. Nenhum Autoload novo.

| Ajuste exportado no CameraRig | Padrao |
| --- | --- |
| SensibilidadeMouse | 0.08 graus/pixel |
| Suavizacao | 7 |
| VelocidadeAnalogico | 30 graus/s |
| VelocidadeMaxima | 40 graus/s, inclusive diagonais |
| LimiteYaw | -115 a +115 graus |
| PitchMinimo / PitchMaximo | -50 / +70 graus |
| InverterY | false |
| RecentralizarAutomaticamente | false |
| EsperaRecentralizacao | 8 segundos, se habilitada |
| VelocidadeRecentralizacao | 32 graus/s |

Abra Experiencia.tscn e selecione Nave/CameraRig para ajustar esses campos.
O amortecimento usa delta real, independente do Ritmo acelerado de testes.
Nao existe roll manual, tremor novo nem recenter instantaneo.

| ContextoDeEntrada | Comportamento |
| --- | --- |
| Olhar | Mouse capturado, WASD/setas/analogico esquerdo orientam a visao |
| Combate | Cursor visivel, entrada controla mira; camera recentraliza suavemente |
| Gelo | Esquerda/direita controlam evasao; camera recentraliza suavemente |
| Menu | Cursor livre; olhar e viagem pausados |
| Bloqueado | Salto/inicializacao/erro: sem comando de olhar; recenter suave |

R ou pressionar o analogico direito recentraliza durante observacao. Esc abre
menu e libera cursor; Esc/CONTINUAR retoma. Perder foco abre menu por padrao,
configuravel em PausarAoPerderFoco na raiz da experiencia. Na chegada o cursor
fica livre para REINICIAR VIAGEM. Disparo e escudo mantem as acoes anteriores.

Normal e Fulldome usam a mesma orientacao global da CameraPiloto. As cinco
vistas auxiliares e a inversa da projecao da mira acompanham essa orientacao.
F8 alterna preview, F9 calibracao, F11 fullscreen, F10 diagnostico com angulos/contexto.

## Composicao e edicao

| Cena em Robusto/Cenas | Posicao local do Planeta | Composicao inicial aproximada |
| --- | --- | --- |
| Terra.tscn | (20,42,-300) | 4 graus a direita |
| Marte.tscn | (145,40,-300) | 26 graus a direita |
| Urano.tscn | (-140,35,-320) | 24 graus a esquerda |
| AnaBranca.tscn | (25,155,-330) | 25 graus acima |

Angulos variam com o deslocamento da nave. Nas quatro cenas, selecione
Mundo/CorposCelestes/Planeta: e um Node3D arrastavel pelo gizmo. No Inspector,
edite Transform > Position. Salve a cena, execute o projeto com F5, depois use
F1/F2/F4/F6 dentro do jogo para testar Terra/Marte/Urano/Ana Branca.
F6 do editor executa uma cena isolada, nao o mesmo atalho do jogo.

Superficie contem a malha que gira; CentroDoPlaneta e filho da ancora e acompanha
o movimento. Aneis tambem e filho de Planeta em Urano. Nao e necessario mover
os tres separadamente. Nenhum marcador obriga a camera a olhar automaticamente.
Chegada preserva sua estrutura anterior. Curvas Rota nao foram alteradas.

## Cockpit

As cadeiras eram meshes geradas separadamente, nao um modelo externo indivisivel.
Nao tinham gameplay, colisoes ou sistemas essenciais filhos. CabineCompacta=true
na experiencia impede gerar AssentoPassageiro, EncostoPassageiro e BaseAssento,
alem dos trechos posteriores de janelas/portas do compartimento de passageiros.
O gerador mantem o modo antigo disponivel para as cenas herdadas na copia.

O cockpit termina em z=1.45, com parede traseira opaca, piso, arremates e apoio
estrutural; nao ficou aberto para o espaco. Mantidos painel frontal, comando,
para-brisa, vidros laterais e superiores da area do piloto. A camera apenas
rotaciona, sem atravessar paredes ou piso. Os dois extremos laterais, teto e
limite inferior foram capturados e inspecionados.

Limitacao: o interior ainda e procedural; suas pecas aparecem ao executar,
nao como um prefab completo arrastavel no editor. Nao foi criado modelo final.

## Ana Branca

SuperficieEstelar.gdshader substitui a aparencia branca simples por ruido 3D
em camadas, variacao branco-azulada, filamentos sutis, emissao e fluxo lento.
Nao usa mapa externo, nao tem costura UV e nao acrescenta grandes labaredas.
RotacaoDoPlaneta=0.003 rad/s gira somente a superficie; fluxo padrao=0.012.

HaloEstelar.gdshader atua numa segunda esfera transparente aditiva: raio72
em torno de superficie de raio60. ForcaHalo=0.2, sem bloom global novo.
LuzEstelar e uma OmniLight3D fria: energia1.8, alcance800, atenuacao0.6,
sem sombras. Acrescenta iluminacao local sem remover a luz ambiente herdada.

Para editar: abra AnaBranca.tscn e selecione Planeta. AparenciaAnaBranca expoe
Emissao=0.65, ForcaHalo=0.2 e EnergiaLuz=1.8, atualizados tambem no editor.
Para detalhe/cor/fluxo, selecione Superficie > Mesh > Material > Shader Parameters.
Contraste padrao=0.7. Ajuste RotacaoDoPlaneta na raiz; raio em Mesh da Superficie
ou Halo; cor/alcance em LuzEstelar. Os tres parametros exportados na ancora
controlam os valores correspondentes, portanto ajuste-os ali, nao duplicados
no material/luz. Os materiais sao duplicados em runtime para evitar propagacao.

Fontes/licencas: nenhum download novo. Shaders sao codigo original desta rodada;
NASA foi referencia contextual, sem imagem incorporada. ASSET_SOURCES.md registra
links e creditos dos assets reaproveitados. A superficie e uma interpretacao
artistica, nao reconstrucao cientifica. Panorama existente preservado.

## Validacao

- Build .NET: zero erros e zero avisos.
- Testes de dominio: aprovados, incluindo limites, velocidade angular diagonal,
  recenter sem salto e equivalencia de suavizacao a 30/60 quadros por segundo.
- Importacao final Godot headless/editor: concluida sem erros reportados.
- ValidarVisualCamera: 61 verificacoes headless e 87 com render, zero falhas.
- ValidarRobusto: 47 verificacoes headless e 68 com render, zero falhas.
- Entrada por evento de mouse e acoes de teclado, recenter, pausa/contextos,
  ausencia de alteracao no transform da nave e limites do cockpit verificados.
- Fullscreen, Normal, Fulldome, grade, direcao de projecao e raios de mira testados.
- Sequencia completa: Terra, saltos, Marte/piratas/escudo, Urano/gelo,
  Ana Branca, chegada e reinicio; sistemas persistentes preservados.
- Superficie estelar: variacao luminosa 0.260 e 0 de 256 amostras saturadas;
  captura inspecionada para textura e halo. Medida local, nao calibracao de brilho.

Capturas: `.godot/validacao_visual_camera`; regressao: `.godot/validacao_robusta`.
Logs: `.godot/visual-camera-headless.log`, `visual-camera-render.log`,
`camera-regressao.log`, `visual-camera-regressao-grafica.log` e
`importacao-visual-camera.log`. Comandos reproduziveis em DEBUG_GUIDE.md.

Durante a validacao, nomes automaticos dos meshes gerados impediram a contagem
legivel das janelas; a criacao passou a solicitar nomes legiveis. A chegada
tambem foi explicitamente incluida no contexto de menu para manter o botao de
reinicio clicavel. A execucao automatizada desativa a pausa por perda de foco
apenas nas cenas de teste; o jogo a conserva. Nao ha falhas pendentes nos testes.

## Limites restantes

Nao houve teste com arcade fisico, projetor ou publico no planetario. Fulldome
continua experimental, sem warp/blending. Sensibilidade, conforto e brilho
precisam de ensaio presencial. As execucoes com fixed-fps verificam comportamento
e render, nao demonstram FPS real; desempenho na resolucao da sala permanece
sem certificacao. Nao foi produzido executavel exportado nesta rodada.
HUD/roteiro/duracoes preservados; arte e curadoria cientifica continuam provisorias.

## Arquivos desta rodada

Modificados em relacao ao manifesto inicial desta rodada:

```text
Apresentacao/Cenas/InteriorDaNave.cs
Robusto/Cenarios/CenarioDaViagem.cs
Robusto/Cenas/Experiencia.tscn
Robusto/Cenas/Terra.tscn
Robusto/Cenas/Marte.tscn
Robusto/Cenas/Urano.tscn
Robusto/Cenas/AnaBranca.tscn
Robusto/Entrada/EntradaDaViagem.cs
Robusto/Projecao/SuperficieEstelar.gdshader
Robusto/UI/InterfaceDaViagem.cs
Robusto/Viagem/DiretorDaViagem.cs
Testes/TestesRobustos.cs
Testes/Integracao/ValidarRobusto.cs
ASSET_SOURCES.md
DEBUG_GUIDE.md
FULLDOME.md
ROBUST_PROTOTYPE.md
SCENARIOS.md
```

Novos, incluindo os respectivos .uid gerados para scripts/shader:

```text
Robusto/Cenarios/AparenciaAnaBranca.cs
Robusto/Dominio/EstadoDoOlhar.cs
Robusto/Projecao/HaloEstelar.gdshader
Robusto/Viagem/ControladorDoOlhar.cs
Testes/Integracao/ValidarVisualCamera.cs
Testes/Integracao/ValidarVisualCamera.tscn
RELATORIO_VISUAL_CAMERA.md
```

Logs, capturas e caches em .godot nao integram o codigo versionado. O git status
tambem inclui trabalho herdado; nao atribuir todas as suas entradas a esta rodada.
