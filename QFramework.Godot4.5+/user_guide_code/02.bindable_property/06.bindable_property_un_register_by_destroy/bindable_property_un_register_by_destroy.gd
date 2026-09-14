# 继承
extends NodeWithDestroyEvent

class_name BindablePropertyUnRegisterByDestroy

var some_number:BindableProperty = BindableProperty.new(10)

func _ready() -> void:
	
	some_number.register(func(number:int):
		print("some number:%d" % number)
		
	# 通过创建一个子节点销毁，self 不需要继承 NodeWithDestroyEvent
	).un_register_when_created_child_node_destroyed(self) 
	
	some_number.register(func(number:int):
		print("some number:%d" % number)	
	# self 需要继承 NodeWithDestroyEvent
	).un_register_when_node_destroyed(self)
