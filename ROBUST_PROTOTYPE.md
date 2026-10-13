# Viagem robusta - Planetario UFBA

## Abrir a versao correta

- Original preservado: `E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA`.
- Nova versao: `E:\Godot\GAME_robusto`.
- Importe **E:\Godot\GAME_robusto\project.godot** na Godot **4.7.1 .NET**.
- Compile pelo botao Build e pressione F5. A nave parte automaticamente; nao precisa de W.
- Esc abre configuracoes e pausa a sequencia. F11 alterna janela/tela cheia.

PowerShell, de qualquer pasta:

```powershell
dotnet build 'E:\Godot\GAME_robusto\JogoPlanetarioUFBA.sln'
& 'E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe' --path 'E:\Godot\GAME_robusto'
```

Para desenvolver em janela, acrescente `-- --janela`.

## Arquitetura

```text
GAME_robusto/
  project.godot
  Robusto/
    Cenas/          Experiencia, Terra, Marte, Urano, AnaBranca, Chegada, Pirata
    Cenarios/      CenarioDaViagem
    Dados/         roteiro.json, Catalogo.tres, Exibicao.tres
    Dominio/       SequenciaDaViagem, ManobraDoGelo (C# puro)
    Entrada/       EntradaDaViagem
    Eventos/       CombateDeMarte, EventoDoGelo
    Projecao/      ConfiguracaoExibicao, GerenciadorExibicao, GradeCalibracao, Fisheye
    UI/            InterfaceDaViagem
    Viagem/        DiretorDaViagem, PilotoAutomatico, AudioDaViagem
  assets/robusto/   Texturas planetarias 2K
  Testes/          Regras puras e integracao Godot
  Apresentacao/    Sistemas e cenas herdados
  Jogabilidade/   Regras e salto herdados
  Projecao/        Porta de projecao herdada
```

Na cena `Robusto/Cenas/Experiencia.tscn`, persistem a nave, a cabine, o CameraRig,
o audio, o controlador da viagem e a interface. So o conteudo de
`RecipienteCenarios` e substituido. Nenhum Autoload novo foi criado.

O fluxo novo usa apenas o catalogo `Robusto/Dados/Catalogo.tres`.
As cenas legadas de Jupiter/Saturno/Buraco Negro permanecem na copia completa,
mas NAO pertencem ao fluxo robusto e nao sao carregadas por ele.

## Sequencia

Terra -> salto -> Marte -> piratas -> pos-Marte -> salto -> Urano -> gelo
-> pos-Urano -> salto -> Ana Branca -> salto final -> chegada.

`SequenciaDaViagem` e uma maquina de estados pura. Observacoes usam duracao;
combate, gelo e salto so avancam apos conclusao. Nao existem escolhas de rota
nem cadeias de temporizadores distribuidas. Na chegada, REINICIAR VIAGEM
retorna a Terra. Esc pausa os eventos e o movimento principal.

## Piloto automatico

A camera permite olhar independentemente da nave: mouse, WASD/setas ou
analogico esquerdo; R/pressionar analogico direito recentraliza suavemente.
Nao ha translacao nem roll manual. Durante combate/gelo a entrada pertence
ao minigame; durante salto fica bloqueada. Esc libera o cursor.
O cockpit robusto usa CabineCompacta, sem bancos nem compartimento de passageiros,
com fechamento traseiro e vidros dianteiros, laterais e superiores preservados.
Detalhes e edicao: `RELATORIO_VISUAL_CAMERA.md`.

Cada cenario tem um `Path3D` chamado `Rota`, com `Curve3D` salva na cena.
`PilotoAutomatico` amostra a curva por distancia, interpola a orientacao e
mantem roll nivelado. O publico nao controla posicao, yaw, pitch ou roll.
Perto do fim da curva a velocidade diminui suavemente; nao existe teleporte
para o inicio. Dimensione a rota para a duracao pretendida da apresentacao.

`Velocidade` e configurada por etapa no roteiro, em unidades de mundo/segundo.
Os modelos nao estao em escala astronomica. O Inspector da nave expoe
`Suavizacao`. Os marcadores InicioEvento de Marte/Urano controlam a distancia
minima de inicio, alem da duracao da apresentacao.

## Cenários e salto

O `GerenciadorCenarios` original foi reutilizado. Ele valida cadastro,
recurso, identificador e estrutura antes de retirar o ambiente anterior.
As pastas estruturais exigidas pelo contrato original foram preservadas;
nao ha uma segunda gerencia de cenarios concorrente.

O `ControladorDoSalto` ganhou a entrada automatica e escala de duracao apenas
nesta copia. Reutilizamos `ApresentacaoDoSalto`: 320 rastros em MultiMesh,
aceleracao, fundo escuro, hiperespaço e desaceleracao, sem flash branco.
A duracao total configurada em `roteiro.json` distribui as fases existentes.
O carregamento ocorre durante o tunel. A carga e sincrona, adequada aos cinco
cenarios pequenos; se futuros assets causarem travamentos, migrar o gerente
para carregamento em segundo plano. Uma falha pausa o fluxo, preserva o
ambiente e apresenta o erro; Esc/CONTINUAR tenta novamente.

## Combate de Marte

Os Marker3D em `Jogabilidade/PontosDeAparecimento` definem a formacao.
O prefab `Pirata.tscn` centraliza o modelo Kenney em sua origem. As naves
acompanham o deslocamento principal e oscilam lentamente em torno dos pontos.
Nao ha IA ou fisica orbital.

Mouse movimenta a mira; clique esquerdo ou Espaco dispara. WASD/setas ou
analogico esquerdo tambem movem a mira. Botao A de gamepad dispara.
O adaptador entrega acoes logicas, sem depender de um arcade stick especifico.
Mapeamento de arcade real ainda deve ser conferido no equipamento.

O tiro e hitscan: o raio da projecao intercepta volumes esfericos, escolhe o
alvo mais proximo e gera um rastro curto. Alvos atingidos encolhem e sao removidos.
O evento termina ao neutralizar todos. `QuantidadePiratas`, `RaioAcerto` e
`IntervaloDisparo` ficam no Inspector do `CombateDeMarte` da cena principal.
A quantidade efetiva e limitada ao numero de marcadores criados.

E ou botao B ativa o escudo por tres segundos. A API `AtivarEscudo()` e os
sinais `EscudoSolicitado`/`EscudoAtivado` sao pontos de extensao para a plateia.
Os ataques sao cenicos, sem dano acumulativo/game over; o escudo contabiliza
impactos bloqueados. Nao existe rede ou integracao com smartphones.

## Urano e manobra

O cristal esta visivel ao longe durante a apresentacao de Urano. No evento,
ele se aproxima do marcador AlertaGelo. Surge a barra; segure A/esquerda ou
D/direita por dois segundos. Trocar de lado reinicia o acumulo. Sem entrada,
a aproximacao pausa em distancia segura, sem condenar a sessao a uma colisao.

Ao completar a barra, a nave executa uma evasao lateral suave e retorna ao
eixo da rota; o gelo passa ate FimGelo. O piloto nao controla a curva.
Tempo de aproximacao, correcao, evasao e distancia lateral sao exportados
na raiz de `Urano.tscn`. A malha de gelo e um placeholder artistico Kenney.

## Ana Branca e chegada

AnaBranca usa superficie procedural emissiva azulada, ruido animado sutil,
halo transparente e luz fria local, sem bloom exagerado. Mantem campo estelar,
movimento lento e HUD reduzido. Nao possui minigame. A aparencia e artistica,
nao uma simulacao fisica de atmosfera estelar. O final usa `Chegada.tscn`,
substituivel no catalogo sem refazer o diretor. Ambos requerem arte final.

## Falas e audio

Edite `Robusto/Dados/roteiro.json`: `Titulo`, `Texto`, `Duracao`, `Velocidade`
e `Audio`. O ultimo aceita caminho `res://` de um AudioStream importado.
Deixe vazio para silencio. O player de narracao e persistente e para a fala
anterior quando muda a etapa. Nao foram baixados audio, vozes ou trilha;
as duracoes ainda nao sao sincronizadas automaticamente ao tamanho da fala.

## Documentos complementares

- `SCENARIOS.md`: edicao visual de cenas, planetas, curvas e gatilhos.
- `FULLDOME.md`: configuracoes, preview, calibracao e limitacoes.
- `DEBUG_GUIDE.md`: todos os controles e execucao dos testes.
- `ASSET_SOURCES.md`: creditos, licencas e placeholders.
- `RELATORIO_ROBUSTO.md`: entrega, validacoes e preservacao do original.

## Versionamento

Foi feita copia completa, incluindo arquivos nao commitados, assets, caches
e `.git`. A branch da COPIA e `codex/prototipo-robusto`; a original nao mudou.
O remoto herdado permanece configurado, mas nenhum commit ou push foi feito.
Nao execute push sem decidir se esta versao deve ir para o mesmo repositorio.
O manifesto inicial esta em `E:\Godot\manifesto_original_robusto.json`.
