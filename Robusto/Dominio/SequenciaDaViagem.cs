using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoPlanetario.Robusto.Dominio;

public sealed record EtapaDaViagem
{
    public string Id { get; init; } = "";
    public string Cenario { get; init; } = "";
    public string Tipo { get; init; } = "observacao";
    public string Titulo { get; init; } = "";
    public string Texto { get; init; } = "";
    public string Audio { get; init; } = "";
    public float Duracao { get; init; } = 10;
    public float Velocidade { get; init; } = 1;
}

/// <summary>Uma unica maquina de sequencia. Eventos e saltos precisam confirmar conclusao.</summary>
public sealed class SequenciaDaViagem
{
    public IReadOnlyList<EtapaDaViagem> Etapas { get; }
    public int Indice { get; private set; }
    public float Tempo { get; private set; }
    public EtapaDaViagem Atual => Etapas[Indice];
    public bool Terminou => Atual.Tipo == "chegada";
    public event Action<EtapaDaViagem>? Alterada;

    public SequenciaDaViagem(IReadOnlyList<EtapaDaViagem> etapas)
    {
        if (etapas.Count == 0 || etapas.Any(e => string.IsNullOrWhiteSpace(e.Id) ||
            string.IsNullOrWhiteSpace(e.Cenario) || !float.IsFinite(e.Duracao) || e.Duracao <= 0 ||
            !float.IsFinite(e.Velocidade) || e.Velocidade < 0 ||
            e.Tipo is not ("observacao" or "salto" or "combate" or "gelo" or "chegada")) ||
            etapas.Select(e => e.Id).Distinct().Count() != etapas.Count)
            throw new ArgumentException("Roteiro vazio, duplicado ou invalido.");
        Etapas = etapas;
    }

    public void Atualizar(float delta, bool eventoConcluido, bool permitirAvanco = true)
    {
        if (!float.IsFinite(delta) || delta <= 0 || Terminou) return;
        Tempo += delta;
        if ((Atual.Tipo == "observacao" && Tempo >= Atual.Duracao && permitirAvanco) ||
            (Atual.Tipo is "salto" or "combate" or "gelo" && eventoConcluido)) Avancar();
    }

    public void IrPara(string id)
    {
        var indice = Etapas.ToList().FindIndex(e => e.Id == id);
        if (indice < 0) throw new ArgumentException("Etapa inexistente: " + id);
        Indice = indice;
        Tempo = 0;
        Alterada?.Invoke(Atual);
    }

    private void Avancar()
    {
        if (Indice >= Etapas.Count - 1) return;
        Indice++;
        Tempo = 0;
        Alterada?.Invoke(Atual);
    }
}
