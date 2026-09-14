extends Node

var event_a:EasyEvent = EasyEvent.new()
var event_b:EasyEvent = EasyEvent.new()

var event_a_un_register:UnRegister

func _ready() -> void:
	
	event_a_un_register = event_a.register(func():
		pass
	)
	
	event_b.register(_on_event_b)
	
func _on_event_b():
	pass

	
func _notification(what: int) -> void:
	
	# 确定此脚本要销毁了
	if what == NOTIFICATION_PREDELETE:
		
		# 通过 un_register 注销
		event_a_un_register.un_register()
		
		# 通过函数注销
		event_b.un_register(_on_event_b)
	
