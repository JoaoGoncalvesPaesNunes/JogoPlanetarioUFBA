# Relatorio de ambiente

Data local: 2026-08-18.

## Versoes encontradas

- Godot .NET/C#: `4.7.1.stable.mono.official.a13da4feb`.
- Executavel testado: `E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe`.
- Godot sem C#: `4.7.1.stable.official.a13da4feb`, encontrado anteriormente em `E:\Godot\Godot_v4.7.1-stable_win64_console.exe`.
- .NET SDK: `10.0.400`.
- Runtimes .NET disponiveis: 6.0.11, 8.0.13 e 10.0.11.

## Validacoes realizadas

- `dotnet build .\JogoPlanetarioUFBA.sln`: sucesso, 0 avisos, 0 erros.
- `dotnet run --project .\Testes\TestesDominio.csproj`: sucesso.
- `E:\Godot\Godot_v4.7.1-stable_mono_win64_console.exe --headless --path . --quit-after 3`: sucesso.

## Diagnostico

A edicao .NET/C# da Godot esta disponivel e carregou a cena principal em modo headless.

O build sem `mono` nao deve ser usado para executar este projeto, pois nao possui loader para scripts C#. O erro observado anteriormente nesse build foi:

```text
No loader found for resource: res://Apresentacao/Cenas/CenaMicroPrototipo.cs (expected type: Script)
```

Use a edicao `stable_mono` para execucao visual e testes pela Godot.
