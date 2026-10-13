# Fontes, licencas e placeholders

Revisao para GitHub em 2026-10-13: fontes dos assets presentes identificadas;
nenhum asset com ORIGEM A CONFIRMAR encontrado. Sem novos downloads nesta rodada.
Kenney: CC0 com License.txt local; Quaternius: CC0 conforme documentacao herdada;
Solar System Scope: CC BY 4.0, manter credito abaixo; panorama: termos proprios
do autor, nao rotular todo o pacote como CC0. Fontes Quaternius e modelos legados
estao detalhados em Documentacao/ASSETS_3D.md. A equipe ainda precisa definir
uma licenca geral para seu codigo; essas licencas cobrem somente os respectivos assets.

Consulta/download: 2026-10-05. Nenhum asset comercial sem licenca foi utilizado.

## Novos downloads

Autor de todas as tres texturas: **Solar System Scope / INOVE**.
Licenca: **Creative Commons Attribution 4.0 International (CC BY 4.0)**.
[Licenca](https://creativecommons.org/licenses/by/4.0/).
[Fonte e termos](https://ssi.solarsystemscope.com/textures/).
As texturas usam dados/imagens da NASA com tratamento artistico do autor;
nao devem ser apresentadas como mosaicos cientificos sem alteracoes.

| Recurso | Arquivo local | URL de download |
| --- | --- | --- |
| Earth Day Map 2K | assets/robusto/2k_earth_daymap.jpg | https://ssi.solarsystemscope.com/textures/download/2k_earth_daymap.jpg |
| Uranus 2K | assets/robusto/2k_uranus.jpg | https://ssi.solarsystemscope.com/textures/download/2k_uranus.jpg |
| Mars 2K | assets/robusto/2k_mars.jpg | https://upload.wikimedia.org/wikipedia/commons/4/46/Solarsystemscope_texture_2k_mars.jpg |

Todos sao 2048 x 1024. Arquivos recebidos sem edicao; aplicados a SphereMesh,
com iluminacao e orientacao na cena. O download direto de Marte retornou 403;
foi usada a mesma obra publicada no Wikimedia Commons, com autoria e licenca:
https://commons.wikimedia.org/wiki/File:Solarsystemscope_texture_2k_mars.jpg

Credito a manter em redistribuicoes: "Planet textures: Solar System Scope,
CC BY 4.0, https://www.solarsystemscope.com/textures/; rendered with scene lighting."

## Reaproveitados da copia

| Recurso | Origem / autor | Licenca e arquivo |
| --- | --- | --- |
| Nave pirata | https://kenney.nl/assets/space-kit / Kenney | CC0; assets/espaco/kenney/space_kit/modelos/craft_speederA.glb |
| Corpo gelado | mesma fonte / Kenney | CC0; assets/espaco/kenney/space_kit/modelos/rock_crystalsLargeA.glb |
| Panorama espacial | https://space-spheremaps.itch.io/space-spheremaps / Space Spheremaps | Termos proprios, uso em prototipos/comercial permitido conforme documentacao herdada; assets/espaco/space_spheremaps/hazy_nebulae_1.png |

A licenca local Kenney esta em `assets/espaco/kenney/space_kit/License.txt`.
O README do panorama preserva os termos registrados na integracao original,
incluindo a restricao de AI scraping. Nao foi baixado outro panorama nesta etapa.
Outros assets herdados, incluindo Quaternius, permanecem na copia mas nao sao
carregados pela viagem robusta. Seus creditos estao em `Documentacao/ASSETS_3D.md`.

## Arte provisoria/local

- Ana Branca: superficie procedural emissiva e halo originais do projeto,
  atualizados em 2026-10-07; nao sao textura NASA nem simulacao cientifica.
- Chegada: esfera provisoria para um destino a definir.
- Aneis de Urano: TorusMesh fino, ilustrativo, nao modelo cientifico dos aneis.
- Gelo e piratas: modelos Kenney com escala, centralizacao e material adaptados.
- Cockpit e rastros: geometria/shaders do proprio prototipo, reutilizados.
- Nenhum audio, voz, fonte externa ou textura gerada por IA foi adicionado.

O conjunto melhora leitura visual; escalas, distancias, cores e roteiro ainda
precisam de curadoria cientifica antes de uma apresentacao educativa definitiva.

## Rodada visual e camera - 2026-10-07

Nenhum asset externo novo foi baixado. `SuperficieEstelar.gdshader` combina
ruido 3D em camadas, emissao e fluxo lento; `HaloEstelar.gdshader` usa uma esfera
aditiva transparente. Sao codigo original desta rodada, sem arquivo de terceiros
ou nova licenca externa. Os demais mapas e modelos preservam os creditos acima.

Referencias visuais/contextuais consultadas, sem incorporar imagens:
- https://science.nasa.gov/asset/hubble/white-dwarf-merger-illustration/
- https://science.nasa.gov/missions/hubble/nasas-hubble-uncovers-rare-white-dwarf-merger-remnant/

A granulacao, halo e escalas sao escolhas artisticas para o prototipo, nao uma
reconstrucao observacional da superficie de uma ana branca.
