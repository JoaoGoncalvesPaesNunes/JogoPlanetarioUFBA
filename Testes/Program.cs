using System;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

var estadoDaNave = new EstadoDaNave();
Garantir(estadoDaNave.Energia == 100f, "Energia inicial deve ser 100.");
Garantir(estadoDaNave.ConsumirEnergia(10f), "Consumo de energia positivo deve alterar estado.");
Garantir(Math.Abs(estadoDaNave.Energia - 90f) < 0.001f, "Energia deve cair para 90.");
Garantir(!estadoDaNave.ConsumirEnergia(0f), "Consumo zero nao deve alterar estado.");
Garantir(estadoDaNave.AlterarEstabilidadeDaDobra(50f), "Estabilidade deve aceitar variacao positiva.");
Garantir(Math.Abs(estadoDaNave.EstabilidadeDaDobra - 100f) < 0.001f, "Estabilidade deve respeitar limite superior.");
Garantir(estadoDaNave.AlterarEstabilidadeDaDobra(-120f), "Estabilidade deve aceitar variacao negativa.");
Garantir(Math.Abs(estadoDaNave.EstabilidadeDaDobra) < 0.001f, "Estabilidade deve respeitar limite inferior.");

var barramento = new BarramentoDeEventos();
EventoDeDominio? eventoRecebido = null;
barramento.EventoPublicado += evento => eventoRecebido = evento;
barramento.Publicar(new EnergiaAlterada(estadoDaNave.Energia, "Teste", DateTime.UtcNow));
Garantir(eventoRecebido is EnergiaAlterada, "Barramento deve entregar evento publicado.");

Console.WriteLine("Testes de dominio concluidos com sucesso.");

static void Garantir(bool condicao, string mensagem)
{
    if (!condicao)
    {
        throw new InvalidOperationException(mensagem);
    }
}
