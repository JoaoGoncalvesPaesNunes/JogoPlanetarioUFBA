# Controles e validacao

## Controles normais

| Entrada | Acao |
| --- | --- |
| Mouse | Olhar ao redor na observacao; mira durante combate |
| Clique esquerdo / Espaco / gamepad A | Disparo |
| WASD / setas / analogico esquerdo | Visao na observacao; mira no combate; esquerda/direita no gelo |
| R / pressionar analogico direito | Recentralizar suavemente a visao |
| E / gamepad B | Escudo de teste |
| Esc | Pausar/continuar e abrir configuracoes |
| F11 | Tela cheia/janela |
| REINICIAR VIAGEM | Retornar a Terra depois da chegada |

W nao acelera a nave. Nao ha escolha de destinos. O gamepad usa acoes Godot;
o mapeamento fisico de um arcade stick especifico nao foi validado.

O mouse fica capturado durante observacao. Esc libera o cursor e pausa;
perder foco tambem abre o menu por padrao. Combate libera o cursor para mirar,
gelo usa apenas a evasao e salto bloqueia o olhar. Esses eventos recentralizam
a visao suavemente. Na chegada, o cursor fica livre para REINICIAR VIAGEM.
Limites e ajustes do Inspector: consulte `RELATORIO_VISUAL_CAMERA.md`.

Teste dedicado de camera, cockpit, composicao e estrela:

```powershell
& 'E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe' --path 'E:\Godot\GAME_robusto' --fixed-fps 60 --disable-vsync --log-file .godot/visual-camera-render.log res://Testes/Integracao/ValidarVisualCamera.tscn -- --janela
```

Acrescente `--headless` para validar a estrutura sem render. Capturas ficam
em `.godot/validacao_visual_camera`. FPS com `--fixed-fps` nao e benchmark.

## Atalhos de desenvolvimento

Ativos apenas em build debug e com `AtalhosDebug` habilitado no Inspector:

| Atalho | Etapa |
| --- | --- |
| F1 | Terra |
| F2 | Marte, apresentacao |
| F3 | Piratas |
| F4 | Urano, apresentacao |
| F5 | Gelo, aproximacao e manobra |
| F6 | Ana Branca |
| F7 | Salto final |
| F8 | Alternar Fulldome Preview |
| F9 | Ativar preview e alternar calibracao |
| F10 | Informacoes de etapa, rota, salto e FPS |

O menu debug nao aparece na apresentacao normal. Para iniciar diretamente,
altere `EtapaInicialDebug` na raiz de Experiencia.tscn ou passe um parametro:

```powershell
& 'E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe' --path 'E:\Godot\GAME_robusto' -- --janela --etapa=piratas
```

IDs: `terra`, `salto_marte`, `marte`, `piratas`, `pos_marte`, `salto_urano`,
`urano`, `gelo`, `pos_urano`, `salto_ana`, `ana_branca`, `salto_final`, `chegada`.
`Ritmo` acelera os testes; mantenha 1 em apresentacao. Reiniciar uma etapa
recarrega seu ambiente, restaurando gelo, rota e alvos.

## Testes

Todos os comandos abaixo devem usar a COPIA:

```powershell
Set-Location 'E:\Godot\GAME_robusto'
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
& 'E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe' --headless --path . --fixed-fps 60 --log-file .godot/robusto-integracao.log res://Testes/Integracao/ValidarRobusto.tscn -- --janela
& 'E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe' --path . --fixed-fps 60 --disable-vsync --log-file .godot/robusto-visual.log res://Testes/Integracao/ValidarRobusto.tscn -- --janela
```

A cena de teste percorre a sequencia completa, confirma persistencia, mira,
neutralizacao, escudo, evasao, reinicio e modo dome. A rodada grafica salva
capturas em `.godot/validacao_robusta` e verifica pixels nao vazios.
Os testes antigos `ValidarSalto.tscn` e `ValidarCenarios.tscn` continuam disponiveis
na copia para regressao dos componentes reaproveitados.

## Validacao manual antes da sala

1. Rodar F5 sem tocar no teclado; conferir que a nave segue sozinha.
2. No combate, errar um disparo e acertar cada nave visivel; conferir rastro e remocao.
3. Em Urano, observar o objeto distante; segurar cada lado em execucoes distintas.
4. Conferir a passagem lateral e retorno suave a rota.
5. Comparar texto e enquadramento em monitor, janela quadrada e tela cheia.
6. Ativar F8/F9 e conferir costuras, orientacao e legibilidade no equipamento real.
7. Medir FPS sem `--fixed-fps`, com a resolucao desejada, durante salto e combate.
