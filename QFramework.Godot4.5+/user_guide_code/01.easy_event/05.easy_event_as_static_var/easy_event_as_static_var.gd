extends Node2D

class_name EasyEventAsStaticVar

# 定义事件
static var event_a:EasyEvent = EasyEvent.new()

func _ready() -> void:
	
	# 注册事件
	event_a.register(func():
		print("event a:received")
	).un_register_when_node_exiting_tree(self)
	
	# 如果是在别的脚本可以通过类名.事件的方式访问
	EasyEventAsStaticVar.event_a.register(func():
		pass	
	).un_register_when_node_exiting_tree(self)

func _process(_delta: float) -> void:
	
	# 当空格键按下
	if Input.is_key_pressed(KEY_SPACE):
		event_a.trigger()
