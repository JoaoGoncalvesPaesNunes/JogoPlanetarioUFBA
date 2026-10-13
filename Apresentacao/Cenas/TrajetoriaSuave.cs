using Godot;

namespace ProjetoPlanetario.Apresentacao.Cenas;

public static class TrajetoriaSuave
{
    public static Curve3D Criar()
    {
        var pontos = new Vector3[]
        {
            new(0f, 0f, 0f), new(1.2f, 0.2f, -12f),
            new(-0.6f, 0.45f, -26f), new(0.5f, 0.1f, -40f)
        };
        var curva = new Curve3D { BakeInterval = 0.08f };
        for (var indice = 0; indice < pontos.Length; indice++)
        {
            // Tangentes compartilhadas mantem a direcao continua entre os trechos Bezier.
            var anterior = pontos[Mathf.Max(0, indice - 1)];
            var seguinte = pontos[Mathf.Min(pontos.Length - 1, indice + 1)];
            var tangente = (seguinte - anterior) / (indice == 0 || indice == pontos.Length - 1 ? 3f : 6f);
            curva.AddPoint(pontos[indice], -tangente, tangente);
        }
        return curva;
    }

    public static Basis Orientacao(Curve3D curva, float distancia)
    {
        var antes = curva.SampleBaked(Mathf.Max(0f, distancia - 0.6f), true);
        var depois = curva.SampleBaked(Mathf.Min(curva.GetBakedLength(), distancia + 0.6f), true);
        return Basis.LookingAt((depois - antes).Normalized(), Vector3.Up);
    }
}
