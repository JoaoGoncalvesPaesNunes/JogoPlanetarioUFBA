using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using ProjetoPlanetario.Robusto.Viagem;
using ProjetoPlanetario.Robusto.Dominio;

namespace ProjetoPlanetario.Testes.Integracao;

public partial class ValidarVisualCamera : Node
{
    private DiretorDaViagem _jogo = null!;
    private int _verificacoes;
    public override async void _Ready()
    {
        try
        {
            _jogo = GD.Load<PackedScene>("res://Robusto/Cenas/Experiencia.tscn").Instantiate<DiretorDaViagem>();
            _jogo.Ritmo = 0.05f;
            _jogo.PausarAoPerderFoco = false;
            AddChild(_jogo);
            await Quadros(5);
            Exigir(_jogo.Erro == "", "Cena inicia sem erro");
            var cabine = _jogo.Nave.GetNode<Node3D>("InteriorDaNave");
            Exigir(cabine.FindChildren("*Passageiro*", "", true, false).Count == 0, "Sem cadeiras de passageiros");
            Exigir(cabine.FindChildren("BaseAssento*", "", true, false).Count == 0, "Sem bases de cadeiras");
            Exigir(cabine.FindChildren("JanelaSala*", "", true, false).Count == 0, "Sem sala traseira");
            Exigir(cabine.GetNode<Node3D>("AnteparaTraseira").Position.Z < 1.6f, "Fundo fechado junto ao cockpit");
            Exigir(cabine.FindChildren("CoberturaPanoramica*", "", true, false).Count > 0, "Vidros superiores preservados");
            Exigir(cabine.FindChildren("JanelaLateral*", "", true, false).Count == 4, "Vidros laterais preservados");

            foreach (var id in new[] { "terra", "marte", "urano", "ana_branca" })
            {
                _jogo.IrParaEtapa(id);
                Centralizar();
                await Quadros(3);
                var planeta = _jogo.Cenarios.CenarioAtual!.GetNode<Node3D>("Mundo/CorposCelestes/Planeta");
                var direcao = _jogo.Nave.GlobalBasis.Inverse() * (planeta.GlobalPosition - _jogo.Camera.GlobalPosition);
                var yaw = Mathf.RadToDeg(Mathf.Atan2(direcao.X, -direcao.Z));
                var pitch = Mathf.RadToDeg(Mathf.Atan2(direcao.Y, new Vector2(direcao.X, direcao.Z).Length()));
                Exigir(id switch { "terra" => Mathf.Abs(yaw) < 8, "marte" => yaw > 20 && yaw < 35,
                    "urano" => yaw < -20 && yaw > -35, _ => pitch > 20 && pitch < 35 }, "Composicao relativa: " + id);
                Exigir(planeta.GetNode<Marker3D>("CentroDoPlaneta").GlobalPosition.IsEqualApprox(planeta.GlobalPosition), "Alvo acompanha o planeta");
                await Capturar(id + "_frente");
                var naveAntes = _jogo.Nave.GlobalTransform;
                MoverMouse(new Vector2(yaw / _jogo.Olhar.SensibilidadeMouse, -pitch / _jogo.Olhar.SensibilidadeMouse));
                Assentar();
                Exigir(_jogo.Nave.GlobalTransform.IsEqualApprox(naveAntes), "Olhar nao move nem gira nave");
                Exigir((-_jogo.Camera.GlobalBasis.Z).Dot((planeta.GlobalPosition - _jogo.Camera.GlobalPosition).Normalized()) > 0.995f, "Olhar encontra planeta: " + id);
                await Quadros(3);
                await Capturar(id + "_olhar");
                if (id == "ana_branca") await ValidarEstrela(planeta);
                _jogo.Exibicao.DefinirModo(true);
                await Quadros(3);
                var pixel = _jogo.Exibicao.ProjetarPonto(planeta.GlobalPosition);
                var centro = GetViewport().GetVisibleRect().Size / 2;
                var raio = Mathf.Min(centro.X, centro.Y);
                Exigir(pixel.DistanceTo(centro) / raio < 0.15f, "Fulldome segue a mesma camera");
                Exigir(_jogo.Exibicao.DirecaoNaTela(pixel).Dot((planeta.GlobalPosition - _jogo.Camera.GlobalPosition).Normalized()) > 0.999f, "Raio fisheye consistente");
                await Capturar(id + "_domo_olhar");
                Centralizar();
                await Quadros(3);
                pixel = _jogo.Exibicao.ProjetarPonto(planeta.GlobalPosition);
                var superficie = planeta.GetNode<MeshInstance3D>("Superficie");
                var anguloRaio = Mathf.Asin(((SphereMesh)superficie.Mesh).Radius / planeta.GlobalPosition.DistanceTo(_jogo.Camera.GlobalPosition));
                Exigir(pixel.DistanceTo(centro) / raio + anguloRaio / (Mathf.Pi / 2) < 0.7f, "Corpo inteiro longe da borda do domo");
                await Capturar(id + "_domo_frente");
                _jogo.Exibicao.DefinirModo(false);
            }

            _jogo.IrParaEtapa("marte"); Centralizar();
            foreach (var visao in new[] { new Vector2(-115, 0), new Vector2(115, 0), new Vector2(0, 70), new Vector2(0, -50) })
            {
                Centralizar();
                MoverMouse(new Vector2(visao.X / 0.08f, -visao.Y / 0.08f));
                Assentar();
                await Quadros(3);
                Exigir(Mathf.Abs(_jogo.Olhar.Rotation.Z) < 0.001f, "Sem roll manual");
                Exigir(_jogo.Camera.GlobalPosition.DistanceTo(_jogo.Nave.GlobalPosition) < 0.3f, "Camera permanece no ponto do piloto");
                await Capturar($"cockpit_{visao.X}_{visao.Y}");
            }
            Centralizar();
            Input.ActionPress("mira_direita");
            await Quadros(30);
            Input.ActionRelease("mira_direita");
            Exigir(_jogo.Olhar.Estado.Yaw > 4, "Stick/teclado controla visao em Olhar");
            var anguloAntes = _jogo.Olhar.Estado.Yaw;
            GetViewport().PushInput(new InputEventKey { PhysicalKeycode = Key.R, Pressed = true }, true);
            Exigir(_jogo.Olhar.Estado.Yaw == anguloAntes, "R nao teleporta a camera");
            Assentar();
            Exigir(Mathf.Abs(_jogo.Olhar.Estado.Yaw) < 0.01f, "R recentraliza");
            _jogo.Interface.MostrarMenu(true);
            Exigir(Input.MouseMode == Input.MouseModeEnum.Visible, "Menu libera cursor");
            MoverMouse(new Vector2(400, 400)); Assentar();
            Exigir(Mathf.Abs(_jogo.Olhar.Estado.Yaw) < 0.01f, "Menu bloqueia olhar");
            await Capturar("menu");
            _jogo.Interface.MostrarMenu(false);

            foreach (var id in new[] { "piratas", "gelo", "salto_final" })
            {
                MoverMouse(new Vector2(200, -100)); Assentar();
                _jogo.IrParaEtapa(id);
                var alvoAntes = _jogo.Olhar.Estado.YawAlvo;
                MoverMouse(new Vector2(1000, 1000));
                Input.ActionPress("mira_direita"); await Quadros(3); Input.ActionRelease("mira_direita");
                Exigir(_jogo.Olhar.Estado.YawAlvo == alvoAntes, "Evento nao disputa camera: " + id);
                Exigir(_jogo.Contexto == (id == "piratas" ? ContextoDeEntrada.Combate : id == "gelo" ? ContextoDeEntrada.Gelo : ContextoDeEntrada.Bloqueado), "Contexto explicito: " + id);
                Assentar();
                Exigir(Mathf.Abs(_jogo.Olhar.Estado.Yaw) < 0.01f, "Eventos recentralizam suavemente");
            }
            _jogo.IrParaEtapa("ana_branca"); Centralizar();
            Exigir(_jogo.Contexto == ContextoDeEntrada.Olhar, "Observacao restaura contexto de visao");
            if (DisplayServer.GetName() != "headless")
            {
                GetWindow().Mode = Window.ModeEnum.Fullscreen;
                await Quadros(5); await Capturar("fullscreen");
                Exigir(GetWindow().Mode == Window.ModeEnum.Fullscreen, "Fullscreen preservado");
                _jogo.Exibicao.DefinirModo(true); _jogo.Exibicao.Calibracao = true;
                await Quadros(5); await Capturar("fullscreen_domo_calibracao");
            }
            GD.Print($"[VISUAL CAMERA] {_verificacoes} verificacoes aprovadas. Zero falhas.");
            _jogo.QueueFree(); await Quadros(3); GetTree().Quit();
        }
        catch (Exception erro) { Input.MouseMode = Input.MouseModeEnum.Visible; GD.PushError(erro.ToString()); GetTree().Quit(1); }
    }
    private async Task ValidarEstrela(Node3D planeta)
    {
        Exigir(planeta.GetNode<Node3D>("Halo").Visible, "Halo ativo");
        Exigir(planeta.GetNode<OmniLight3D>("LuzEstelar").LightEnergy > 0, "Luz propria ativa");
        var superficie = planeta.GetNode<MeshInstance3D>("Superficie");
        var rotacao = superficie.Rotation;
        _jogo.Cenarios.CenarioAtual!.AtualizarAmbiente(1);
        Exigir(superficie.Rotation.DistanceTo(rotacao) > 0 && superficie.Rotation.DistanceTo(rotacao) < 0.01f, "Rotacao estelar sutil");
        if (DisplayServer.GetName() == "headless") return;
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        using var imagem = GetViewport().GetTexture().GetImage();
        var centro = _jogo.Camera.UnprojectPosition(planeta.GlobalPosition);
        var borda = _jogo.Camera.UnprojectPosition(planeta.GlobalPosition + _jogo.Camera.GlobalBasis.X * 60);
        var raio = (borda - centro).Length() * imagem.GetWidth() / GetViewport().GetVisibleRect().Size.X;
        centro *= new Vector2(imagem.GetWidth(), imagem.GetHeight()) / GetViewport().GetVisibleRect().Size;
        var minimo = 1f; var maximo = 0f; var saturados = 0; var total = 0;
        for (var y = -0.6f; y <= 0.6f; y += 0.08f)
            for (var x = -0.6f; x <= 0.6f; x += 0.08f)
            {
                var cor = imagem.GetPixel((int)(centro.X + x * raio), (int)(centro.Y + y * raio));
                var brilho = (cor.R + cor.G + cor.B) / 3;
                minimo = Mathf.Min(minimo, brilho); maximo = Mathf.Max(maximo, brilho);
                if (cor.R > 0.98f && cor.G > 0.98f && cor.B > 0.98f) saturados++;
                total++;
            }
        Exigir(maximo - minimo > 0.04f, "Superficie possui variacao visivel");
        Exigir(saturados < total / 20, "Superficie nao estourada");
        GD.Print($"[ESTRELA] Variacao {maximo - minimo:F3}; pixels saturados {saturados}/{total}.");
    }
    private void MoverMouse(Vector2 delta) => GetViewport().PushInput(new InputEventMouseMotion { ScreenRelative = delta }, true);
    private void Assentar() { for (var i = 0; i < 600; i++) _jogo.Olhar.Atualizar(1f / 60, Vector2.Zero); }
    private void Centralizar() { _jogo.Olhar.Recentralizar(); Assentar(); }
    private async Task Quadros(int n) { for (var i = 0; i < n; i++) await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); }
    private async Task Capturar(string nome)
    {
        if (DisplayServer.GetName() == "headless") return;
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        using var imagem = GetViewport().GetTexture().GetImage();
        Exigir(!imagem.IsEmpty(), "Captura nao vazia " + nome);
        DirAccess.MakeDirRecursiveAbsolute("res://.godot/validacao_visual_camera");
        imagem.SavePng("res://.godot/validacao_visual_camera/" + nome + ".png");
    }
    private void Exigir(bool condicao, string mensagem) { _verificacoes++; if (!condicao) throw new InvalidOperationException(mensagem); }
}
