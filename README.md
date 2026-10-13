# GAME - Iniciacao Cientifica UFBA

Experiencia espacial interativa para o Planetario da UFBA, desenvolvida em
Godot com C#. Esta branch contem o **prototipo robusto atual**.

## Sobre o projeto

A viagem segue Terra -> Marte -> Urano -> Ana Branca -> destino final, com
saltos, combate contra piratas e evasao de um corpo gelado. A nave e **on-rails**:
a trajetoria e automatica. O jogador controla a visao e as interacoes dos eventos,
nao a aceleracao ou a direcao da nave. Isso e uma decisao de design.

## Inicio rapido

1. Instale **Godot 4.7.1 .NET** e o **.NET SDK** (validado: 10.0.400).
2. Clone/baixe a branch **codex/prototipo-robusto**, conforme abaixo.
3. Importe o arquivo `project.godot` no Project Manager da Godot.
4. Aguarde a importacao dos recursos e compile o C# com **Build**.
5. Pressione **F5** no editor. Dentro do jogo, Esc abre configuracoes.

## Requisitos

- Godot **4.7.1 stable .NET/mono**, confirmada pelo executavel de desenvolvimento
  e pelo `Godot.NET.Sdk/4.7.1` no projeto C#. A edicao sem .NET nao serve.
- .NET **SDK**, nao somente Runtime. Framework alvo: `net8.0`. O SDK efetivamente
  testado e **10.0.400**; outras versoes nao foram certificadas nesta entrega.
  Os testes de console precisam tambem do runtime .NET 8 (`dotnet --list-runtimes`).
- Ambiente testado: Windows x64, GPU NVIDIA GTX 1650, renderer **Compatibility**
  (`gl_compatibility`). Use drivers e suporte OpenGL adequados a esse renderer.
  Linux/macOS e outros equipamentos nao foram testados nesta entrega.
- Internet para baixar o projeto e restaurar pacotes .NET na primeira compilacao.
- Git apenas para a opcao de clone; Git LFS nao e necessario atualmente.

Downloads oficiais: [Godot](https://godotengine.org/download/archive/) e
[.NET](https://dotnet.microsoft.com/download/dotnet).
Confira `dotnet --list-sdks`. O editor nao inclui o compilador C#;
veja os [requisitos oficiais de C#](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html).
Nao ha addons externos, banco de dados, servidor ou chave de API exigidos.

## Baixando o projeto

### Opcao 1 - Git

```sh
git clone --branch codex/prototipo-robusto https://github.com/JoaoGoncalvesPaesNunes/JogoPlanetarioUFBA.git
cd JogoPlanetarioUFBA
```

O comando seleciona a versao robusta, nao a `main` antiga. Em repositorio privado,
use uma conta com acesso concedido pela equipe; nao coloque tokens na URL.

### Opcao 2 - GitHub ZIP

Abra a [branch robusta](https://github.com/JoaoGoncalvesPaesNunes/JogoPlanetarioUFBA/tree/codex/prototipo-robusto),
confira o seletor de branch e escolha **Code > Download ZIP**. Extraia tudo antes
de abrir. Nao execute de dentro do ZIP. ZIP nao inclui historico Git.

## Abrindo o projeto na Godot

1. Abra a Godot **4.7.1 .NET** instalada, no Project Manager.
2. Clique em **Import / Importar**.
3. Navegue ate a pasta clonada ou extraida.
4. Selecione **project.godot**, confirme a importacao e abra o projeto.
5. Aguarde a importacao de texturas e modelos. A primeira abertura demora mais:
   a pasta `.godot` sera criada localmente. Shaders sao preparados pela engine.
6. Compile com **Build**. Alternativa no terminal, dentro da pasta do projeto:

```sh
dotnet build JogoPlanetarioUFBA.sln
```

Nao precisa copiar caches, instalar Blender nem baixar os assets manualmente.
Modelos/texturas necessarios ja estao no repositorio. Nao ha trilha ou voz gravada
na viagem atual; as falas aparecem como texto.

## Rodando o prototipo

Pressione **F5** ou **Play Project / Executar projeto** no editor.
A cena principal ja esta configurada: `res://Robusto/Cenas/Experiencia.tscn`.
Se o editor solicitar uma Main Scene, selecione essa cena.

A nave parte sozinha; nao e necessario segurar W. A janela inicia em tela cheia.
Na chegada, **REINICIAR VIAGEM** volta para Terra.

**F5 no editor** executa o projeto inteiro. **F6 no editor** executa somente a
cena aberta: os ambientes nao tem camera/sistemas persistentes independentes,
portanto use F5 para jogar. Com foco na janela do jogo, F5/F6 sao atalhos debug
de etapas diferentes, descritos abaixo.

## Controles

### Durante a viagem

| Entrada | Acao |
| --- | --- |
| Mouse | Olhar horizontal/vertical, com cursor capturado |
| WASD / setas / analogico esquerdo | Olhar ao redor |
| R / pressionar analogico direito | Recentralizar suavemente |
| Esc | Pausar/continuar, liberar cursor e abrir configuracoes |
| F11 | Alternar tela cheia/janela |

Limites: yaw +/-115 graus, pitch -50/+70 graus, sem roll manual.
Perder foco pausa por padrao. Durante saltos o olhar fica bloqueado e retorna
suavemente a frente. Nao ha controle livre da posicao da nave.

### Combate em Marte

- Mouse, WASD/setas ou analogico esquerdo: movimentar a mira.
- Clique esquerdo, Espaco ou botao A do gamepad: disparar.
- E ou botao B: escudo de teste.

Nesse contexto a entrada nao gira a camera. Neutralize os contatos para seguir.
O escudo e uma extensao experimental, sem integracao com smartphones.

### Evento de Urano

Segure A/seta esquerda ou D/seta direita, ou o analogico esquerdo para um lado,
ate completar a correcao. Mudar de lado reinicia o acumulo. A nave executa a
evasao automaticamente; sem entrada, o evento aguarda em distancia segura.
Nao existe comando de olhar nesse contexto. Arcade fisico ainda nao foi validado.

### Debug

Disponiveis em build debug com `AtalhosDebug` habilitado, **com foco no jogo**:

| Tecla | Acao |
| --- | --- |
| F1 | Terra |
| F2 | Marte |
| F3 | Piratas |
| F4 | Urano |
| F5 | Gelo |
| F6 | Ana Branca |
| F7 | Salto final |
| F8 | Alternar Fulldome Preview |
| F9 | Ativar preview e alternar grade de calibracao |
| F10 | Diagnostico de etapa, FPS, rota e camera |

Bindings registrados em runtime por `Robusto/Entrada/EntradaDaViagem.cs`;
Esc/F1-F11 tratados em `Robusto/Viagem/DiretorDaViagem.cs`. Nao procure todos
esses bindings na aba Input Map antes de executar.

## Modos de exibicao

**Tela normal:** para desenvolvimento e monitor/TV/projecao convencional.
**Fullscreen:** tamanho da janela, independente da projecao; F11 alterna.
Para testar fullscreen no editor, execute em janela separada, nao embutida.

**Fulldome Preview:** Esc > EXIBICAO > Fulldome Preview; para sair, selecione
Tela normal. F8 tambem alterna em debug. O preview mostra um fisheye circular
de ate 180 graus; areas pretas laterais no monitor retangular sao intencionais.
F9 mostra a grade no mesmo jogo, nao ha uma cena separada de calibracao.

O modo Fulldome atual e experimental e devera ser calibrado e validado no
equipamento real do planetario. Nao possui warp, blending ou certificacao de
conforto. A resolucao padrao e 1024; 4K nao e garantia de desempenho.
Configuracao: `Robusto/Dados/Exibicao.tres`. Detalhes em [FULLDOME.md](FULLDOME.md).

## Estrutura dos cenarios

| Ambiente | Cena |
| --- | --- |
| Viagem principal | `res://Robusto/Cenas/Experiencia.tscn` |
| Terra | `res://Robusto/Cenas/Terra.tscn` |
| Marte | `res://Robusto/Cenas/Marte.tscn` |
| Urano | `res://Robusto/Cenas/Urano.tscn` |
| Ana Branca | `res://Robusto/Cenas/AnaBranca.tscn` |
| Destino final provisorio | `res://Robusto/Cenas/Chegada.tscn` |

Cenas antigas de Jupiter/Saturno/Buraco Negro existem como legado, mas nao
participam da viagem robusta. Catalogo ativo: `Robusto/Dados/Catalogo.tres`.

## Alterando a posicao de um planeta

1. No FileSystem, abra a cena do ambiente da tabela acima.
2. Na arvore, expanda `Mundo > CorposCelestes` e selecione **Planeta**.
3. Abra **3D** e use o gizmo de mover: arraste a seta do eixo desejado.
4. Para valores exatos, abra **Transform > Position** no Inspector.
5. **Rotation** gira a ancora; **Scale** muda seu tamanho. Evite escala zero.
6. Nas quatro primeiras regioes, a malha fica no filho **Superficie**; em Urano,
   os aneis acompanham Planeta. Na Ana Branca, halo e luz tambem acompanham.
7. Salve com Ctrl+S e execute o projeto com F5.
8. Com foco no jogo, use F1/F2/F4/F6 para conferir a regiao correspondente.

O destino final mantem Planeta como malha direta. Nao mova a camera para corrigir
a composicao do planeta. Para detalhes da estrela, veja [SCENARIOS.md](SCENARIOS.md).

## Alterando a trajetoria automatica

Abra a cena do ambiente e selecione **Rota (Path3D)**. Na barra do editor 3D,
ative a edicao da curva e mova os pontos/alcas Bezier. Conserve ao menos dois
pontos distintos e tangentes suaves; evite atravessar planetas ou curvas fechadas.
Salve e teste pela cena principal. O piloto amostra a `Curve3D` por distancia,
nao depende de um PathFollow3D controlado pelo jogador.

Marte e Urano possuem `Jogabilidade/Gatilhos/InicioEvento`: ajuste o marcador
para o local adequado da nova rota. Nao o deixe no fim absoluto, onde a nave
desacelera. A velocidade por etapa fica no roteiro abaixo.

## Alterando dialogos/falas

Edite `Robusto/Dados/roteiro.json`, uma lista JSON de etapas. `Titulo` e `Texto`
aparecem na interface; `Duracao` controla observacao, `Velocidade` o movimento.
`Audio` opcional recebe um caminho `res://` para um AudioStream importado.
Omitido/vazio significa silencio; nao ha sincronizacao automatica com a fala.

Preserve `Id`, `Cenario`, `Tipo` e a ordem ao editar somente textos. Use aspas
duplas, escape aspas dentro das falas e nao deixe virgula apos o ultimo item.
Salve, execute F5 e va a etapa pelo debug. Combate/gelo/salto dependem de
conclusao do evento, nao apenas de Duracao.

## Problemas comuns

- **C# nao carrega/Build falha:** confira edicao .NET, SDK e restauracao NuGet.
  Rode `dotnet build JogoPlanetarioUFBA.sln` e leia o primeiro erro.
- **Teste pede Microsoft.NETCore.App 8:** instale runtime .NET 8 ou SDK 8 adicional;
  nao altere o TargetFramework somente para ocultar o problema.
- **Sem texturas:** aguarde a importacao, confira a extracao completa e leia Output.
- **F6 nao tem camera:** execute F5; o ambiente depende da experiencia principal.
- **Arquivos reimportados:** `.godot` e cache local e nao vem do GitHub.
- **Cursor preso:** Esc libera. Mouse controla mira no combate, nao camera.
- **Nave nao responde a W:** rota automatica; W altera olhar/mira conforme contexto.
- **Fullscreen limitado:** desative execucao embutida no editor e use F11.
- **Circulo/grade inesperados:** Esc > Tela normal ou F8; F9 alterna a grade.

## Desenvolvimento e creditos

- [Guia detalhado de desenvolvimento](docs/DEVELOPMENT_GUIDE.md)
- [Controles e testes](DEBUG_GUIDE.md)
- [Edicao de cenarios](SCENARIOS.md)
- [Arquitetura robusta](ROBUST_PROTOTYPE.md)
- [Fontes e licencas dos assets](ASSET_SOURCES.md)
- [Revisao para publicacao](docs/PUBLICACAO_GITHUB.md)
- [README historico, apenas referencia](Documentacao/README_LEGADO.md)

Mantenha os creditos Solar System Scope (CC BY 4.0), Kenney/Quaternius (CC0)
e os termos proprios do panorama. Nao ha uma licenca geral do codigo definida
neste repositorio; a equipe deve decidi-la antes de autorizar reutilizacao externa.
