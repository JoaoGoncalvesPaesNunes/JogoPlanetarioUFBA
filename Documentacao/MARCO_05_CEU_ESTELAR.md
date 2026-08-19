# Marco 05 - Ceu Estelar Procedural

Este incremento adiciona um skydome procedural ao prototipo on-rails, sem baixar novos arquivos externos.

## Objetivo

Melhorar a sensacao de espaco profundo com um fundo mais bonito que o preto plano e as poucas estrelas estaticas do cenario de assets.

## Implementacao

- `res://Apresentacao/Cenas/CeuEstelarMarco05.tscn` e a subcena carregada pela cena principal.
- `res://Apresentacao/Cenas/CeuEstelar.cs` gera:
  - um domo escuro grande;
  - uma camada de estrelas profundas;
  - uma faixa galactica sutil;
  - variacao de brilho e cor entre estrelas frias, brancas e douradas.

As estrelas usam `MultiMeshInstance3D` para reduzir custo de renderizacao e evitar centenas de nos individuais na arvore da cena.

## Decisoes

- O ceu e deterministico por semente fixa, entao nao muda a cada execucao.
- Nao foi usado asset externo neste passo, evitando nova dependencia de licenca.
- O domo fica separado do cenario, com cena propria, para facilitar troca futura por skybox HDRI, panorama 8K ou textura de dome.

## Limitacoes

- Ainda nao e um skybox astronomicamente real.
- A faixa galactica e estilizada, nao uma textura da Via Lactea.
- Para exibicao final no planetario, o proximo passo visual deve testar contraste e legibilidade no projetor/domo.
