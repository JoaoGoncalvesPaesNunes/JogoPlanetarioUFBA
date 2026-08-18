# Jogo Planetario UFBA

Fundacao inicial de um jogo imersivo e inclusivo para o Planetario da UFBA, criado em Godot 4 com C#.

O objetivo destes primeiros marcos nao e entregar o jogo completo. A base atual prova uma arquitetura pequena para desenvolvimento em desktop, com entrada logica desacoplada de hardware, eventos de dominio, estado minimo da nave, porta de projecao e um microprototipo executavel em primeira pessoa.

## Ambiente validado

- Godot: 4.7.1 stable, executavel localizado em `E:\Godot\Godot_v4.7.1-stable_win64_console.exe`.
- .NET SDK: 10.0.400.
- Target framework do projeto: `net8.0`.

## Como executar

Abra o projeto pela edicao .NET/C# da Godot 4.7.1 ou compativel e rode a cena principal.

Pelo terminal:

```powershell
E:\Godot\Godot_v4.7.1-stable_win64_console.exe --path E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA
```

Observacao: o executavel Godot encontrado neste ambiente (`E:\Godot\Godot_v4.7.1-stable_win64_console.exe`) e o build editor padrao e nao carregou scripts `.cs` no teste headless. Para executar o microprototipo, instale ou aponte para a edicao .NET/C# correspondente.

## Controles do microprototipo

- `WASD` ou setas: alinhar o reticulo da nave com o ponto de dobra.
- `Espaco`: acionar estabilizador de dobra, consumindo energia e aumentando estabilidade.
- Mouse: clicar nos botoes da estacao virtual.
- Estacao virtual `ESQ`/`DIR`: selecionar rota demonstrativa Marte/Jupiter.
- Estacao virtual `ACAO 1`: apoio da tripulacao para aumentar estabilidade.
- Estacao virtual `ACAO 2`: redistribuir energia com pequena perda de estabilidade.

## Validacao

```powershell
dotnet build .\JogoPlanetarioUFBA.csproj
dotnet run --project .\Testes\TestesDominio.csproj
E:\Godot\Godot_v4.7.1-stable_win64_console.exe --headless --path . --quit-after 2
```

O build C# e os testes de dominio foram validados. A validacao headless da cena depende da edicao .NET/C# da Godot.

## Escopo atual

- Perfil funcional `DesktopDev`.
- Adaptador `AdaptadorTecladoMouse` lendo acoes logicas do Input Map.
- Camera de primeira pessoa e cockpit placeholder.
- Loop central de alinhamento com ponto de dobra.
- `EstadoDaNave` puro com energia, integridade e estabilidade da dobra.
- `BarramentoDeEventos` tipado.
- HUD de debug.
- Estacao virtual clicavel para simular uma futura estacao arcade.
- Porta de projecao desktop e stub documentado para planetario.
- Diretor narrativo minimo para ORION e gatinho como texto.
- Cenario temporario com estrelas, planeta, asteroides e marcador de dobra.

## Fora do escopo deste marco

Nao foram implementados planetas finais, hardware real de arcade, correcao de domo, audio, dublagem, animacao final, backend, telemetria de pesquisa ou gameplay completo de 30 a 40 minutos.
