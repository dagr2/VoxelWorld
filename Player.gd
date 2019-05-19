extends KinematicBody

export(float,1,20) var Runspeed=10
export(float,.1,20) var Walkspeed=.5
export(float,10,100) var Jumppower=20
export(float,0.1,100) var Gravity = 0.5
export(float,1,100) var MouseSens=1.0

# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var SPEED = 10.0
var active=true
var vel=Vector3(0,0,0)
var dir=Vector3(0,0,0)

var pitch=0
var yaw=0

# Called when the node enters the scene tree for the first time.
func _ready():
    Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
    #pass # Replace with function body.

var is_grav=false
func _input(event):
    if active and event is InputEventMouseMotion:
        var dx=event.relative.x
        var dy=event.relative.y
        pitch -= dx*MouseSens/1000.0
        yaw -= dy*MouseSens/1000.0
        
        dir=Vector3(0,0,1).rotated(Vector3(1,0,0),0)
        dir=dir.rotated(Vector3(0,1,0),pitch)
        #direction.y -=10
        
        $Camera.rotation.x = yaw
        rotation.y=pitch
        

    if event is InputEventMouseButton and event.pressed and event.button_index == 1:

        print(hit)
#        var from = camera.project_ray_origin(event.position)
#        var to = from + camera.project_ray_normal(event.position) * 100
#        var space_state = get_world().direct_space_state
#        hit = space_state.intersect_ray($Camera.translation, dir*100)     
#        print(hit)
          
func _physics_process(delta):
    var rc=$Camera/RayCast
    hit = rc.get_collider()        
    
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
    dir=Vector3(0,0,1).rotated(Vector3(1,0,0),0)
    dir=dir.rotated(Vector3(0,1,0),pitch)
    #direction.y -=10
    
    $Camera.rotation.x = yaw
    rotation.y=pitch
    $Lamp.rotation=$Camera.rotation
    
    var walk=Vector3(0,0,0)
    if Input.is_action_just_pressed("gravity"):
        is_grav = !is_grav
    
    if Input.is_action_pressed("ui_cancel"):
        get_tree().quit()
        
    if Input.is_action_pressed("reset"):
        get_tree().reload_current_scene()
                
    if Input.is_action_pressed("ui_up"):
        walk = Walkspeed*Vector3(dir.x,0,dir.z)*10
            
    if Input.is_action_pressed("ui_down"):
        walk = -Walkspeed*Vector3(dir.x,0,dir.z)*10
        
    if Input.is_action_pressed("ui_left"):
        pass

        
    if Input.is_action_pressed("ui_right"):
        pass
        
    if Input.is_action_pressed("ui_jump") and is_on_floor():
        vel=vel+Vector3(0,Jumppower,0)


    if Input.is_action_pressed("ui_sneak"):
        pass
        
    if Input.is_action_just_pressed("light"):
        if $Lamp.light_energy>0:
             $Lamp.light_energy=0
        else:
             $Lamp.light_energy=6
        
    if Input.is_action_just_pressed("ui_inv"):
        active = not active
        if active:
            Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
            $Hud.hide()
        else:
            Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
            $Hud.show()
    if not is_on_floor() and is_grav:
        vel=vel+Vector3(0,-Gravity,0)
    else:
        vel =vel#*0.8#-= dir*0.1
    move_and_slide(vel+walk,Vector3(0,1,0))            
    
    var rc=$Camera/RayCast
    rc.cast_to=dir
    var hitpos=rc.get_collision_point()
    var hitnorm=rc.get_collision_normal() 
    
var hit

func get_hit():
    return hit
    

    