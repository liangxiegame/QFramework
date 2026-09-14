# 继承
extends NodeWithDestroyEvent

class_name EasyEventReceiver

func _ready() -> void:
	# 注册
	EasyEventRealUnRegister.some_event.register(func():
		print("on some event")
	).un_register_when_node_destroyed(self)
