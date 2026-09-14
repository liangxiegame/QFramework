extends Node2D

class_name BindablePropertyBaicUsage

# 定义金币
var coin:BindableProperty = BindableProperty.new(10)

# 金币 Label
@onready var coin_label: Label = $CoinLabel

func _ready() -> void:
	# 注册 coin 属性 当变更时会调用注册函数并把变更后的值传过来
	coin.register(func(c:int):
		print(c)
		coin_label.text = "金币:%d" % c
	).un_register_when_node_exiting_tree(self)

func _process(_delta: float) -> void:
	
	# 当按下空格时
	if Input.is_key_pressed(KEY_SPACE):
		# 金币值 + 1
		coin.value += 1
