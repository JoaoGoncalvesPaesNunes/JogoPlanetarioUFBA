extends SceneTree

var falhas: Array[String] = []
var pasta := "res://.godot/validacao_visual"

func _initialize() -> void:
    executar.call_deferred()

func verificar(condicao: bool, mensagem: String) -> void:
    if not condicao:
        falhas.append(mensagem)
        push_error(mensagem)

func quadros(quantidade: int) -> void:
    for i in quantidade:
        await process_frame

func estabilizar_janela() -> void:
    # Com --fixed-fps a simulacao pode adiantar antes do resize nativo do Windows.
    var prazo := Time.get_ticks_msec() + 350
    while Time.get_ticks_msec() < prazo:
        await process_frame

func capturar(nome: String) -> void:
    if DisplayServer.get_name() == "headless":
        return
    await RenderingServer.frame_post_draw
    var imagem := root.get_texture().get_image()
    verificar(imagem != null and not imagem.is_empty(), "Renderizacao deve produzir pixels.")
    if imagem == null:
        return
    var amostras: Dictionary = {}
    for x in range(0, imagem.get_width(), 8):
        for y in range(0, imagem.get_height(), 8):
            amostras[imagem.get_pixel(x, y).to_html()] = true
    verificar(amostras.size() > 80, "Cena nao pode ser vazia ou uniforme.")
    imagem.save_png(pasta.path_join(nome + ".png"))

func executar() -> void:
    DirAccess.make_dir_recursive_absolute(pasta)
    var cena: Node = load("res://Apresentacao/Cenas/CenaNavegacaoOnRails.tscn").instantiate()
    root.add_child(cena)
    current_scene = cena
    await quadros(30)
    var rota: Path3D = cena.get_node("TrajetoriaOnRails")
    var seguidor: PathFollow3D = rota.get_node("SeguidorDaNave")
    var orientacao: Node3D = seguidor.get_node("OrientacaoSuave")
    var voo: Node3D = orientacao.get_node("ControladorDaNaveOnRails")
    var camera: Camera3D = voo.get_node("CameraPiloto")
    verificar(voo.has_node("InteriorDaNave"), "Cabine deve acompanhar a nave.")
    verificar(not cena.get_node("HUDDeDebug").visible, "Debug deve iniciar recolhido.")
    verificar(not cena.get_node("EstacaoVirtual").visible, "Estacao deve iniciar recolhida.")
    var direcao_anterior := Vector3.ZERO
    var maior_angulo := 0.0
    for indice in range(1, 2000):
        var distancia := rota.curve.get_baked_length() * indice / 2000.0
        var direcao := (rota.curve.sample_baked(distancia + 0.01, true) - rota.curve.sample_baked(distancia - 0.01, true)).normalized()
        if indice > 1:
            maior_angulo = max(maior_angulo, rad_to_deg(direcao.angle_to(direcao_anterior)))
        direcao_anterior = direcao
    verificar(maior_angulo < 0.6, "Tangente da rota deve ser continua: %.3f graus" % maior_angulo)
    print("[VALIDACAO] Maior variacao de tangente: %.4f graus" % maior_angulo)
    await capturar("01_cockpit")
    voo.call("AlterarOlhar", Vector2(0, -72))
    await quadros(60)
    await capturar("08_teto_panoramico")
    voo.call("RecentralizarCamera")
    await quadros(60)
    Input.action_press("camera_direita")
    await quadros(35)
    Input.action_release("camera_direita")
    await quadros(30)
    verificar(camera.rotation.y < -0.1, "Olhar para direita deve girar camera para direita.")
    verificar(voo.rotation.length() < 0.001, "Olhar nao deve inclinar a cabine.")
    await capturar("02_olhar_direita")
    voo.call("AlterarOlhar", Vector2(80, 12))
    await quadros(60)
    await capturar("09_portas_passageiros")
    voo.call("RecentralizarCamera")
    await quadros(60)
    verificar(camera.rotation.length() < 0.01, "Camera deve recentralizar suavemente.")
    root.size = Vector2i(960, 540)
    await estabilizar_janela()
    await capturar("03_compacto")
    root.size = Vector2i(1600, 720)
    await estabilizar_janela()
    await capturar("04_amplo")
    root.size = Vector2i(1280, 720)
    await estabilizar_janela()
    var tab := InputEventKey.new()
    tab.physical_keycode = KEY_TAB
    tab.pressed = true
    Input.parse_input_event(tab)
    await quadros(2)
    tab.pressed = false
    Input.parse_input_event(tab)
    verificar(cena.get_node("EstacaoVirtual").visible, "Tab deve abrir estacao.")
    await capturar("05_estacao")
    cena.get_node("EstacaoVirtual").hide()
    Input.action_press("piloto_avancar")
    var anterior := orientacao.quaternion
    var maior_giro := 0.0
    for indice in 1150:
        await process_frame
        maior_giro = max(maior_giro, rad_to_deg(anterior.angle_to(orientacao.quaternion)))
        anterior = orientacao.quaternion
    verificar(seguidor.progress_ratio > 0.5, "Aceleracao deve produzir deslocamento na rota.")
    verificar(maior_giro < 1.0, "Orientacao nao deve dar saltos entre quadros.")
    print("[VALIDACAO] Maior giro por quadro: %.4f graus" % maior_giro)
    await capturar("06_travessia")
    Input.action_release("piloto_avancar")
    Input.action_press("piloto_recuar")
    await quadros(100)
    var progresso_antes := seguidor.progress_ratio
    await quadros(30)
    verificar(seguidor.progress_ratio < progresso_antes, "Re deve diminuir progresso sem virar a nave.")
    Input.action_release("piloto_recuar")
    Input.action_press("piloto_avancar")
    await quadros(1200)
    Input.action_release("piloto_avancar")
    verificar(cena.call("EscolherDestino", "marte"), "Escolha deve iniciar salto para Marte.")
    await quadros(400)
    verificar(cena.get_node("GerenciadorCenarios").call("ObterIdentificadorAtual") == "marte", "Salto deve carregar Marte.")
    await capturar("07_conclusao")
    var confirmar := InputEventKey.new()
    confirmar.physical_keycode = KEY_ENTER
    confirmar.pressed = true
    Input.parse_input_event(confirmar)
    await quadros(2)
    confirmar.pressed = false
    Input.parse_input_event(confirmar)
    verificar(seguidor.progress_ratio < 0.01, "Enter deve reiniciar a travessia concluida.")
    print("[VALIDACAO] Falhas: ", falhas.size())
    quit(0 if falhas.is_empty() else 1)
