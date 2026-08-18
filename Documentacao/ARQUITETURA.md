# Arquitetura inicial

Este projeto usa uma base simples inspirada em Ports & Adapters apenas onde isso reduz acoplamento real.

## Decisoes principais

- A jogabilidade consome a porta `IEntradaDoPiloto`, nao teclas fisicas.
- `AdaptadorTecladoMouse` traduz o Input Map da Godot para acoes logicas.
- Estacoes de passageiros usam `IEntradaDeEstacao`; a UI `EstacaoVirtual` e apenas um adaptador de desenvolvimento.
- O estado minimo da nave fica em `EstadoDaNave`, uma classe C# pura, para facilitar testes.
- O primeiro loop central usa alinhamento de rota: o piloto move um reticulo logico, tenta coincidir com waypoints de calibracao e aciona eventos cooperativos.
- Eventos de dominio passam por `BarramentoDeEventos`, instanciado pela cena principal. Nao foi criado Autoload nesta etapa para evitar estado global prematuro.
- A apresentacao usa `IPortaDeProjecao`; `AdaptadorProjecaoDesktop` funciona em monitor e `AdaptadorProjecaoPlanetario` fica como stub ate existirem especificacoes reais do domo.
- O `DiretorNarrativo` reage a eventos e publica pedidos de dialogo sem conhecer audio, animacao ou layout.

## Estrutura criada

- `Nucleo/`: configuracao, estado e eventos de dominio.
- `Entrada/`: portas, modelos, adaptadores e roteamento do Input Map.
- `Jogabilidade/`: controle de voo do microprototipo.
- `Projecao/`: porta e adaptadores de apresentacao.
- `Narrativa/`: diretor narrativo minimo.
- `Publico/`: estacao virtual de desenvolvimento.
- `Apresentacao/`: cena principal e HUD.
- `Dados/`: resources configuraveis.
- `Testes/`: testes simples sem dependencias externas.

## Limites assumidos

O perfil `Planetario` nao tenta corrigir geometria, lente, warping, blending ou regioes reais do domo. Qualquer implementacao concreta depende de informacoes tecnicas do Planetario da UFBA.

O Input Map inicial e garantido em runtime por `ConfiguradorInputMap`. Isso mantem a cena executavel desde a primeira abertura e evita espalhar teclas na jogabilidade. Em uma etapa posterior, esses mapeamentos podem ser consolidados no editor da Godot.

## Marco 2 - Cockpit e loop central

A cena principal agora e `CenaCockpitLoopCentral`, uma primeira fatia jogavel em primeira pessoa. A cena anterior `CenaMicroPrototipo` permanece como referencia/debug. O jogador nao pilota mais uma nave vista de fora; a camera representa o ponto de vista do cockpit. A camada `CockpitDoPiloto` desenha uma estrutura temporaria com janela, reticulo, marcador de destino e barras de energia/alinhamento/carga de dobra.

O `ControladorDeVoo` calcula `ApontamentoDoPiloto`, `PontoDeDobra` e `AlinhamentoDaRota`. Quando o alinhamento e bom, `ControladorDoLoopCentral` acumula progresso de aproximacao ate disparar `WaypointAlcancado` e `CalibracaoIniciada`.

A calibracao depende da estacao virtual: `ACAO 1` prepara suporte e `CONFIRMAR` conclui. Cada calibracao adiciona 50% de `CargaDeDobra`; dois ciclos concluem a sequencia. Ao final, `CONFIRMAR` reinicia o loop sem reiniciar a aplicacao.

## Ambiente de execucao

O projeto foi criado como Godot C# e compilou com `Godot.NET.Sdk/4.7.1`. A cena principal foi validada em modo headless com `E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe`. Para execucao visual, usar a edicao .NET/C# da Godot; o build sem `mono` nao carrega scripts `.cs`.
