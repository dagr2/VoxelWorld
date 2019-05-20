extends Control

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

# Called when the node enters the scene tree for the first time.
func _ready():
    Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
    var s=OS.get_screen_size()
    $Label.text="Res: "+str(s.x)+", "+str(s.y)
    #OS.set_window_size(s)
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):   
    if Input.is_action_just_pressed("F11"):
        OS.window_fullscreen = !OS.window_fullscreen

func _on_BtnStart_pressed():
    get_tree().change_scene("res://Spatial.tscn")


func _on_Button_pressed():
    get_tree().quit()


func _on_Button2_pressed():
    OS.window_fullscreen = !OS.window_fullscreen
