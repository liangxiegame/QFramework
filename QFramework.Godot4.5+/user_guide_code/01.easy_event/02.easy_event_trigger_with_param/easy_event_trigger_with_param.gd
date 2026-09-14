extends Node2D

class_name EasyEventTriggerWithParam

# 定义事件
var event_a:EasyEvent = EasyEvent.new()

func _ready() -> void:
	
	# 注册事件
	# 注册时接收参数，可以带类型，可以不带类型
	# 带类型需要保证发送的时候类型一致
	# 不带类型则随意，但是不推荐这么用
	event_a.register(func(param:int): 
		print(param)
	).un_register_when_node_exiting_tree(self)
	
func _process(_delta: float) -> void:
	# 按下空格则触发事件
	if Input.is_key_pressed(KEY_SPACE):
		event_a.trigger(50)
