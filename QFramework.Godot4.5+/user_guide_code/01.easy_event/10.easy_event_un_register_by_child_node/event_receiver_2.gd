extends Node

class_name EasyEventReceiver2

func _ready() -> void:
	
	EasyEventUnRegisterByChildNode.some_event.register(func():
		print("on some event triggered")	
	).un_register_when_created_child_node_destroyed(self)
