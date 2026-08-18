# ADR 0002 - Projecao desacoplada da jogabilidade

## Contexto

O ambiente final do planetario ainda nao tem especificacao tecnica suficiente para implementar warping, area real de domo ou calibracao.

## Decisao

Criar `IPortaDeProjecao` com um adaptador desktop funcional e um stub para planetario.

## Consequencias

- O microprototipo roda em monitor convencional.
- A jogabilidade nao depende da geometria de exibicao.
- A futura integracao com o planetario fica localizada em um adaptador especifico.
