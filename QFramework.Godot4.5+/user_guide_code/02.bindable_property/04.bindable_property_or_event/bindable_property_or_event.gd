extends Node2D

class_name BindablePropertyOrEvent

# 定义
var some_number_property:BindableProperty = BindableProperty.new(10)
var some_text_property:BindableProperty = BindableProperty.new("文本")
# 支持 EasyEvent
var event_a:EasyEvent = EasyEvent.new()

func _ready() -> void:
	# 合并注册
	(some_number_property
		.or_event(some_text_property)
		.or_event(event_a)
		.register_with_a_call(func(): # 也支持初始化调用一次
			print("or event triggered") \
		).un_register_when_node_exiting_tree(self)
	) # 用括号括一下 可以少写很多 \
	
func _process(_delta: float) -> void:
	# 当 1 键按下
	if Input.is_key_pressed(KEY_1):
		var random_number = randi_range(0,100)
		some_number_property.value = random_number
	# 当 2 键按下
	elif Input.is_key_pressed(KEY_2):
		var random_number = randi_range(0,100)
		some_text_property.value = "text:%d" % random_number
	# 当 3 键按下
	elif Input.is_key_pressed(KEY_3):
		event_a.trigger()
		
