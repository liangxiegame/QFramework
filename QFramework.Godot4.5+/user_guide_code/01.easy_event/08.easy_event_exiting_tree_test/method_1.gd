extends Node

var event_a:EasyEvent = EasyEvent.new()

@onready var child_node:Node = $ChildNode # onready 是在 enter tree 之后

# 懒加载模式
var _child_node:Node = null
var child_node_lazy:Node:
	get:
		if _child_node == null:
			_child_node = $ChildNode
		return _child_node

func _enter_tree() -> void:
	event_a.register_with_a_call(func():
		# 只能用这种，
		$ChildNode.name = "123"
		# 或者用懒加载的方式
		child_node_lazy.name = "123"
		# 这种就不能用了，此时是 null
		child_node.name = "123"
		
	).un_register_when_node_exiting_tree(self)
