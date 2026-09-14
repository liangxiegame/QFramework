extends Node2D

class_name EasyEventUnRegisterByChildNode

# 定义
static var some_event:EasyEvent = EasyEvent.new()

# 获取子节点
@onready var event_receiver_2: Node = $EventReceiver2


func _process(_delta: float) -> void:
	
	# 按下空格键，触发事件
	if Input.is_key_pressed(KEY_SPACE):
		print("空格按下")
		some_event.trigger()
		
		
	# 按下数字 1 键，销毁子节点
	if Input.is_key_pressed(KEY_1):
		if event_receiver_2 != null:
			event_receiver_2.queue_free()
			event_receiver_2 = null
