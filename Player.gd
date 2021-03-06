extends KinematicBody

export(float,1,20) var Runspeed=10
export(float,.1,5) var Walkspeed=.5
export(float,.1,10) var Sprintspeed=1.0
export(float,10,100) var Jumppower=20
export(float,0.1,100) var Gravity = 0.5
export(float,1,100) var MouseSens=1.0
export(int,1,10) var VisibleChunks=3


var SPEED = 10.0
var active=true
var vel=Vector3(0,0,0)
var dir=Vector3(0,0,0)
var is_sprinting=false

var pitch=0
var yaw=0
var is_grav=true
var is_flying=true
var hit

func _ready():
    Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
    #OS.window_fullscreen =true
    get_parent_spatial().vischunks=VisibleChunks
    translation.y=2+ get_parent_spatial().HeightAt(translation.x,translation.z)

func get_clicked_block():
    var p=$Camera/RayCast.get_collision_point()
    var n=$Camera/RayCast.get_collision_normal()
    var p2=p-n/2
    var p3=Vector3(round(p2.x),round(p2.y),round(p2.z))
    var chunk = $Camera/RayCast.get_collider()    
    var res={}
    res.Chunk=chunk
    res.Block=p3
    return res
    
func _input(event):
    if active and event is InputEventMouseMotion:
        var dx=event.relative.x
        var dy=event.relative.y
        pitch -= dx*MouseSens/1000.0
        yaw -= dy*MouseSens/1000.0
        
        dir=Vector3(0,0,1).rotated(Vector3(1,0,0),0)
        dir=dir.rotated(Vector3(0,1,0),pitch)
        
        $Camera.rotation.x = yaw
        rotation.y=pitch
        
    if event is InputEventMouseButton and event.pressed and event.button_index == 1:
        if $Camera/RayCast.is_colliding():
            var res=get_clicked_block()
            var p3=res.Block
            var chunk = res.Chunk
            
            chunk.SetBlock(p3.x,p3.y,p3.z,0)
            
            #print(p2)
            
        
          
var can_collide=false       
    
func _process(delta):
    if $Camera/RayCast.is_colliding():
        var t=get_parent().get_node("Target")
        t.translation=$Camera/RayCast.get_collision_point()
        
    is_sprinting=false
    dir=Vector3(0,0,1).rotated(Vector3(1,0,0),0)
    dir=dir.rotated(Vector3(0,1,0),pitch)
    
    $Camera.rotation.x = yaw
    rotation.y=pitch
    $Lamp.rotation=$Camera.rotation
  
        
    if Input.is_action_pressed("sprint"):
        is_sprinting=true
          
    var speed=Walkspeed
    if is_sprinting:
        speed=Sprintspeed
    
    var walk=Vector3(0,0,0)
    var fly=Vector3(0,0,0)
    
    
    if Input.is_action_just_pressed("toggle_collision"):
      pass
        
    if Input.is_action_just_pressed("F11"):
        OS.window_fullscreen = !OS.window_fullscreen
        
    if Input.is_action_just_pressed("F1"):
        get_parent().AddChilds()
            
    if Input.is_action_just_pressed("gravity"):
        is_grav = !is_grav
    
    if Input.is_action_just_pressed("ui_cancel"):
        get_tree().change_scene("res://MainMenu.tscn")
        
    if Input.is_action_pressed("reset"):
        translation=Vector3(0,2+ get_parent_spatial().HeightAt(0,0),0)
        is_flying=true
                
    if Input.is_action_pressed("ui_up"):
        walk = speed*Vector3(dir.x,0,dir.z)*10
            
    if Input.is_action_pressed("ui_down"):
        walk = -speed*Vector3(dir.x,0,dir.z)*10
        
    if Input.is_action_pressed("ui_left"):
        pass
        
    if Input.is_action_pressed("ui_right"):
        pass
        
    if Input.is_action_pressed("ui_jump"):
        if !is_flying:
            if is_on_floor(): 
                vel=vel+Vector3(0,Jumppower,0)
        else:
            fly.y = 10;

    if Input.is_action_pressed("ui_sneak"):
        if is_flying:
            fly.y = -10
        
    if Input.is_action_just_pressed("light"):
        if $Lamp.light_energy>0:
             $Lamp.light_energy=0
        else:
             $Lamp.light_energy=6
            
    if Input.is_action_just_pressed("toggle_fly"):
        is_flying = !is_flying
        
    if Input.is_action_just_pressed("ui_inv"):
        active = not active
        if active:
            Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
            $Hud.hide()
        else:
            Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
            get_parent().get_node("IngameMenu").show()
            
    if not is_on_floor() and is_grav and not is_flying:
        vel=vel+Vector3(0,-Gravity,0)
    else:
        vel =vel#*0.8#-= dir*0.1
    move_and_slide(vel+walk+fly,Vector3(0,1,0))            
    
    
func _physics_process(delta):
    pass

func get_hit():
    return hit
    

    