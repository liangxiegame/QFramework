extends Node2D

var some_number:BindableProperty = BindableProperty.new(10)
var some_event:EasyEvent = EasyEvent.new()

@onready var node_3d: Node3D = $Node3D

func _ready() -> void:
	
	some_number.register(func(number:int):
		print("some number:%d" % number)
	).un_register_when_node_2d_hidden(self) # 自己需要是 Node2D
	
	some_event.register(func():
		print("some event")
	).un_register_when_node_3d_hidden(node_3d)


func _process(_delta: float) -> void:
	if Input.is_key_pressed(KEY_SPACE):
		print("空格键按下")
		some_number.value = randi_range(0,100)
		some_event.trigger()
	
	# 按下数字 1 则隐藏节点（会触发事件自动注销）
	if Input.is_key_pressed(KEY_1):
		self.hide()
		node_3d.hide()
