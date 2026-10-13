# Cenarios modulares

## Versao robusta: edicao visual

O projeto desta pasta inicia em `Robusto/Cenas/Experiencia.tscn`.
O texto apos "Documentacao legada" descreve apenas o prototipo herdado.

### Abrir um ambiente

Abra `Robusto/Cenas/Terra.tscn`, `Marte.tscn`, `Urano.tscn`,
`AnaBranca.tscn` ou `Chegada.tscn`. Cada uma e independente.
Para jogar uma delas use F5 com `EtapaInicialDebug`, nao F6: os ambientes
nao possuem cameras ou controles duplicados.

### Mover planeta, luz e objetos

1. Selecione `Mundo/CorposCelestes/Planeta` na arvore.
2. Use o gizmo de mover/rotacionar/escalar no editor 3D.
3. Em Terra, Marte, Urano e AnaBranca, Planeta e um Node3D de posicionamento;
   a malha/material esta no filho Superficie. So esse filho gira lentamente.
4. Urano tem `Aneis` filho de Planeta: acompanha o gizmo automaticamente.
5. A iluminacao esta em `Iluminacao/AmbienteDestino`. Para modificar por
   regiao, habilite filhos editaveis na instancia e ajuste LuzSolar.
6. CentroDoPlaneta tambem e filho de Planeta nas quatro cenas. E uma referencia,
   nao obriga a camera a mirar nele. O olhar do jogador soma-se a orientacao da nave.

Posicoes locais iniciais: Terra (20,42,-300), Marte (145,40,-300),
Urano (-140,35,-320), AnaBranca (25,155,-330). Ajuste em Transform > Position
ou arrastando o gizmo; salve e teste em F5, depois F2/F4/F6 dentro do jogo
para Marte/Urano/Ana Branca. Chegada conserva a malha direta anterior.

Na AnaBranca, selecione Planeta para ajustar Emissao, ForcaHalo e EnergiaLuz.
Em Superficie > Mesh > Material > Shader Parameters ficam contraste, fluxo
e cores. A raiz da cena expoe RotacaoDoPlaneta em radianos/segundo (0.003).
O filho Halo fornece a esfera transparente, e LuzEstelar fornece luz fria local.
Veja o relatorio visual para os parametros da camera e limites de conforto.

### Editar o caminho

1. Selecione `Rota` (Path3D).
2. Ative edicao de curva na barra do editor 3D.
3. Arraste pontos e alcas Bezier. Mantenha tangentes suaves, sem curvas fechadas.
4. Adicione pontos para aumentar a duracao da viagem antes do fim da rota.
5. Altere `Velocidade` da etapa em `Robusto/Dados/roteiro.json`.
6. Execute a etapa via F1-F7; a nave percorre automaticamente essa curva.

As coordenadas ficam no recurso Curve3D da cena, nao em arrays de gameplay.
A nave inicia no primeiro ponto ao carregar o ambiente. Curvas devem conter
pelo menos dois pontos distintos e permanecer longe dos planetas.

### Definir eventos

Marte e Urano possuem `Jogabilidade/Gatilhos/InicioEvento`.
Mova esse Marker3D junto ao ponto da curva onde deseja permitir o evento.
O sistema projeta a posicao na curva e aguarda a distancia correspondente,
alem da duracao da apresentacao. Evite colocar o gatilho no ultimo ponto:
o piloto desacelera assintoticamente perto do fim.

Piratas: mova os Marker3D de `Jogabilidade/PontosDeAparecimento`; eles definem
a formacao no inicio do evento. Duplicar um marcador adiciona uma posicao;
ajuste tambem QuantidadePiratas na cena principal. O modelo usado esta em
`Pirata.tscn`, com offset corrigido para o centro visual coincidir com a mira.
O evento termina quando todos os alvos criados forem neutralizados.

Gelo: mova `Mundo/Objetos/Gelo`, `Jogabilidade/Gatilhos/AlertaGelo` e `FimGelo`.
O primeiro e a origem distante; o segundo e o ponto de espera; o terceiro
e o fim da passagem. Nao posicione AlertaGelo dentro da nave. Ajuste os quatro
parametros exportados na raiz Urano: aproximacao, correcao, duracao e desvio.
A manobra termina pelo tempo de evasao, retornando o desvio a zero.

### Adicionar ou substituir um ambiente

Duplique uma cena robusta e conserve o script CenarioDaViagem, Rota e os
containers exigidos por CenarioBase. Defina um Identificador exclusivo.
Registre-o em `Robusto/Dados/Catalogo.tres`; insira uma etapa de salto e uma
de observacao no roteiro. Nao e necessario destruir nave, camera ou HUD.
Para mudar apenas o destino final, substitua o caminho `chegada` no catalogo.

## Documentacao legada

## Decisao e preservacao

O projeto usa Godot .NET/C# e convencoes em portugues. A cena principal continua
sendo `Apresentacao/Cenas/CenaNavegacaoOnRails.tscn`. Nao ha Autoloads.
Ela criava por codigo o ambiente, nave, camera, rota, HUD e entrada. A comunicacao
existente utiliza eventos C# do barramento e callbacks, sem ligacoes entre regioes.

O ambiente existente mistura planeta oceanico, gigante com aneis, cometa, naves
e asteroides. Nao representa uma saida da Terra definida. Por isso foi preservado
em `Prototipo/Prototipo.tscn`, um ambiente tecnico adicional, nao uma oitava regiao
narrativa. `CenarioInicial = "prototipo"` mantem o comportamento normal do jogo.

As subcenas `CenarioEspacialMarco04.tscn` e `CometaMarco04.tscn`, os assets e
`SistemaPlanetario.cs` continuam nos caminhos anteriores. Sao compostos dentro do
prototipo, sem alterar suas transformacoes, materiais ou animacoes. A iluminacao
original foi transferida para `CenarioPrototipo.cs`, sob `Iluminacao`.

Foi usado apenas um script base, sem heranca de cenas: cada arquivo pode ser
editado diretamente sem depender dos nos internos de outra regiao. O script
base guarda metadados, valida os containers e oferece `AtualizarAmbiente(delta)`.
O prototipo usa esse metodo para preservar a animacao condicionada a viagem.

## Arquivos

```text
Apresentacao/Cenarios/
  CenarioBase.cs
  CenarioPrototipo.cs
  GerenciadorCenarios.cs
  TerraSaida/TerraSaida.tscn
  Marte/Marte.tscn
  Jupiter/Jupiter.tscn
  Saturno/Saturno.tscn
  Urano/Urano.tscn
  AnaBranca/AnaBranca.tscn
  BuracoNegro/BuracoNegro.tscn
  Prototipo/Prototipo.tscn
Dados/Cenarios/
  CatalogoCenarios.cs
  CatalogoCenarios.tres
Testes/Integracao/
  ValidarCenarios.cs
  ValidarCenarios.tscn
```

Os arquivos `.uid` associados aos scripts sao gerados pela Godot e devem ser
versionados com eles. O teste C# de integracao e compilado pelo projeto Godot;
o projeto de testes puros exclui essa pasta, sem adicionar dependencia da engine.

## Arvore de execucao

```text
CenaNavegacaoOnRails
  GerenciadorCenarios
  RecipienteCenarios
    Prototipo (ou outra regiao carregada)
  TrajetoriaOnRails
    SeguidorDaNave/OrientacaoSuave/ControladorDaNaveOnRails
      CameraPiloto
      InteriorDaNave
  OlharDesktop
  InstrumentosDoPiloto
  HUDDeDebug
  EstacaoVirtual
  ApresentacaoDoSalto
```

Estado da nave, segmento de viagem, narrativa, barramento, entrada, projecao,
rota e interfaces permanecem na composicao principal. O gerenciador nao os
reinicia nem altera. Audio global futuro tambem deve ficar fora do recipiente.
O NodePath novo e `GerenciadorCenarios.Recipiente = ../RecipienteCenarios`.
Os caminhos existentes da camera, cockpit e interface foram mantidos.

## Estrutura de cada regiao

```text
Raiz (CenarioBase / Node3D)
  Mundo
    CorposCelestes
    Geometria
    Objetos
    EfeitosVisuais
  Jogabilidade
    Eventos
    Gatilhos
    PontosDeAparecimento
  Iluminacao
  Audio
  AlvosDeCamera
  Debug (oculto inicialmente)
```

Marte e Jupiter agora possuem planetas provisórios e iluminacao compartilhada
para o salto com escolha (ver `Documentacao/SALTO_MARTE_JUPITER.md`). Os outros
cinco cenarios narrativos contem somente os containers e metadados.
Nenhum possui camera principal. Eventos especificos futuros pertencem a
`Jogabilidade/Eventos`; use `AlvosDeCamera` para referencias de observacao.
Coloque luzes e WorldEnvironment locais em `Iluminacao` para que saiam junto
com a regiao. Recursos compartilhados devem ser duplicados antes de alteracoes
que precisem ser exclusivas de um cenario.

## Abrir e carregar

Abra o `.tscn` desejado pelo painel FileSystem da Godot. Os containers e metadados
ficam disponiveis para edicao. Regioes vazias nao produzem uma experiencia jogavel
ao executar isoladamente, pois nao duplicam camera, luzes ou sistemas globais.
Para testar com a nave, altere `CenarioInicial` na raiz da cena principal e use F5;
ao terminar, restaure `prototipo`. A rota e os marcadores atuais continuam sendo
os do prototipo: a troca de ambiente ainda nao escolhe segmentos de gameplay.

Em C#, com a cena principal pronta:

```csharp
var gerenciador = GetNode<GerenciadorCenarios>("GerenciadorCenarios");
if (!gerenciador.CarregarCenario("marte"))
    GD.PushError(gerenciador.UltimoErro);
string atual = gerenciador.ObterIdentificadorAtual();
gerenciador.DescarregarCenarioAtual();
```

Use `ProjetoPlanetario.Apresentacao.Cenarios`. O caminho do GetNode no exemplo
e relativo a raiz principal. IDs disponiveis: `terra_saida`, `marte`, `jupiter`,
`saturno`, `urano`, `ana_branca`, `buraco_negro` e `prototipo`.

O carregamento e sincrono. Primeiro valida recurso, tipo, identificador e
containers; falhas mantem o ambiente anterior e retornam false com `UltimoErro`.
O anterior sai imediatamente da arvore e recebe QueueFree. Ha no maximo um
cenario no recipiente. Carregar o mesmo ID recria o ambiente; descarregar duas
vezes e seguro. Nao remova/libere manualmente o cenario atual: use a API.

Sinais `CenarioDescarregado(id)` e `CenarioCarregado(id)` informam trocas efetivas.
Uma troca solicitada dentro desses callbacks e rejeitada para evitar reentrada;
agende para outro quadro quando necessario. Nao ha sinal de carregamento
assincrono, cache de instancias nem Autoload do gerenciador.

## Adicionar uma regiao

1. Duplique uma das cenas vazias para uma nova pasta em `Apresentacao/Cenarios`.
2. Ajuste a raiz e os metadados no Inspector: `Identificador`, `NomeExibicao`,
   `IndiceDeEtapa` e `ProximosCenarios`. Use IDs sem acentos.
3. Adicione somente o par ID/caminho em `Dados/Cenarios/CatalogoCenarios.tres`.
4. Carregue o ID pela API. Nao precisa alterar o gerenciador ou a cena principal.
5. Quando houver decisao narrativa, ajuste os proximos IDs das regioes pertinentes.

`IndiceDeEtapa` usa 0 para Terra, 1 para Marte/Jupiter, 2 para Saturno/Urano,
3 para Ana Branca/Buraco Negro e -1 para o prototipo. As ramificacoes previstas
estao em `ProximosCenarios`, mas nao sao executadas automaticamente. O trecho
inicial permite somente a escolha Marte/Jupiter antes do salto. Nao foram
implementados votacao, piratas, pressao, radiacao, asteroides ou iceberg espacial.
**Chegada ainda nao foi definida nem implementada.**

## Validacao e relatorio

```powershell
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --headless --path . res://Testes/Integracao/ValidarCenarios.tscn
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --path . --script res://Testes/ValidarApresentacao.gd --fixed-fps 60 --disable-vsync
```

Resultado: build sem erros/avisos; testes de dominio aprovados; 173 verificacoes
de integracao aprovadas. Foram carregadas as sete cenas independentemente e
testadas Terra -> Marte -> Saturno -> Ana Branca e Terra -> Jupiter -> Urano ->
Buraco Negro, persistencia dos sistemas, progresso da rota, liberacao do anterior,
IDs/caminhos invalidos, sinais, reentrada, recarga e descarregamento repetido.

O editor importou os recursos sem erros de parser, NodePath ou carregamento.
A regressao visual original passou antes e depois, incluindo camera, avanco, re,
salto e reinicio. A imagem inicial foi inspecionada e comparada: diferenca RGB
zero nas 57.600 amostras, mantendo resolucao 1280x720. Logs e capturas ficam em
`.godot/`, sem versionamento. Uma execucao restrita emitiu aviso da engine sobre
certificados do Windows; a importacao com acesso normal nao apresentou esse erro.

Arquivos existentes alterados nesta etapa: `CenaNavegacaoOnRails.cs` e `.tscn`,
`JogoPlanetarioUFBA.csproj`, `Testes/TestesDominio.csproj` e `README.md`.
Arquivos novos: arvore listada acima, UIDs e este documento. As demais alteracoes
ja presentes no git status pertencem a etapa anterior e foram preservadas.
