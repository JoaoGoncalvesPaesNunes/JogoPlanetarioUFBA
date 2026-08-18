# Marco 03 - Navegacao on-rails, cometa e salto

## Objetivo

Transformar a experiencia principal em uma navegacao presa a trilho, com avanco/re, camera livre limitada, passagem por cometa e evento final de salto gravitacional.

## Fluxo

1. A cena `CenaNavegacaoOnRails` carrega o perfil `DesktopDev`.
2. A nave inicia no começo de uma `Path3D`.
3. `W` avanca pela rota; `S` recua; sem input a nave desacelera.
4. `A`/`D` e setas movem a camera, sem alterar a trajetoria.
5. Marcadores de rota disparam eventos narrativos uma unica vez.
6. ORION anuncia aproximacao e passagem pelo cometa.
7. Ao final, ORION avisa anomalia gravitacional.
8. O salto dispara flash/fade textual.
9. O segmento conclui.
10. `CONFIRMAR` na estacao virtual reinicia o teste.

## Controles

- `W`: avancar.
- `S`: recuar.
- `A`/`D`: olhar para esquerda/direita.
- Setas: olhar horizontal/vertical.
- Estacao virtual `CONFIRMAR`: reiniciar depois da conclusao.

## Arquitetura

- `SegmentoDeViagem`: progresso fisico e marcadores narrativos.
- `MarcadorDeRota`: ponto de disparo configuravel, com `ExecutarUmaVez`.
- `ControladorDeVoo`: entrada logica, velocidade na rota e camera limitada.
- `CenaNavegacaoOnRails`: integra `Path3D`, `PathFollow3D`, cenario, cometa e eventos.
- `ApresentacaoDoSalto`: overlay de flash/fade acionado por evento.
- `DiretorNarrativo`: ORION reage a eventos do segmento.

## Comportamento de re

Dar re reduz `ProgressoNaRota`, mas nao rearma marcadores ja executados. Ao cruzar novamente a aproximacao/passagem do cometa, as falas nao repetem. Reiniciar o segmento rearma os marcadores.

## Placeholders

O cometa usa esfera e cauda simples. O salto usa overlay branco e texto. A rota, estrelas e planeta sao graybox. Nao ha arte final, audio, fisica orbital, combate ou projecao real de domo.

## Validacao

Executar:

```powershell
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe --headless --path . --quit-after 3
```

O teste visual completo deve ser feito no editor/janela da Godot .NET.
