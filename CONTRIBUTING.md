# Contribuindo

Este repositorio esta em fase de fundacao.

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

Quando possivel, tambem abra a cena principal na Godot e verifique o microprototipo.
