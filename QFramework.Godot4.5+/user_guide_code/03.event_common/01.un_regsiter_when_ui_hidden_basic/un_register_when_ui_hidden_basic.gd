extends Control

# 定义
var some_event:EasyEvent = EasyEvent.new()
var some_number:BindableProperty = BindableProperty.new(10)

func _ready() -> void:
	some_event.register(func():	
		print("on some event")
	).un_register_when_ui_hidden(self) # self 只要继承 Control 就可以
	
	some_number.register(func(number:int):
		print("some number:%d" % number)	
	).un_register_when_ui_hidden(self)

func _process(_delta: float) -> void:
	
	# 按下空格键 触发事件
	if Input.is_key_pressed(KEY_SPACE):
		print("空格按下")
		some_event.trigger()	
		some_number.value = randi_range(0,100)
		
	# 按下数字 1 隐藏自己
	if Input.is_key_pressed(KEY_1):
		self.hide()
