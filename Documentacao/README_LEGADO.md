# Arquivo historico: README anterior a publicacao robusta

Este documento preserva o texto antigo, incluindo caminhos locais e controles
legados. Para instalar e executar a versao atual, use o README na raiz.

# Jogo Planetario UFBA

## Nova versao robusta (copia independente)

Esta pasta e `E:\Godot\GAME_robusto`. A cena principal agora e
`Robusto/Cenas/Experiencia.tscn`, com viagem automatica Terra -> Marte -> Urano
-> Ana Branca -> chegada. Abra o projeto desta pasta, compile e pressione F5.

Comece por [ROBUST_PROTOTYPE.md](ROBUST_PROTOTYPE.md),
[DEBUG_GUIDE.md](DEBUG_GUIDE.md) e [FULLDOME.md](FULLDOME.md).
Os textos abaixo descrevem os marcos LEGADOS, preservados como referencia;
seus controles e a escolha Marte/Jupiter nao se aplicam ao fluxo robusto.

---

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
dotnet build "E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA\JogoPlanetarioUFBA.sln"
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --path "E:\Projetos\PlanetarioUFBA\JogoPlanetarioUFBA"
```

Observacao: use a edicao .NET/C# da Godot. O build sem `mono` nao carrega scripts `.cs`.

## Controles principais

- `W`: avancar pela rota on-rails.
- `S`: recuar pela rota on-rails.
- Soltar `W`/`S`: a nave desacelera ate parar.
- `A`/`D` ou setas esquerda/direita: olhar para os lados.
- Setas cima/baixo: olhar para cima/baixo.
- Botao direito do mouse pressionado: olhar livremente; soltar ou `Esc` libera o cursor.
- `R`: recentralizar o olhar suavemente.
- `Tab` ou botao ESTACAO: abrir/fechar os controles do passageiro.
- `F3`: mostrar/ocultar o debug.
- No ponto de salto, escolha MARTE/JUPITER no HUD, use `1`/`2` ou ACAO 1/2 da estacao.
- O salto carrega o destino escolhido; o olhar volta a ficar livre ao chegar.
- Ao chegar a Marte ou Jupiter, VOLTAR A ROTA INICIAL retorna ao inicio do percurso. Avance com `W` para escolher um destino novamente. `Enter` ou CONFIRMAR na estacao tambem reinicia o segmento.

A nave nao controla direcao lateral. Ela permanece presa a uma trajetoria predefinida; o piloto controla ritmo e camera.

## Validacao

```powershell
dotnet build .\JogoPlanetarioUFBA.csproj
dotnet run --project .\Testes\TestesDominio.csproj
E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe --headless --path . --quit-after 3
```

O build C#, os testes de dominio e a carga headless da cena foram validados com a edicao .NET/C# da Godot.

## Escopo atual

A arquitetura de cenarios esta documentada em [SCENARIOS.md](SCENARIOS.md).
Ha sete regioes independentes preparadas e um ambiente tecnico `prototipo`
que preserva a inicializacao atual. A troca de ambiente mantem nave, camera,
HUD e estado da viagem; os eventos futuros das regioes ainda nao foram criados.

- Perfil funcional `DesktopDev`.
- Adaptador `AdaptadorTecladoMouse` lendo acoes logicas do Input Map.
- Cena principal `CenaNavegacaoOnRails`.
- Cabine 3D de onibus espacial com cobertura panoramica, janelas e portas envidracadas, corredor e quatro assentos.
- Portas e assentos sao elementos visuais, ainda sem embarque ou animacao de abertura.
- Camera de primeira pessoa suavizada, independente da estrutura da nave.
- Navegacao on-rails com curvas Bezier continuas e orientacao suavizada separada de `PathFollow3D`.
- HUD compacto com instrumentos e estacao recolhivel.
- Planeta oceanico com continentes, nuvens e atmosfera; gigante gasoso com aneis.
- Avanco, re, parada e camera livre limitada.
- Assets espaciais iniciais integrados de Kenney Space Kit e Quaternius Ultimate Spaceships.
- Cometa visual com meteoros Kenney e cauda emissiva simples.
- Skybox 360 realista com panorama equiretangular via `PanoramaSkyMaterial`.
- Marcadores narrativos por progresso fisico.
- Salto em fases com rastros 3D e escolha de Marte/Jupiter; detalhes em [SALTO_MARTE_JUPITER.md](Documentacao/SALTO_MARTE_JUPITER.md).
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
