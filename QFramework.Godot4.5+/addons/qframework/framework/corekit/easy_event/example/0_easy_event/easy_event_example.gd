extends Node2D

var event_a:EasyEvent = EasyEvent.new()
var event_with_arg:EasyEvent = EasyEvent.new()
var event_b:EasyEvent = EasyEvent.new()
var event_c:EasyEvent = EasyEvent.new()
var event_d:EasyEvent = EasyEvent.new()

@onready var btn_send_event_a: Button = $BtnSendEventA
@onready var btn_send_event_with_arg: Button = $BtnSendEventWithArg

@onready var message_box: Label = $MessageBox

func _ready() -> void:

	# 基础事件
	event_a.register(func():
		message_box.text = message_box.text.insert(0,"接收到事件 event_a\n")
	).un_register_when_node_exiting_tree(self) # 会随着这个 Node 销毁而自动注销
	
	# 可以传参数
	event_with_arg.register(func(arg:int):
		message_box.text = message_box.text.insert(0,"接收到事件 event_with_param:%d\n" % arg)
	).un_register_when_node_exiting_tree(self) # 会随着这个 Node 销毁而自动注销
	
	# 注册时候就带一次调用
	event_b.register_with_a_call(func():
		message_box.text = message_box.text.insert(0,"接收到事件 event_b（注册时就出发一次）\n")
	).un_register_when_node_exiting_tree(self)
	
	# 注册时就带一次调用,但是带参数，所以参数需要 bind 一下
	event_c.register_with_a_call((func(text:String):
		message_box.text = message_box.text.insert(0,"接收到事件 event_c:%s（注册时就出发一次，带参数）\n" % text)
	).bind("凉鞋 123")).un_register_when_node_exiting_tree(self)
	
	btn_send_event_a.pressed.connect(func():
		event_a.trigger() # 触发事件
	)
	
	btn_send_event_with_arg.pressed.connect(func():
		event_with_arg.trigger(10) # 触发事件带一个参数 10 
	)
	
	
	# 以下代码为如何注销代码
	# 注册后获得注销器
	var event_d_unregster = event_d.register(func():
		pass	
	)
	
	# 直接通过注销器即可注销
	event_d_unregster.un_register()
	
	# 如果是有名字的函数，则可以用常规的事件注销
	var event_d_receiver = func(): pass
	event_d.register(event_d_receiver)
	event_d.un_register(event_d_receiver)
	
	event_d.register(_on_event_d)
	event_d.un_register(_on_event_d)

func _on_event_d():
	pass
