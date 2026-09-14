extends Node2D

class_name EasyEventUnregisterbasic

# 定义事件
var event_a:EasyEvent = EasyEvent.new()
var event_b:EasyEvent = EasyEvent.new()

# 注销器对象
var event_b_unregister:UnRegister = null

func _ready() -> void:
	
	# 用函数注册事件
	event_a.register(_on_event_a)
	
	# 注册后会获得一个 unregister 对象
	event_b_unregister = event_b.register(func():
		print("on event b")	
	)

func _on_event_a():
	print("on event a")

func _process(_delta: float) -> void:
	
	# 空格按下
	if Input.is_key_pressed(KEY_SPACE):
		print("空格按下")
		# 触发事件 A 和 B
		event_a.trigger()
		event_b.trigger()
		
	# ESC 按下
	if Input.is_key_pressed(KEY_ESCAPE):
		print("注销事件")
		# 用函数注销事件
		event_a.un_register(_on_event_a)
		
		# 用 unregister 注销事件
		event_b_unregister.un_register()
