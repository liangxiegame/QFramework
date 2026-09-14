extends Node2D


class SomeArchitecture extends Architecture:
	
	func init():
		register_model(SomeModel.new())
		pass
		
class SomeModel extends AbstractModel:
	
	func get_model_name():
		return "SomeModel"
		
	func init():
		pass


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	
	var architecture = SomeArchitecture.new()
	add_child(architecture)
	
	var model =  architecture.get_model("SomeModel") as SomeModel
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
