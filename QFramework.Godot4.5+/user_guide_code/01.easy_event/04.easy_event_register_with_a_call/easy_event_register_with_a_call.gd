extends Node2D

class_name EasyEventRegisterWithACall

# 定义事件
var event_a:EasyEvent = EasyEvent.new()

func _ready() -> void:

	# 注册事件，同时调用一次注册的函数	
	event_a.register_with_a_call(func():
		on_event_a()	
	).un_register_when_node_exiting_tree(self)

func on_event_a():
	print("on event a:received")
	
func _process(_delta: float) -> void:
	# 当空格按下
	if Input.is_key_pressed(KEY_SPACE):
		
		print("空格键按下")
		# 触发事件 A
		event_a.trigger()
