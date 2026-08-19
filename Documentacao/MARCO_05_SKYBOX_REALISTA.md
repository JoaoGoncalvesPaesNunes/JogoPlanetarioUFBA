# Marco 05 - Skybox Realista 360

Este marco substitui o ceu procedural por um panorama espacial realista configurado no sistema de Sky da Godot.

## Asset Escolhido

- Nome: Space Spheremaps
- Criador: Space Spheremaps
- Pagina oficial: https://space-spheremaps.itch.io/space-spheremaps
- Arquivo escolhido: `hazy_nebulae_1.png`
- Caminho no projeto: `res://assets/espaco/space_spheremaps/hazy_nebulae_1.png`
- Formato: PNG
- Resolucao: 8192 x 4096
- Proporcao: 2:1
- Tamanho local: 7.618.702 bytes

## Licenca e Termos

A pagina oficial informa que o pacote contem quatro mapas esfericos equiretangulares/panoramicos em PNG 8192 x 4096, gratuitos para uso pessoal, prototipagem e projetos comerciais.

Em comentario do autor na propria pagina, os termos sao descritos como similares a CC BY 4.0 para compartilhar e adaptar, inclusive comercialmente, mas sem exigencia de atribuicao. O autor tambem informa uma restricao: as imagens nao devem ser usadas para AI scraping. Atribuicao e apreciada, mas nao obrigatoria conforme o texto atual.

## Arquivos do Pacote

O pacote oficial baixado continha 4 imagens:

- `blue_nebulae_1.png`
- `hazy_nebulae_1.png`
- `plain_starfield_1.png`
- `ringed_gas_giant_planet.png`

Foi escolhido `hazy_nebulae_1.png` porque e escuro, sutil, nao contem planeta gigante, nao tem nave/estacao/texto e tende a preservar a leitura do HUD e cockpit.

## Implementacao Godot

Implementacao aplicada em `CenaNavegacaoOnRails.cs`:

```text
WorldEnvironment
    Environment
        BackgroundMode = Sky
        Sky = Sky
            SkyMaterial = PanoramaSkyMaterial
                Panorama = hazy_nebulae_1.png
```

O panorama nao foi aplicado como plano, sprite, quad ou esfera gigante. Ele fica no sistema de fundo da Godot e nao deve gerar parallax quando a nave avanca ou recua pela rota.

## Remocoes

- Removido o skydome procedural `CeuEstelar`.
- Removidas as estrelas estaticas pequenas da subcena `CenarioEspacialMarco04`.
- Mantidos separadamente os objetos 3D proximos: planeta estilizado, asteroides, cometa, naves e antena.

## Verificacao

- Build C# executado.
- Testes de dominio executados.
- Cena carregada via Godot headless.
- Tentativa de captura frente/tras/cima/baixo em headless: nao conclusiva, pois o backend headless retornou textura de viewport nula.

Itens verificados por implementacao:

- Skybox/panorama realista configurado no `WorldEnvironment`.
- Uso de `PanoramaSkyMaterial`.
- Nenhum plano/QuadMesh/Sprite3D usado como fundo principal.
- Nenhuma esfera low-poly usada como fundo principal.
- Pontos procedurais antigos removidos como ambientacao principal.
- Objetos 3D proximos continuam independentes.

## Limitacoes

A validacao headless confirma carga tecnica, mas nao substitui uma inspecao visual manual no editor/janela do jogo. Recomenda-se abrir a cena na Godot e girar a camera para confirmar ausencia de costuras perceptiveis em tela cheia.
