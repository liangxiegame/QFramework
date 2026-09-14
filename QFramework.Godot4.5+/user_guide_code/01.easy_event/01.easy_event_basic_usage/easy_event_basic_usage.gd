extends Node2D

# 定义事件
var event_a:EasyEvent = EasyEvent.new()

func _ready() -> void:
	
	# 注册事件
	event_a.register(func():
		print("事件 A 触发")
	).un_register_when_node_exiting_tree(self) # 自动注销

func _process(_delta: float) -> void:
	
	# 当按下空格时
	if Input.is_key_pressed(Key.KEY_SPACE):
		# 触发事件
		event_a.trigger()
