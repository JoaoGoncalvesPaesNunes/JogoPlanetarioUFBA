# ADR 0001 - Entrada logica antes de hardware

## Contexto

O jogo devera rodar em desktop durante o desenvolvimento e futuramente aceitar arcade sticks, gamepads, paineis fisicos e dispositivos assistivos.

## Decisao

A jogabilidade consome portas logicas (`IEntradaDoPiloto` e `IEntradaDeEstacao`). Dispositivos concretos ficam em adaptadores.

## Consequencias

- O controlador da nave nao conhece `W`, `Espaco`, mouse ou botoes fisicos.
- Novos dispositivos podem ser adicionados sem reescrever mecanicas principais.
- A configuracao de hardware fica fora do gameplay.
