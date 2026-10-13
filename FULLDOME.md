# Exibicao e Fulldome experimental

## Selecionar

- A viagem inicia em tela cheia no modo normal por padrao.
- Esc abre EXIBICAO: escolha Tela normal ou Fulldome Preview.
- F11 alterna tela cheia/janela. No editor, execute em janela separada para
  que a janela embutida da Godot nao limite a saida do jogo.
- Debug: F8 alterna preview; F9 alterna grade de calibracao.
- Arquivo: `Robusto/Dados/Exibicao.tres`, editavel pelo Inspector.
- Linha de comando: `-- --fulldome`; acrescente `--janela` para testar em monitor.
- `--calibracao` abre preview com grade e a viagem pausada no menu.

## Implementacao

`GerenciadorExibicao` implementa a porta existente `IPortaDeProjecao`.
O mundo e o gameplay continuam 3D. No modo normal a camera persistente renderiza
diretamente. No preview, cinco cameras de 90 graus, na MESMA origem optica,
renderizam frente, esquerda, direita, cima e baixo em SubViewports que
compartilham World3D. Nao sao cameras de gameplay independentes.

O shader amostra a face apropriada pela direcao esferica e produz fisheye
equidistante de ate 180 graus. A saida e quadrada; em monitor retangular,
o circulo permanece inteiro, com areas pretas laterais intencionais.
As cinco faces param de atualizar quando se volta ao modo normal.

O CameraRig agora permite olhar ao redor. As cinco vistas derivam da mesma
orientacao global de CameraPiloto; nao existe um segundo controle de olhar.
O centro acompanha a visao do jogador, com a calibracao optica aplicada depois.
Mouse/analogico, limites, suavizacao e recentralizacao valem nos dois modos.
Combate e gelo recentralizam suavemente e reservam a entrada ao evento.
Limite angular atual: 40 graus/s; recentralizacao: 32 graus/s; sem roll manual.
Esses valores precisam de ensaio de conforto na sala, nao sao certificacao.

A origem recentralizada olha para frente: o centro acompanha o eixo da camera,
nao uma promessa de zenite fisico. `RotacaoCameraGraus.X = 90` permite testar
um eixo dirigido para cima; oriente conforme o mapeamento do equipamento real.

## Configuracoes no Inspector

| Campo | Efeito |
| --- | --- |
| FulldomePreview | Modo inicial |
| TelaCheia | Janela ou fullscreen inicial |
| ResolucaoSaida | Lado da imagem quadrada, 512 a 4096 |
| FisheyeFov | Campo total, 90 a 180 graus |
| RotacaoCameraGraus | Pitch, yaw, roll do rig optico, em graus |
| OrientacaoDomo | Rotacao bidimensional da imagem |
| Escala | Zoom da saida |
| Recorte | Raio de corte do circulo |
| EspelharHorizontal / Vertical | Reflexoes de saida |
| RaioSeguroUI | Inscricao da area de texto no circulo |
| WarpingHabilitado | Apenas reserva; emite aviso, NAO aplica warp |

O modo inicial usa 1024, nao 4K. O menu oferece 1024, 2048 e 4096.
As faces usam metade do lado da saida. Alteracoes do menu valem para a sessao;
para persistir, edite o `.tres`. 4096 existe, mas nao foi certificado para uso
ao vivo. Meça o desempenho no computador de projecao antes de aumentar.

## Mira e interface

A mira do combate usa a inversa da MESMA projecao, incluindo FOV, espelhos,
rotacao, escala e orientacao optica. Cliques fora do circulo nao disparam.
Textos ficam em uma area segura central configuravel e nao nos cantos extremos.

O HUD e uma composicao 2D segura por cima da imagem circular, nao texto 3D
calibrado no domo. Espelhos/rotacao do mundo nao deformam o texto do HUD.
O SubViewport `SaidaDomeMaster` contem mundo e grade; a janela final acrescenta
o HUD. Ao integrar um equipamento que consuma textura em vez da janela,
sera necessario incluir a camada de UI ou definir outra estrategia de texto.

## Calibracao

F9 mostra circulo externo, centro, 12 linhas radiais, tres faixas angulares e
referencias frente/tras/esquerda/direita. Sao referencias da saida, nao bussola
astronomica. O eixo optico esta marcado no centro. Teste primeiro sem espelhos,
escala 1 e recorte 1. Depois confira orientacao, horizonte pretendido e leitura
dos textos na sala. A grade nao substitui uma calibracao de projetores.

## Limites importantes

- FULLDOME_PREVIEW experimental; sem garantia de compatibilidade com a marca Fulldome.
- Sem SDK proprietario, blending, correcao de lente, warp ou sincronizacao multi-projetor.
- Custos de cinco vistas: FPS em monitor normal nao equivale a FPS do domo.
- Sem anti-aliasing dedicado nas costuras das faces; avaliar no equipamento.
- UI plana e area segura precisam de validacao humana no domo.
- Conforto, brilho, horizonte e velocidades precisam de ensaio presencial.
- Nenhum teste foi executado no planetario real.

Referencias tecnicas consultadas: [Viewports](https://docs.godotengine.org/en/stable/tutorials/rendering/viewports.html)
e [Camera3D](https://docs.godotengine.org/en/stable/classes/class_camera3d.html).
