extends Node2D

class_name BindablePropertyUnRegisterBasic

# 定义
var some_number:BindableProperty = BindableProperty.new(10)
var some_text:BindableProperty = BindableProperty.new("abc")

var some_number_un_register:UnRegister

func _ready() -> void:
	# 注册后获得 unregister
	some_number_un_register = some_number.register(func(number:int):
		print("some number:%d" % number)
	)
	
	# 通过有名函数注册
	some_text.register(_on_some_text_changed)
	
func _on_some_text_changed(text:String):
	print("some text:%s" % text)

func _process(_delta: float) -> void:
	if Input.is_key_pressed(KEY_SPACE):
		print("空格键按下")
		some_number.value = randi_range(0,100)
		some_text.value = "%s" % ["def","qwe","aaa"].pick_random()
		
	if Input.is_key_pressed(KEY_1):
		some_number_un_register.un_register()
		some_text.un_register(_on_some_text_changed)
