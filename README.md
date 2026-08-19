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

## Controles principais

- `W`: avancar pela rota on-rails.
- `S`: recuar pela rota on-rails.
- Soltar `W`/`S`: a nave desacelera ate parar.
- `A`/`D` ou setas esquerda/direita: olhar para os lados.
- Setas cima/baixo: olhar para cima/baixo.
- Mouse: clicar nos botoes da estacao virtual.
- Ao final do segmento, `CONFIRMAR` na estacao virtual reinicia o teste.

A nave nao controla direcao lateral. Ela permanece presa a uma trajetoria predefinida; o piloto controla ritmo e camera.

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
- Cena principal `CenaNavegacaoOnRails`.
- Camera de primeira pessoa e cockpit graybox.
- Navegacao on-rails com `Path3D`/`PathFollow3D`.
- Avanco, re, parada e camera livre limitada.
- Assets espaciais iniciais integrados de Kenney Space Kit e Quaternius Ultimate Spaceships.
- Cometa visual com meteoros Kenney e cauda emissiva simples.
- Skybox 360 realista com panorama equiretangular via `PanoramaSkyMaterial`.
- Marcadores narrativos por progresso fisico.
- Flash/fade simples de salto gravitacional.
- Loop central de dois waypoints preservado em `CenaCockpitLoopCentral`.
- `EstadoDaNave` puro com energia, integridade, estabilidade da dobra e carga de dobra.
- `BarramentoDeEventos` tipado.
- HUD de debug.
- Estacao virtual clicavel para simular uma futura estacao arcade.
- Porta de projecao desktop e stub documentado para planetario.
- Diretor narrativo minimo para ORION e gatinho como texto.
- Cenario inicial com subcenas de assets espaciais, planeta estilizado, asteroides, nave distante e antena de pesquisa.
- Sensacao inicial de avanco com velocidade frontal e paralaxe simples do cenario.

## Fora do escopo deste marco

Nao foram implementados planetas finais, hardware real de arcade, correcao de domo, audio, dublagem, animacao final, backend, telemetria de pesquisa ou gameplay completo de 30 a 40 minutos.
