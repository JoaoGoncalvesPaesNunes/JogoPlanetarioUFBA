# Relatorio de entrega - nova versao robusta

Data: 2026-10-05. Engine: Godot 4.7.1 .NET. Codigo: C#.

## 1. Preservacao e localizacao

- **Original:** `E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA`.
- **Nova pasta:** `E:\Godot\GAME_robusto`.
- Antes de editar: inventario recursivo, git status e copia completa, incluindo
  arquivos ainda nao commitados, `.git`, assets e caches.
- A copia inicial teve **891 arquivos / 69.603.883 bytes / zero divergencias SHA-256**.
- A verificacao posterior do original encontrou **891 arquivos / zero alteracoes**.
- Manifesto externo: `E:\Godot\manifesto_original_robusto.json`.
- Nenhum script, cena, project.godot ou historico do original foi alterado.
- Nova branch apenas na copia: `codex/prototipo-robusto`.
- Nenhum commit ou envio ao GitHub foi realizado nesta etapa.

## 2. Diretorios e arquivos

A arvore principal esta em `ROBUST_PROTOTYPE.md`. Os diretorios herdados foram
mantidos; as novas funcionalidades estao agrupadas em `Robusto/`.

### Cenas criadas

- `Robusto/Cenas/Experiencia.tscn`: nova raiz persistente.
- `Robusto/Cenas/Terra.tscn`: abertura.
- `Robusto/Cenas/Marte.tscn`: apresentacao e marcadores dos piratas.
- `Robusto/Cenas/Urano.tscn`: planeta, aneis e gelo.
- `Robusto/Cenas/AnaBranca.tscn`: contemplacao.
- `Robusto/Cenas/Chegada.tscn`: destino substituivel.
- `Robusto/Cenas/Pirata.tscn`: modelo com centro visual corrigido.
- `Testes/Integracao/ValidarRobusto.tscn`: teste completo.

### Scripts criados

- `Robusto/Cenarios/CenarioDaViagem.cs`
- `Robusto/Dominio/SequenciaDaViagem.cs`
- `Robusto/Dominio/ManobraDoGelo.cs`
- `Robusto/Entrada/EntradaDaViagem.cs`
- `Robusto/Eventos/CombateDeMarte.cs`
- `Robusto/Eventos/EventoDoGelo.cs`
- `Robusto/Projecao/ConfiguracaoExibicao.cs`
- `Robusto/Projecao/GerenciadorExibicao.cs`
- `Robusto/Projecao/GradeCalibracao.cs`
- `Robusto/UI/InterfaceDaViagem.cs`
- `Robusto/Viagem/AudioDaViagem.cs`
- `Robusto/Viagem/PilotoAutomatico.cs`
- `Robusto/Viagem/DiretorDaViagem.cs`
- `Testes/TestesRobustos.cs`
- `Testes/Integracao/ValidarRobusto.cs`

### Outros arquivos criados

- `Robusto/Dados/roteiro.json`
- `Robusto/Dados/Catalogo.tres`
- `Robusto/Dados/Exibicao.tres`
- `Robusto/Projecao/Fisheye.gdshader`
- `Robusto/Projecao/SuperficieEstelar.gdshader`
- Tres texturas em `assets/robusto/`, listadas em `ASSET_SOURCES.md`.
- `ROBUST_PROTOTYPE.md`, `FULLDOME.md`, `DEBUG_GUIDE.md`, `ASSET_SOURCES.md`
  e este relatorio.
- Arquivos auxiliares `.uid` e `.import` gerados pela Godot.
- Logs e capturas em `.godot/`, ignorados pelo Git.

### Arquivos herdados alterados nesta tarefa, somente na copia

Comparacao contra os hashes no momento da copia, nao contra o ultimo commit:

| Arquivo | Alteracao |
| --- | --- |
| project.godot | Nome, nova cena principal, fullscreen, fundo preto |
| Jogabilidade/Viagem/ControladorDoSalto.cs | Entrada automatica e escala de duracao |
| Testes/Program.cs | Executa regras robustas |
| Testes/TestesDominio.csproj | Inclui classes puras robustas |
| README.md | Aponta para a nova versao e identifica documentacao legada |
| SCENARIOS.md | Guia de edicao visual robusta, preservando referencia anterior |

O git status tambem mostra mudancas anteriores herdadas do prototipo.
Elas nao devem ser confundidas com edicoes desta tarefa.

## 3. Reaproveitamento

Foram reutilizados o `GerenciadorCenarios`, o contrato `CenarioBase`, o catalogo
de recursos, a geometria da cabine panoramica, a orientacao suave de curvas,
o controlador e os 320 rastros do salto, o panorama espacial, os modelos Kenney
e a porta `IPortaDeProjecao`. Os antigos adaptadores e testes permanecem disponiveis.

Foram adicionados diretor de sequencia, piloto automatico, dois eventos locais,
entrada logica de mira, audio persistente opcional, HUD/menu responsivo,
composicao fisheye, calibracao, configuracao e ferramentas de debug.
Nao foram adicionados Autoloads, servidor de rede ou dependencias externas de codigo.

## 4. Funcionamento e edicao

| Sistema | Entrega e local de edicao |
| --- | --- |
| Piloto automatico | Curve3D de Rota por ambiente; velocidade por etapa; nenhuma entrada controla a nave |
| Rota visual | Selecione Path3D/Rota, mova pontos e alcas; veja SCENARIOS.md |
| Inicio dos eventos | Marker3D InicioEvento em Marte/Urano, combinado com tempo da apresentacao |
| ScenarioManager | Gerente existente valida e troca apenas RecipienteCenarios |
| Salto | Preparacao, aceleracao, tunel MultiMesh, carga e desaceleracao; duracao no JSON |
| Combate | Formacao simples acompanha a nave; hitscan, rastros, acertos e remocao |
| Mira | Mouse, teclado ou analogico; raio consistente nos modos normal e fisheye |
| Escudo | E/B ou API/sinais; extensao local sem smartphones |
| Gelo | Aproximacao, alerta, acumulo lateral, evasao automatica e retorno ao trilho |
| Ana Branca | Superficie autoiluminada estilizada, ritmo lento e HUD reduzido |
| Final | Cena provisoria substituivel por cadastro no catalogo |
| Falas | Titulo/Texto/Audio no roteiro.json, sem dezenas de falas nos scripts |

Os detalhes e parametros de Inspector estao em `ROBUST_PROTOTYPE.md` e
`SCENARIOS.md`. Nao ha Jupiter, Saturno ou Buraco Negro no novo catalogo/roteiro.
Os arquivos legados continuam presentes apenas por preservacao da copia completa.

## 5. Exibicao

- F5 inicia em fullscreen. F11 alterna; `-- --janela` inicia em janela.
- Esc abre o menu de exibicao e pausa a viagem.
- O menu seleciona Tela normal ou Fulldome Preview e a resolucao de saida.
- `Exibicao.tres` define padroes persistentes sem mostrar um menu na sala.
- F8 alterna preview; F9 exibe grade; `-- --fulldome --calibracao` abre calibracao.
- A grade mostra circulo, centro, linhas radiais, referencias e angulos.
- O preview usa cinco faces de um rig, compartilhando mundo e origem optica.
- A imagem circular pode ser 1024/2048/4096; o padrao e 1024.
- Configuracoes incluem FOV, rotacao optica, orientacao, escala, corte,
  espelhos e raio seguro da UI.

**Limite:** nao e integracao certificada com equipamento Fulldome. Nao ha warp,
edge blending, SDK de fabricante ou ensaio no domo real. A UI e uma camada 2D
segura, nao calibrada fisicamente; a textura intermediaria nao inclui o HUD.
Consulte `FULLDOME.md` antes de conectar qualquer sistema de projecao.

## 6. Assets

Foram baixadas apenas as texturas 2K de Terra, Marte e Urano, de Solar System
Scope, sob CC BY 4.0. Marte foi obtido no Wikimedia Commons depois de erro
HTTP 403 no download direto. Nenhuma autenticacao ou pagamento foi utilizado.
Autoria, URL, licenca, arquivo e tratamento estao em `ASSET_SOURCES.md`.

Continuam provisorios: Ana Branca, destino final, anel simplificado, piratas
e corpo gelado estilizados. Nao ha vozes, trilha ou sons novos; existe o player
persistente para adicionar narracao. Distancias/tamanhos nao sao cientificos.

## 7. Debug e inicio direto

F1 Terra; F2 Marte; F3 piratas; F4 Urano; F5 gelo; F6 Ana Branca; F7 salto final;
F8 preview; F9 calibracao; F10 informacoes; F11 fullscreen; Esc menu.
F1-F10 exigem build debug e AtalhosDebug habilitado.
`-- --etapa=piratas --janela` inicia diretamente no combate.
Todos os IDs e comandos de execucao estao em `DEBUG_GUIDE.md`.

## 8. Verificacao

Compilacao, regras puras, integracao headless e rodada grafica foram executadas
na COPIA. O original nao foi aberto para teste nem recompilado nesta tarefa.

- Build da solucao: zero erros e zero avisos.
- Regras puras novas e anteriores: aprovadas.
- Integracao robusta headless: 47 verificacoes, zero falhas.
- Integracao grafica final: 67 verificacoes, zero falhas, sem erros ou avisos no log.
- Regressao do gerenciador: 173 verificacoes, zero falhas.
- Regressao do salto: 72 verificacoes, zero falhas.
- Original: 891 hashes conferidos, zero diferencas.

A integracao percorre todas as etapas, testa movimento automatico, tiros sobre
o centro VISUAL das malhas, combate prolongado, escudo, passagem do gelo fora
da cabine, reinicio, persistencia e somente um ambiente ativo. Repete a mira
no fisheye, confere rotacao/espelho, gera capturas e procura pixels nao vazios.
Testa janela 960x540, 1600x900, 1024x1024, fullscreen e saidas 1024/2048.
As capturas sao inspecionadas visualmente; nao substituem validacao no planetario.
Hardware grafico identificado: NVIDIA GeForce GTX 1650, OpenGL Compatibility.

### Problemas encontrados e tratados

- Download de Marte recusado no dominio original: mesma obra licenciada obtida no Commons.
- Importacao dentro do sandbox sem acesso ao cache/certificados: testes repetidos
  com permissao adequada; nao foi alterada configuracao de seguranca.
- Botao de chegada testado antes da atualizacao do HUD: sincronizacao de quadros corrigida.
- Malhas Kenney fora da origem: prefab/transformacao de modelo centralizados.
- Rodape do HUD oscilando por tamanho minimo: ancoragem inferior estavel.
- Simulacao de mouse em coordenadas de janela/headless: teste usa coordenadas locais do viewport.
- Reinicio da mesma etapa preservava posicao do gelo: agora recarrega o ambiente.
- Combate longo podia deixar alvos para tras: a formacao acompanha a nave.
- Aviso de cena ausente na regressao do salto e intencional: testa falha recuperavel.

## 9. Limites e proximos passos

1. Validar formato de entrada do equipamento e calibrar orientacao/brilho/area segura na sala.
2. Medir FPS reais, sem passo fixo, na resolucao pretendida; 4K nao foi certificado.
3. Revisar roteiro, mapas, proporcoes e simplificacoes com a equipe cientifica.
4. Refinar arte da Ana Branca, final, gelo e naves sem alterar os contratos do fluxo.
5. Adicionar narracao licenciada e ajustar duracoes. Audio ainda nao dirige a sequencia.
6. Testar o arcade stick fisico e, depois, integrar smartphones pela API de escudo.
7. Para cenarios futuros pesados, avaliar carregamento assincrono e transicao sem travamento.
8. A evasao usa uma coreografia testada, nao colisao fisica geral. Apos mover gelo/rota,
   repetir os testes; nao ha garantia automatica para geometria arbitraria do editor.
9. Sem exportacao de executavel final nesta etapa; o projeto e executado na Godot .NET.

A entrega e um prototipo funcional e editavel, nao conteudo final nem sistema
de projecao homologado. Os 23 criterios funcionais foram tratados no escopo
experimental descrito, com as limitacoes de hardware e arte explicitadas acima.
