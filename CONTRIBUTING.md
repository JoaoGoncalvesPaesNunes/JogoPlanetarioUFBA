# Contribuindo

Esta branch contem a versao robusta. Leia README.md e
docs/DEVELOPMENT_GUIDE.md antes de alterar cenas ou sistemas.

## Padroes

- C# em portugues, sem acentos em identificadores.
- Classes pequenas e nomes claros.
- Eventos tipados para comunicacao entre sistemas.
- Entrada fisica sempre passa por adaptadores.
- Logica de dominio deve ser testavel quando pratico.

## Antes de enviar mudancas

Execute:

```powershell
dotnet build .\JogoPlanetarioUFBA.csproj
dotnet run --project .\Testes\TestesDominio.csproj
```

Tambem execute os testes de integracao indicados no guia e confira a cena
Robusto/Cenas/Experiencia.tscn. Nao envie caches, builds ou segredos.
