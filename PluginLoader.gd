extends Node

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

# Called when the node enters the scene tree for the first time.
func _ready():
    rescan()
    
func scan():
    var files=[]
    var dir=Directory.new()
    dir.open("./plugins")
    dir.list_dir_begin()
    
    while true:
        var file=dir.get_next()
        if file=="":
            break
        elif not file.begins_with("."):
            files.append(file)
    dir.list_dir_end()
    return files

func rescan():
    if false:
        var files=scan()
        print(files)
        for f in files:
            var s=load(f)
            if (s!=null):
                var ss=s.new()
            

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#    pass
