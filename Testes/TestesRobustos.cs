using System;
using ProjetoPlanetario.Robusto.Dominio;
using ProjetoPlanetario.Jogabilidade.Viagem;

public static class TestesRobustos
{
    public static void Executar()
    {
        var olhar = new EstadoDoOlhar();
        olhar.Mover(10000, 10000);
        Exigir(olhar.YawAlvo == 115 && olhar.PitchAlvo == 70, "Limites positivos da visao");
        olhar.Atualizar(0.1f);
        Exigir(MathF.Sqrt(olhar.Yaw * olhar.Yaw + olhar.Pitch * olhar.Pitch) <= 4.001f, "Velocidade angular limitada inclusive na diagonal");
        for (var i = 0; i < 600; i++) olhar.Atualizar(1f / 60);
        Exigir(Math.Abs(olhar.Yaw - 115) < 0.01f, "Olhar converge sem ultrapassar limite");
        var yawAnterior = olhar.Yaw;
        olhar.Recentralizar();
        Exigir(olhar.Yaw == yawAnterior, "Recentrar nao teleporta a visao");
        for (var i = 0; i < 600; i++) olhar.Atualizar(1f / 60);
        Exigir(Math.Abs(olhar.Yaw) < 0.01f && Math.Abs(olhar.Pitch) < 0.01f, "Retorno suave ao centro");
        olhar.Mover(-10000, -10000);
        Exigir(olhar.YawAlvo == -115 && olhar.PitchAlvo == -50, "Limites negativos da visao");
        var rapido = new EstadoDoOlhar(); var lento = new EstadoDoOlhar();
        rapido.Mover(30, 15); lento.Mover(30, 15);
        for (var i = 0; i < 120; i++) rapido.Atualizar(1f / 60);
        for (var i = 0; i < 60; i++) lento.Atualizar(1f / 30);
        Exigir(Math.Abs(rapido.Yaw - lento.Yaw) < 0.1f, "Suavidade independente da taxa de quadros");
        var etapas = new[] {
            new EtapaDaViagem { Id = "terra", Cenario = "terra", Duracao = 1 },
            new EtapaDaViagem { Id = "piratas", Cenario = "marte", Tipo = "combate" },
            new EtapaDaViagem { Id = "fim", Cenario = "chegada", Tipo = "chegada" }
        };
        var sequencia = new SequenciaDaViagem(etapas);
        sequencia.Atualizar(float.NaN, false);
        Exigir(sequencia.Tempo == 0, "Delta invalido ignorado");
        sequencia.Atualizar(1, false);
        Exigir(sequencia.Atual.Id == "piratas", "Tempo avanca observacao");
        sequencia.Atualizar(100, false);
        Exigir(sequencia.Atual.Id == "piratas", "Evento nao termina por tempo");
        sequencia.Atualizar(0.1f, true);
        Exigir(sequencia.Terminou, "Conclusao avanca evento");
        sequencia.IrPara("terra");
        Exigir(sequencia.Tempo == 0 && !sequencia.Terminou, "Reinicio limpo");
        var rejeitou = false;
        try { sequencia.IrPara("invalido"); } catch (ArgumentException) { rejeitou = true; }
        Exigir(rejeitou && sequencia.Atual.Id == "terra", "Debug invalido preserva etapa");
        foreach (var lado in new[] { -1, 1 })
        {
            var gelo = new ManobraDoGelo(1, 1, 2);
            gelo.Atualizar(1, 0);
            Exigir(gelo.Fase == FaseDoGelo.Alerta, "Alerta apos aproximacao");
            gelo.Atualizar(100, 0);
            Exigir(gelo.Fase == FaseDoGelo.Alerta, "Espera sem colisao inevitavel");
            gelo.Atualizar(0.5f, lado);
            gelo.Atualizar(0.2f, -lado);
            Exigir(gelo.Correcao < 0.3f, "Troca de lado reinicia correcao");
            gelo.Atualizar(1, lado);
            Exigir(gelo.Fase == FaseDoGelo.Evasao && gelo.Lado == lado, "Correcao dispara evasao");
            gelo.Atualizar(2, 0);
            Exigir(gelo.Fase == FaseDoGelo.Concluido, "Evasao automatica termina");
        }
        var salto = new ControladorDoSalto();
        foreach (var destino in new[] { "marte", "urano", "ana_branca", "chegada" })
        {
            salto.Reiniciar();
            Exigir(salto.IniciarAutomatico(destino), "Salto automatico aceita rota fixa");
            Exigir(!salto.IniciarAutomatico(destino), "Salto duplicado rejeitado");
            for (var i = 0; i < 100; i++) { salto.Atualizar(0.1f); if (salto.PrecisaCarregar) salto.ConfirmarCarregamento(true); }
            Exigir(salto.Fase == FaseDoSalto.Chegada, "Salto chega");
        }
        Console.WriteLine("Regras robustas: sequencia, gelo e saltos aprovados.");
    }
    private static void Exigir(bool condicao, string mensagem) { if (!condicao) throw new Exception(mensagem); }
}
