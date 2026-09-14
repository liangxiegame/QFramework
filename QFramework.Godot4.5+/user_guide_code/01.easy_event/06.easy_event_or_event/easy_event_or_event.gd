extends Node2D

class_name EasyEventOrEvent

# 定义事件
var event_a:EasyEvent = EasyEvent.new()
var event_b:EasyEvent = EasyEvent.new()
var event_c:EasyEvent = EasyEvent.new()

func _ready() -> void:
	# 注册事件
	event_a.or_event(event_b).or_event(event_c).register(func():
		print("event a 或 event b:触发")
	).un_register_when_node_exiting_tree(self)


func _process(_delta: float) -> void:
	
	# 按键 1 按下
	if Input.is_key_pressed(KEY_1):
		print("按键 1 按下")
		# 触发事件 A
		event_a.trigger()
		
	# 按键 2 按下
	if Input.is_key_pressed(KEY_2):
		print("按键 2 按下")
		# 触发事件 B
		event_b.trigger()
