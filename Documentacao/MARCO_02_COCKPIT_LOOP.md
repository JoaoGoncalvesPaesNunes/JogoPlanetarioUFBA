# Marco 02 - Cockpit funcional e loop central

## Objetivo

Criar uma fatia jogavel pequena que demonstre a experiencia central: piloto em primeira pessoa, waypoint visivel, evento cooperativo, acao de passageiro, alteracao do estado da nave e conclusao em dois ciclos.

## Fluxo

1. A cena carrega o perfil `DesktopDev`.
2. ORION apresenta o objetivo inicial.
3. O piloto alinha o reticulo com o waypoint ativo.
4. Manter o alinhamento aumenta o progresso de aproximacao.
5. Ao chegar a 100%, `WaypointAlcancado` inicia uma calibracao.
6. A estacao virtual precisa pressionar `ACAO 1`.
7. Depois, a estacao virtual pressiona `CONFIRMAR`.
8. A calibracao altera energia, estabilidade e carga de dobra.
9. O segundo waypoint repete o mesmo fluxo.
10. Com carga de dobra em 100%, o loop central e concluido.
11. Pressionar `CONFIRMAR` na estacao reinicia a sequencia.

## Controles

- Piloto: `WASD` ou setas para alinhar o reticulo.
- Piloto: `Espaco` para estabilizar a dobra consumindo energia.
- Passageiro: mouse nos botoes da estacao virtual.
- Estacao: `ACAO 1` prepara suporte.
- Estacao: `CONFIRMAR` conclui a calibracao ou reinicia ao final.

## Arquitetura

- `CenaCockpitLoopCentral` monta a cena principal.
- `ControladorDeVoo` consome `IEntradaDoPiloto` e calcula alinhamento.
- `ControladorDoLoopCentral` gerencia estado do loop e progresso dos dois waypoints.
- `EventoDeCalibracao` modela a interacao cooperativa `ACAO 1` + `CONFIRMAR`.
- `EstadoDaNave` guarda energia, integridade, estabilidade e carga de dobra.
- `DiretorNarrativo` reage a eventos e atualiza a fala de ORION/gatinho.
- `CockpitDoPiloto` apresenta cockpit graybox, objetivo, ORION, reticulo e barras.
- `HUDDeDebug` permanece como leitura tecnica de eventos/estado.
- O console da Godot registra transicoes importantes do loop com prefixos como `[LOOP]`, `[OBJETIVO]`, `[EVENTO]`, `[ESTACAO]` e `[ESTADO]`.

## Placeholders

O cockpit usa `Control`/`CanvasLayer` com retangulos, labels e barras. O espaco usa estrelas, asteroides e planeta em primitivas simples. Nao ha arte final, dublagem, efeitos de dobra finais ou projecao real de domo neste marco.

## Validacao

Executado:

```powershell
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe --headless --path . --quit-after 3
```

Build e testes passaram. A Godot .NET carregou a cena em modo headless.
