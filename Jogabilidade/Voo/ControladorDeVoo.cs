using System;
using Godot;
using ProjetoPlanetario.Dados.Nave;
using ProjetoPlanetario.Entrada.Portas;
using ProjetoPlanetario.Nucleo.Estado;
using ProjetoPlanetario.Nucleo.Eventos;

namespace ProjetoPlanetario.Jogabilidade.Voo;

/// <summary>
/// Controla a nave a partir de acoes logicas, sem conhecer teclas ou dispositivos.
/// </summary>
public partial class ControladorDeVoo : Node3D
{
    private IEntradaDoPiloto? _entradaDoPiloto;
    private EstadoDaNave? _estadoDaNave;
    private BarramentoDeEventos? _barramentoDeEventos;
    private ConfiguracaoDaNave _configuracaoDaNave = new();
    private Vector2 _ultimoMovimentoPublicado;

    public void Configurar(
        IEntradaDoPiloto entradaDoPiloto,
        EstadoDaNave estadoDaNave,
        BarramentoDeEventos barramentoDeEventos,
        ConfiguracaoDaNave configuracaoDaNave)
    {
        _entradaDoPiloto = entradaDoPiloto;
        _estadoDaNave = estadoDaNave;
        _barramentoDeEventos = barramentoDeEventos;
        _configuracaoDaNave = configuracaoDaNave;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_entradaDoPiloto is null || _estadoDaNave is null || _barramentoDeEventos is null)
        {
            return;
        }

        var estadoEntrada = _entradaDoPiloto.LerEstado();
        var movimento = estadoEntrada.Movimento;

        if (movimento.LengthSquared() > 0.001f)
        {
            Translate(new Vector3(movimento.X, -movimento.Y, 0f) * _configuracaoDaNave.Velocidade * (float)delta);
            Rotation = new Vector3(-movimento.Y * 0.15f, movimento.X * 0.15f, -movimento.X * 0.25f);
        }
        else
        {
            Rotation = Rotation.Lerp(Vector3.Zero, _configuracaoDaNave.SuavizacaoRotacao * (float)delta);
        }

        if ((movimento - _ultimoMovimentoPublicado).LengthSquared() > 0.01f)
        {
            _ultimoMovimentoPublicado = movimento;
            _barramentoDeEventos.Publicar(new MovimentoDoPilotoAlterado(movimento.X, movimento.Y, DateTime.UtcNow));
        }

        if (estadoEntrada.AcaoPrimariaIniciada)
        {
            _barramentoDeEventos.Publicar(new AcaoDoPilotoExecutada("AcaoPrimaria", DateTime.UtcNow));

            if (_estadoDaNave.ConsumirEnergia(_configuracaoDaNave.ConsumoEnergiaAcaoPrimaria))
            {
                _barramentoDeEventos.Publicar(new EnergiaAlterada(_estadoDaNave.Energia, "AcaoPrimaria", DateTime.UtcNow));
            }
        }
    }
}
