# Escolha e salto para Marte ou Jupiter

## Uso

Execute a cena principal com F5 e avance com W. Ao atingir o ponto de salto
(98% do trecho), a nave para e o HUD exibe MARTE e JUPITER. Clicar em um botao
confirma imediatamente o destino. Tambem e possivel usar 1/2 ou as acoes 1/2
da estacao virtual existente. A primeira escolha valida vence.

O trecho inicial continua sendo `prototipo`; Terra nao recebeu conteudo novo.
Depois da chegada, a camera pode observar o planeta. Enter ou VOLTAR A ROTA INICIAL
recarrega o ambiente inicial e limpa o destino. Avance novamente com W para
repetir a escolha de Marte ou Jupiter, sem fechar o jogo. O retorno e imediato,
sem animacao de viagem de volta nem eventos locais no destino.

## Integracao e estados

O destino fica em `ControladorDoSalto.DestinoSelecionado`, uma classe C# pura
em `Jogabilidade/Viagem`. `CenaNavegacaoOnRails.EscolherDestino(id)` e a entrada
unica utilizada pelo HUD, teclado e estacao. A classe so aceita `marte` ou
`jupiter`, apenas enquanto aguarda escolha ou depois de uma falha recuperavel.

Fases: Normal -> AguardandoDestino -> Preparando (1 s) -> Acelerando (1,2 s)
-> Hiperespaco (minimo 1,6 s e destino pronto) -> Desacelerando (1,2 s) -> Chegada.
Chegada permanece como estado de conclusao ate reiniciar. Falha reabre a escolha.
O segmento on-rails mantem seu estado Saltando durante a espera e a transicao;
o evento SaltoGravitacionalIniciado so e publicado depois da escolha.

Nao ha temporizadores concorrentes. A fase controla a intensidade visual, a
permissao de escolha e o momento da carga. Repetir cliques, teclas ou chamadas
na API durante o salto nao altera o destino nem duplica o carregamento.
A propulsao para; o olhar recentraliza suavemente durante a preparacao e volta
a ficar livre na chegada. Nao ha mudanca de FOV, tremor ou flash branco.

`FaseDoSaltoAlterada` no barramento e o ponto preparado para futuros efeitos
sonoros. Nao foram adicionados sons, pois o projeto nao possui assets de audio
apropriados. Os eventos anteriores de inicio/conclusao continuam publicados.

## Efeito visual

`ApresentacaoDoSalto` substitui o clarão por um MultiMesh com 320 rastros 3D.
Cada rastro parte de um ponto curto e se alonga no eixo do deslocamento enquanto
acelera. Velocidade, comprimento e opacidade variam suavemente com a fase.
O campo cicla com atenuacao nas extremidades, formando um corredor estelar.

Os novos nos `TunelDoSalto/FundoDoTunel` e `TunelDoSalto/RastrosEstelares` ficam
sob o controlador da nave persistente. Uma esfera interna escura de raio 4
oculta o ambiente no pico do efeito. A cabine, mais proxima, permanece visivel.
O fundo e renderizado antes dos rastros transparentes. Na desaceleracao, o novo
planeta e o panorama aparecem enquanto os rastros encolhem e desaparecem.
Tudo usa meshes e materiais nativos da Godot; nenhum asset externo foi baixado.

## Carregamento e planetas

A carga e **sincrona**, pelo `GerenciadorCenarios.CarregarCenario(id)` existente.
Ocorre 0,2 s depois de entrar em Hiperespaco, com o tunel ja apresentado.
As cenas sao pequenas e compartilham o panorama previamente carregado. Nao
foi adicionado outro gerenciador, Autoload ou mecanismo de troca.

O estado so libera a desaceleracao depois da confirmacao de carga e do tempo
minimo. Se uma carga futura for demorada, o ultimo quadro do tunel permanece
visivel, mas a animacao pode pausar durante a chamada sincrona; assets maiores
poderao justificar carregamento threaded dentro do gerenciador numa outra etapa.
Erro de carga preserva o ambiente anterior, registra um aviso e reabre a escolha.

Marte e Jupiter continuam em suas cenas modulares, com um MeshInstance3D em
`Mundo/CorposCelestes` e `CentroDoPlaneta` em `AlvosDeCamera`. Ambos sao
**placeholders**, marcados no nome do no e no metadata `placeholder`:

- Marte: SphereMesh, material avermelhado e NoiseTexture2D provisoria da Godot.
- Jupiter: SphereMesh e shader `Planeta.gdshader` existente, com `gasoso = true`,
  sem aneis. As faixas sao estilizadas, nao uma textura astronomica de Jupiter.
- Fundo: panorama existente `hazy_nebulae_1.png` em `AmbienteDestino.tscn`,
  compartilhado apenas pelos dois destinos, com luz solar e ambiente.

Os planetas ficam em (0, 7, -115), com raio 22. A nave termina proxima a z=-39;
o centro esta no campo de visao e a camera permanece fora da superficie.
Nenhuma camera, nave ou HUD e recriado na troca. Saturno, Urano, Ana Branca,
Buraco Negro, votacao e eventos especificos nao foram implementados.

## Arquivos desta etapa

Criados: `Jogabilidade/Viagem/ControladorDoSalto.cs`,
`Apresentacao/Cenarios/AmbienteDestino.tscn`, `Testes/Integracao/ValidarSalto.cs`
e `.tscn`, este documento e os UIDs gerados pela Godot.

Alterados: `CenaNavegacaoOnRails.cs`, `CockpitDoPiloto.cs`, `ApresentacaoDoSalto.cs`,
`Marte.tscn`, `Jupiter.tscn`, `ControladorDeVoo.cs`, `AdaptadorOlharDesktop.cs`,
`ConfiguradorInputMap.cs`, `EventoDeDominio.cs`, `Testes/Program.cs`,
`Testes/TestesDominio.csproj`, `ValidarCenarios.cs`, `ValidarApresentacao.gd`,
`README.md` e `SCENARIOS.md`. O gerenciador, o catalogo e os outros destinos
nao precisaram ser alterados. Mudancas locais anteriores foram preservadas.

## Testes e ajustes

```powershell
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --headless --path . --fixed-fps 60 res://Testes/Integracao/ValidarSalto.tscn
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --headless --path . res://Testes/Integracao/ValidarCenarios.tscn
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --path . --fixed-fps 60 --disable-vsync res://Testes/Integracao/ValidarSalto.tscn
```

Build sem erros/avisos; testes puros aprovados, incluindo espera sem escolha,
destino bloqueado, tempo minimo, carga lenta simulada, falha e reinicio.
Teste integrado: 52 verificacoes headless, incluindo ambas as chegadas, duplo
clique, acao da estacao, preservacao dos sistemas, planeta no campo de visao,
recurso ausente deliberado e recuperacao. Os testes modulares continuam com
173 verificacoes aprovadas; a regressao de camera, avanco, re e reinicio passou.
Capturas por fase ficam em `.godot/validacao_salto/` e foram inspecionadas.

Na verificacao visual inicial, o planeta aparecia antes da desaceleracao porque
a esfera de fundo estava atras dele. O raio do fundo foi reduzido para cobrir
o ambiente sem cobrir o cockpit. O botao de reinicio foi deslocado para liberar
a vista do planeta. O teste de cenas independentes passou a usar um mundo
isolado por SubViewport, evitando sobrepor ambientes durante a validacao.
O unico aviso esperado e o recurso ausente provocado no teste de recuperacao;
os fluxos normais nao apresentaram erros de parser, NodePath ou recursos.
