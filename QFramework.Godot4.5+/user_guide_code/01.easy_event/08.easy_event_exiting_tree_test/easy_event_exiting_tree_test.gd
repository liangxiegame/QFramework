extends Node2D

class_name EasyEventExitingTreeTest

var event_a:EasyEvent = EasyEvent.new()

func _ready() -> void:
	event_a.register(func():
		print("on event a")	
	).un_register_when_node_exiting_tree(self)
	
	self.tree_exiting.connect(func():
		print("exiting" + str(is_queued_for_deletion()) + str(is_instance_valid(self))) 	
		print(weakref(self).get_ref())
	)

	self.tree_exited.connect(func():
		print("exiting" + str(is_queued_for_deletion()) + str(is_instance_valid(self))) 	
		print(weakref(self).get_ref())
	)
	

func _process(delta: float) -> void:
	if Input.is_key_pressed(KEY_ESCAPE):
		var parent = self.get_parent()
		parent.remove_child(self)
		parent.add_child(self)
		
	if Input.is_key_pressed(KEY_D):
		self.free()
		
	if Input.is_key_pressed(KEY_1):
		get_tree().change_scene_to_file("res://playground/get_model_test/get_model_test.tscn")
		
	if Input.is_key_pressed(KEY_SPACE):
		event_a.trigger()

func _notification(what: int) -> void:
	if what == NOTIFICATION_PREDELETE:
		print("delete")
