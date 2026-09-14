extends Node2D

class_name BindablePropertyRegisterWithInitValue

# 定义事件
var coin:BindableProperty = BindableProperty.new(3)

func _ready() -> void:
	# 注册事件
	coin.register_with_init_value(func(c:int):
		print(c)	
	).un_register_when_node_exiting_tree(self)
