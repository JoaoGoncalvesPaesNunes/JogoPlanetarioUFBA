# ADR 0003 - Navegacao on-rails

## Contexto

O jogo sera usado em uma experiencia de planetario com duracao previsivel, narrativa sincronizada e publico diverso. A pilotagem espacial livre aumentaria risco de desorientacao, dificultaria enquadramentos e tornaria eventos narrativos mais dificeis de coreografar.

## Decisao

O piloto nao controla a trajetoria espacial livre da nave. A nave segue um segmento predefinido usando `Path3D`/`PathFollow3D`. O piloto controla avanco, re, parada e orientacao limitada da camera.

## Beneficios

- Mantem a sessao dentro de uma duracao controlavel.
- Facilita sincronizar ORION, dublagem futura e eventos de cena.
- Preserva agencia do piloto sobre ritmo e observacao.
- Reduz complexidade de IA e navegacao espacial.
- Ajuda acessibilidade ao evitar movimentos bruscos e 6DOF.
- Permite coreografar encontros como cometas, planetas e combates futuros.

## Trade-offs

- O piloto perde liberdade de sair da rota.
- A sensacao de exploracao depende mais de camera, ritmo e eventos.
- Segmentos precisam ser bem desenhados para nao parecerem passivos.

## Consequencias

Progresso fisico e progresso narrativo ficam separados. Dar re move a nave na rota, mas nao desfaz falas, eventos disparados ou estado narrativo. Marcadores podem ser configurados como `ExecutarUmaVez` para impedir repeticao indevida.
