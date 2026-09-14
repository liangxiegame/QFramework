extends Node2D

class_name EasyEventRealUnRegister

# 定义一个事件
static var some_event:EasyEvent = EasyEvent.new()

# 获取
@onready var event_receiver: EasyEventReceiver = $EventReceiver

func _process(_delta: float) -> void:
	if Input.is_key_pressed(KEY_SPACE):
		print("空格键按下")
		some_event.trigger()
		
	# 按下 1 键，删除掉子节点
	if Input.is_key_pressed(KEY_1):
		if event_receiver != null:
			event_receiver.queue_free()
			event_receiver = null
