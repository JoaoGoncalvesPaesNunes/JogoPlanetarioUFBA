# Jogo Planetario UFBA

Fundacao inicial de um jogo imersivo e inclusivo para o Planetario da UFBA, criado em Godot 4 com C#.

O objetivo destes primeiros marcos nao e entregar o jogo completo. A base atual prova uma arquitetura pequena para desenvolvimento em desktop, com entrada logica desacoplada de hardware, eventos de dominio, estado minimo da nave, porta de projecao e um microprototipo executavel em primeira pessoa.

## Ambiente validado

- Godot .NET/C#: 4.7.1 stable mono, executavel localizado em `E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe`.
- .NET SDK: 10.0.400.
- Target framework do projeto: `net8.0`.

## Como executar

Abra o projeto pela edicao .NET/C# da Godot 4.7.1 ou compativel e rode a cena principal.

Pelo terminal:

```powershell
E:\Godot\Godot_v4.7.1-stable_mono_win64.exe --path E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA
```

Observacao: use a edicao .NET/C# da Godot. O build sem `mono` nao carrega scripts `.cs`.

## Controles do microprototipo

- `WASD` ou setas: alinhar o reticulo da nave com o waypoint.
- Mantenha o reticulo alinhado ao waypoint para encher a barra de aproximacao.
- Ao alcancar o waypoint, a cena passa a aguardar a estacao virtual.
- Estacao virtual `ACAO 1`: iniciar suporte da tripulacao.
- Estacao virtual `CONFIRMAR`: concluir calibracao depois de `ACAO 1`.
- `Espaco`: acionar estabilizador de dobra, consumindo energia e aumentando estabilidade.
- Mouse: clicar nos botoes da estacao virtual.
- Estacao virtual `ESQ`/`DIR`: selecionar rota demonstrativa Marte/Jupiter.
- Estacao virtual `ACAO 2`: comando livre de teste, sem avancar a calibracao.

## Validacao

```powershell
dotnet build .\JogoPlanetarioUFBA.csproj
dotnet run --project .\Testes\TestesDominio.csproj
E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe --headless --path . --quit-after 3
```

O build C#, os testes de dominio e a carga headless da cena foram validados com a edicao .NET/C# da Godot.

## Escopo atual

- Perfil funcional `DesktopDev`.
- Adaptador `AdaptadorTecladoMouse` lendo acoes logicas do Input Map.
- Cena principal `CenaCockpitLoopCentral`.
- Camera de primeira pessoa e cockpit graybox.
- Loop central de dois waypoints com evento cooperativo.
- `EstadoDaNave` puro com energia, integridade, estabilidade da dobra e carga de dobra.
- `BarramentoDeEventos` tipado.
- HUD de debug.
- Estacao virtual clicavel para simular uma futura estacao arcade.
- Porta de projecao desktop e stub documentado para planetario.
- Diretor narrativo minimo para ORION e gatinho como texto.
- Cenario temporario com estrelas, planeta, asteroides e marcador de dobra.

## Fora do escopo deste marco

Nao foram implementados planetas finais, hardware real de arcade, correcao de domo, audio, dublagem, animacao final, backend, telemetria de pesquisa ou gameplay completo de 30 a 40 minutos.
