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
    private float _ultimoAlinhamentoPublicado = -1f;
    private float _ultimoEstabilidadePublicada = -1f;
    private float _ultimaVelocidadePublicada = -1f;

    public Vector2 ApontamentoDoPiloto { get; private set; }
    public Vector2 PontoDeDobra { get; private set; } = new(0.36f, 0.18f);
    public float AlinhamentoDaRota { get; private set; }
    public float VelocidadeFrontalAtual { get; private set; }

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

    public void DefinirPontoDeDobra(Vector2 pontoDeDobra)
    {
        PontoDeDobra = pontoDeDobra;
        ApontamentoDoPiloto = Vector2.Zero;
        _ultimoAlinhamentoPublicado = -1f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_entradaDoPiloto is null || _estadoDaNave is null || _barramentoDeEventos is null)
        {
            return;
        }

        var estadoEntrada = _entradaDoPiloto.LerEstado();
        var movimento = estadoEntrada.Movimento;
        var deltaFloat = (float)delta;

        AtualizarVelocidadeFrontal(estadoEntrada.ImpulsoFrontal, deltaFloat);

        ApontamentoDoPiloto += new Vector2(movimento.X, movimento.Y) * _configuracaoDaNave.SensibilidadeDeMira * deltaFloat;
        ApontamentoDoPiloto = LimitarComprimento(ApontamentoDoPiloto, 1f);

        var desvio = PontoDeDobra - ApontamentoDoPiloto;
        AlinhamentoDaRota = 1f - Mathf.Clamp(desvio.Length() / 1.1f, 0f, 1f);

        if (movimento.LengthSquared() > 0.001f)
        {
            Position = new Vector3(-ApontamentoDoPiloto.X * 0.18f, ApontamentoDoPiloto.Y * 0.12f, 0f);
        }

        var rotacaoAlvo = new Vector3(
            -ApontamentoDoPiloto.Y * 0.08f,
            -ApontamentoDoPiloto.X * 0.1f,
            -movimento.X * 0.08f);
        Rotation = Rotation.Lerp(rotacaoAlvo, _configuracaoDaNave.SuavizacaoRotacao * deltaFloat);

        if ((movimento - _ultimoMovimentoPublicado).LengthSquared() > 0.01f)
        {
            _ultimoMovimentoPublicado = movimento;
            _barramentoDeEventos.Publicar(new MovimentoDoPilotoAlterado(movimento.X, movimento.Y, DateTime.UtcNow));
        }

        AtualizarEstabilidadeDaDobra(deltaFloat, desvio);

        if (estadoEntrada.AcaoPrimariaIniciada)
        {
            _barramentoDeEventos.Publicar(new AcaoDoPilotoExecutada("EstabilizarDobra", DateTime.UtcNow));

            if (_estadoDaNave.ConsumirEnergia(_configuracaoDaNave.ConsumoEnergiaAcaoPrimaria))
            {
                _barramentoDeEventos.Publicar(new EnergiaAlterada(_estadoDaNave.Energia, "EstabilizarDobra", DateTime.UtcNow));
            }

            if (_estadoDaNave.AlterarEstabilidadeDaDobra(14f))
            {
                _barramentoDeEventos.Publicar(new EstabilidadeDaDobraAlterada(_estadoDaNave.EstabilidadeDaDobra, "EstabilizarDobra", DateTime.UtcNow));
            }
        }
    }

    private void AtualizarEstabilidadeDaDobra(float delta, Vector2 desvio)
    {
        if (_estadoDaNave is null || _barramentoDeEventos is null)
        {
            return;
        }

        var alinhado = AlinhamentoDaRota >= 0.76f;
        var variacao = alinhado
            ? _configuracaoDaNave.GanhoEstabilidadeAlinhado * delta
            : -_configuracaoDaNave.PerdaEstabilidadeDesalinhado * (1f - AlinhamentoDaRota) * delta;

        if (_estadoDaNave.AlterarEstabilidadeDaDobra(variacao) &&
            Math.Abs(_estadoDaNave.EstabilidadeDaDobra - _ultimoEstabilidadePublicada) >= 1f)
        {
            _ultimoEstabilidadePublicada = _estadoDaNave.EstabilidadeDaDobra;
            _barramentoDeEventos.Publicar(new EstabilidadeDaDobraAlterada(_estadoDaNave.EstabilidadeDaDobra, alinhado ? "RotaAlinhada" : "RotaInstavel", DateTime.UtcNow));
        }

        if (Math.Abs(AlinhamentoDaRota - _ultimoAlinhamentoPublicado) >= 0.04f)
        {
            _ultimoAlinhamentoPublicado = AlinhamentoDaRota;
            _barramentoDeEventos.Publicar(new AlinhamentoDaRotaAlterado(AlinhamentoDaRota, desvio.X, desvio.Y, DateTime.UtcNow));
        }
    }

    private void AtualizarVelocidadeFrontal(float impulsoFrontal, float delta)
    {
        if (_barramentoDeEventos is null)
        {
            return;
        }

        if (impulsoFrontal > 0f)
        {
            VelocidadeFrontalAtual += impulsoFrontal * _configuracaoDaNave.AceleracaoFrontal * delta;
        }
        else if (impulsoFrontal < 0f)
        {
            VelocidadeFrontalAtual += impulsoFrontal * _configuracaoDaNave.FreioFrontal * delta;
        }
        else
        {
            VelocidadeFrontalAtual = Mathf.MoveToward(VelocidadeFrontalAtual, 0f, _configuracaoDaNave.ArrastoFrontal * delta);
        }

        VelocidadeFrontalAtual = Mathf.Clamp(VelocidadeFrontalAtual, 0f, _configuracaoDaNave.VelocidadeFrontalMaxima);

        if (Math.Abs(VelocidadeFrontalAtual - _ultimaVelocidadePublicada) >= 0.08f)
        {
            _ultimaVelocidadePublicada = VelocidadeFrontalAtual;
            _barramentoDeEventos.Publicar(new VelocidadeDaNaveAlterada(VelocidadeFrontalAtual, DateTime.UtcNow));
        }
    }

    private static Vector2 LimitarComprimento(Vector2 valor, float comprimentoMaximo)
    {
        return valor.Length() > comprimentoMaximo
            ? valor.Normalized() * comprimentoMaximo
            : valor;
    }
}
