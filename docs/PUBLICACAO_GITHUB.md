# Preparacao da versao robusta para a equipe

Data: 2026-10-13. Trabalho realizado somente em `E:\Godot\GAME_robusto`.
O prototipo antigo nao foi editado. Nao houve refatoracao ou alteracao de gameplay,
camera, cenas, shaders, roteiro ou assets nesta preparacao. O commit agrega tambem
as alteracoes robustas de etapas anteriores que ainda estavam sem commit.

## Repositorio

- Origin preservado: https://github.com/JoaoGoncalvesPaesNunes/JogoPlanetarioUFBA.git
- Branch dedicada existente: `codex/prototipo-robusto`.
- Repositorio ja era publico. Nao foi criado repositorio nem alterada visibilidade.
- Main e branches antigas preservadas; sem force push ou reescrita de historico.
- Mensagem prevista: `feat: adiciona versao robusta do prototipo e guia da equipe`.
- O hash e o resultado do push sao informados na entrega e verificaveis por
  `git log -1 --oneline` e `git ls-remote origin refs/heads/codex/prototipo-robusto`.
  Este arquivo registra a revisao anterior ao commit, nao antecipa sucesso do envio.

## Ambiente confirmado

Godot 4.7.1.stable.mono.official.a13da4feb; SDK Godot.NET.Sdk/4.7.1;
TargetFramework net8.0; SDK .NET 10.0.400; runtime8.0.13 para testes de console.
Windows x64, renderer Compatibility/OpenGL, NVIDIA GTX1650. C# principal,
Godot shaders e GDScript no teste visual legado. Cena principal configurada:
`res://Robusto/Cenas/Experiencia.tscn`. Sem Autoloads, addons, submodulos,
backend ou servicos autenticados exigidos pelo jogo.

## Documentacao e organizacao

- README.md refeito para quem vai clonar pela primeira vez, sem caminhos de
  execucao fixos da maquina original e sem misturar controles do legado.
- docs/DEVELOPMENT_GUIDE.md criado com arquitetura, edicao, debug e testes.
- Este relatorio criado; CONTRIBUTING.md e ASSET_SOURCES.md atualizados.
- README anterior preservado em Documentacao/README_LEGADO.md, como historico.
- Documentos de marcos anteriores preservados, inclusive seus registros locais.

.gitignore manteve suas regras e ganhou builds/exports na raiz, TestResults,
logs, .DS_Store, .env/.env.* (exceto .env.example) e pastas locais .aws/.ssh/.azure.
Nao se ignoram cenas, scripts, assets, .uid ou receitas .import. .godot, bin/obj,
IDE e downloads ja eram ignorados. Nenhum cache rastreado precisou ser removido.
export_presets.cfg continua ignorado; nao existe exportacao final validada.

## Tamanho, segredos e licencas

Maiores arquivos candidatos:

| Arquivo | Bytes |
| --- | ---: |
| assets/espaco/space_spheremaps/hazy_nebulae_1.png | 7618702 |
| assets/espaco/quaternius/ultimate_spaceships/modelos/Spitfire.gltf | 4716363 |
| assets/espaco/quaternius/ultimate_spaceships/modelos/Spitfire_Spitfire_Orange.png | 3382493 |
| assets/robusto/2k_mars.jpg | 750547 |

Nenhum candidato nem blob do historico inspecionado excede50MiB. Git LFS nao foi
introduzido. O [GitHub bloqueia arquivos acima de100MiB](https://docs.github.com/en/repositories/working-with-files/managing-large-files/about-large-files-on-github).

A busca por padroes de tokens, chaves privadas, senhas atribuidas e nomes de
arquivos de credenciais nao encontrou segredo evidente nos candidatos. Isso e
uma revisao heuristica, nao garantia absoluta de ausencia de segredos.
Credenciais do GitHub CLI ficam no gerenciador do sistema, fora do repositorio.

Todos os assets presentes possuem fontes identificadas na documentacao: Kenney
e Quaternius CC0; Solar System Scope CC BY4.0; panorama com termos proprios.
Termos de redistribuicao do panorama confirmados na
[pagina do autor](https://space-spheremaps.itch.io/space-spheremaps), incluindo a
restricao a AI scraping. Nao ha assets novos nem ORIGEM A CONFIRMAR nesta revisao.
A equipe ainda deve escolher licenca geral do codigo, sem confundir com licencas
dos assets. Nenhuma licenca foi inventada ou aplicada ao projeto inteiro.

## Validacao de copia limpa

Foi criada uma pasta de auditoria separada:
`E:\Godot\validacao_clone_robusto_20261013`, usando somente arquivos retornados
por Git (rastreados e novos nao ignorados), sem .git, .godot, bin/obj ou DLLs.
Nao e uma nova versao de desenvolvimento; nao e o prototipo antigo.

- Build na copia limpa: zero erros/avisos, pacotes restaurados.
- Testes de dominio: aprovados.
- Importacao headless do editor: assets importados do zero.
- Viagem completa headless: 47 verificacoes aprovadas.
- Viagem completa com render: 68 verificacoes aprovadas, zero falhas.
- Camera/cockpit com render: 87 verificacoes aprovadas, incluindo Normal,
  fullscreen e Fulldome/grade. Superficie estelar:0/256 amostras saturadas.
- Referencias res:// verificadas sem recursos ausentes.
- Busca em codigo/cenas/recursos: sem dependencia de caminhos absolutos locais.

Logs e capturas ficam em .godot da pasta de auditoria, fora do commit. O teste
usa os SDKs instalados e o cache NuGet do computador; nao e uma maquina virtual
sem dependencias. Nao certifica Linux/macOS, arcade fisico ou planetario real.
Nao instala programas nem produz executavel exportado. Fulldome continua
experimental, sem warp/blending ou garantia de desempenho na sala.

## Problemas encontrados

O README anterior misturava comandos do prototipo antigo e da versao robusta:
foi substituido e preservado como historico. Sem correcao de codigo necessaria.
A primeira tentativa restrita de build falhou sem diagnostico; repetida com
acesso ao SDK/NuGet, compilou normalmente. gh auth status no sandbox tambem
informou token invalido; com acesso autorizado ao gerenciador de credenciais,
confirmou autenticacao valida. Nenhuma credencial foi alterada.

O conjunto a publicar inclui codigo/cenas robustas, componentes herdados dos
quais dependem, testes, tres mapas planetarios, UIDs/receitas e documentacao.
Arquivos gerados, pessoais e de autenticacao nao integram esse conjunto.
Use `git show --stat` e `git show --name-only` para a lista exata do commit.
