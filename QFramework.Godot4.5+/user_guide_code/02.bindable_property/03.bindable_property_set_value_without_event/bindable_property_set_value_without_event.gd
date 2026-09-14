extends Node2D

class_name BindablePropertySetValueWithoutEvent

# 定义
var some_number_property:BindableProperty = BindableProperty.new(0)

func _ready() -> void:
	# 注册
	some_number_property.register(func(number:int):
		print("some_number_property 变更:%d" % number)
	).un_register_when_node_exiting_tree(self)

func _process(_delta: float) -> void:
	# 当空格键按下
	if Input.is_key_pressed(KEY_SPACE):
		var random_number = randi_range(1,10)
		print("随机置了一个数字:%d" % random_number)
		# 设置值不触发变更事件
		some_number_property.set_value_without_event(random_number)
		
	# 当 1 键按下
	if Input.is_key_pressed(KEY_1):
		var random_number = randi_range(1,10)
		print("随机置了一个数字:%d" % random_number)
		# 设置值将触发变更事件
		some_number_property.value = random_number
