# Cabine panoramica e revisao visual

A cena principal agora representa um onibus espacial. Uma faixa curva de vidro
une o para-brisa ao teto, com estruturas finas nas bordas, sem travessa sobre o
piloto. As laterais possuem janelas amplas e portas duplas envidracadas com
puxadores e soleiras iluminadas. Ha quatro assentos e um corredor traseiro.
As portas sao cenograficas: ainda nao abrem e nao existe sistema de embarque.

O vidro usa transparencia discreta, sem refracao ou reflexos intensos que
atrapalhem a observacao. O panorama espacial existente foi preservado.
Esta revisao nao implementa correcao fisheye nem calibracao de domo; a porta
de projecao continua preservada para essa integracao futura.

## Camera, rota e instrumentos

- Olhar horizontal limitado a 125 graus por lado, vertical a 82 graus.
- Mouse com botao direito, setas e A/D; R recentraliza e Esc libera o cursor.
- Olhar suavizado exponencialmente, sem balanco artificial do casco.
- Curva Bezier com tangentes compartilhadas e orientacao em no separado do
  PathFollow3D, evitando que atualizar o progresso reinicie a rotacao.
- Travessia em aproximadamente 31 segundos na velocidade maxima.
- HUD com energia, propulsao, casco, progresso e narrativa; Tab abre a estacao,
  F3 mostra o debug, Enter reinicia depois da conclusao.
- Planeta oceanico procedural com nuvens, calotas e atmosfera; gigante gasoso
  com aneis. Sao visuais de prototipo, nao representacoes astronomicas precisas.

## Validacao

Build e testes de dominio aprovados. O teste integrado executa com o renderizador
OpenGL da Godot, captura a frente, teto, portas, estacao, resolucoes 960x540 e
1600x720, deslocamento, conclusao e reinicio. Verifica pixels nao uniformes,
direcao da camera, continuidade da rota, re e reinicio. Capturas inspecionadas
em `.godot/validacao_visual/` (pasta gerada, fora do versionamento).

```powershell
dotnet build .\JogoPlanetarioUFBA.sln
dotnet run --project .\Testes\TestesDominio.csproj
& "E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe" --path . --script res://Testes/ValidarApresentacao.gd --fixed-fps 60 --disable-vsync
```

Resultado: zero falhas. Maior variacao de tangente entre amostras: 0,0621 grau;
maior giro por quadro a 60 quadros simulados/segundo: 0,0685 grau.
