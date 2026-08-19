# Assets 3D - Marco 04

Este marco troca os marcadores visuais temporarios por um conjunto pequeno de assets espaciais verificaveis, mantendo a navegacao on-rails e o loop central existentes.

## Fontes

### Kenney Space Kit

- URL oficial: https://kenney.nl/assets/space-kit
- Licenca informada na pagina: Creative Commons CC0
- Licenca local preservada em: `res://assets/espaco/kenney/space_kit/License.txt`
- Formato integrado: GLB
- Uso no projeto: cometa, campo de asteroides, nave secundaria e antena de pesquisa.

Arquivos versionados:

- `res://assets/espaco/kenney/space_kit/modelos/craft_speederA.glb`
- `res://assets/espaco/kenney/space_kit/modelos/meteor_detailed.glb`
- `res://assets/espaco/kenney/space_kit/modelos/meteor_half.glb`
- `res://assets/espaco/kenney/space_kit/modelos/rock_crystalsLargeA.glb`
- `res://assets/espaco/kenney/space_kit/modelos/rock_largeA.glb`
- `res://assets/espaco/kenney/space_kit/modelos/rock_largeB.glb`
- `res://assets/espaco/kenney/space_kit/modelos/satelliteDish_large.glb`

### Quaternius Ultimate Spaceships

- URL oficial: https://quaternius.com/packs/ultimatespaceships.html
- Licenca informada na pagina: CC0
- Formato integrado: glTF autocontido
- Uso no projeto: nave Spitfire como elemento de escala no cenario.

Arquivo versionado:

- `res://assets/espaco/quaternius/ultimate_spaceships/modelos/Spitfire.gltf`
- `res://assets/espaco/quaternius/ultimate_spaceships/modelos/Spitfire_Spitfire_Orange.png`, extraido pela Godot durante a importacao.

O download da Quaternius foi feito pelo Google Drive publico aberto a partir do botao oficial da pagina do autor. Nao foram usados mirrors de terceiros.

## Integracao na cena

- `res://Apresentacao/Cenas/CenarioEspacialMarco04.tscn` contem planeta distante, asteroides, nave Kenney, antena e nave Spitfire Quaternius.
- `res://Apresentacao/Cenas/CometaMarco04.tscn` contem o nucleo do cometa com meteoros Kenney e uma cauda emissiva simples.
- `CenaNavegacaoOnRails.cs` carrega essas subcenas no `_Ready`, sem alterar a rota, o `PathFollow3D`, o HUD, a estacao virtual ou os eventos narrativos.

## Remocoes e substituicoes

- Removido o campo antigo de 140 pontos brancos gerados por codigo.
- Removido o `PlanetaDistanteGraybox` criado por codigo.
- Removidos os nomes `CometaPlaceholder` e `CaudaCometaPlaceholder` da cena principal.
- Mantidas pequenas estrelas estaticas e coloridas dentro da subcena apenas para profundidade visual.

## Limitacoes atuais

- O planeta ainda e uma esfera estilizada local, nao um modelo planetario final de fonte externa.
- A cauda do cometa ainda usa uma malha cilindrica simples com material emissivo, porque o pacote integrado nao traz um efeito VFX pronto.
- As naves ainda sao elementos de cenario; nao ha IA, animacao de frota ou colisao.
- Iluminacao foi melhorada para leitura do prototipo, mas ainda nao e iluminacao final para dome/planetario.

## Proximos passos sugeridos

1. Criar um skybox ou panorama espacial licenciado para substituir as estrelas estaticas.
2. Adicionar materiais finais para planetas e cometa.
3. Criar LODs ou reduzir instancias se a cena crescer para exibicao em dome.
4. Separar um prefab de nave do jogador quando o cockpit deixar de ser somente HUD/graybox.
