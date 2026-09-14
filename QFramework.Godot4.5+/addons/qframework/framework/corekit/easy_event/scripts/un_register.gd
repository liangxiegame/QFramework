class_name UnRegister

var _event;
var _method:Callable;

func _init(e,m) -> void:
	_event = e;
	_method = m;

# 以后不再提供了
func un_regsiter_when_node_destroyed(node:Node):
	un_register_when_node_exiting_tree(node)

# 以后不再提供了 拼写错误(是 exiting 不是 existing)
func un_regsiter_when_node_existing_tree(node:Node):
	un_register_when_node_exiting_tree(node)
	
func un_register_when_node_exiting_tree(node:Node):
	node.tree_exiting.connect(func():
		un_register()	
	);	
	
func un_register_when_node_destroyed(node:NodeWithDestroyEvent):
	node.destroy.connect(func():
		un_register()	
	)

func un_register_when_created_child_node_destroyed(node:Node):
	var created_child_node:NodeWithDestroyEvent = NodeWithDestroyEvent.new()
	node.add_child(created_child_node)
	created_child_node.destroy.connect(func():
		un_register()	
	)

func un_register_when_ui_hidden(ui:CanvasItem):
	ui.hidden.connect(func():
		un_register()
	,CONNECT_ONE_SHOT)

func un_register_when_node_2d_hidden(node_2d:CanvasItem):
	node_2d.hidden.connect(func():
		un_register()
	,CONNECT_ONE_SHOT)
	
func un_register_when_node_3d_hidden(node_3d:Node3D):	
	node_3d.visibility_changed.connect(func():
		if not node_3d.visible:
			un_register())

func un_register():
	_event.un_register(_method)
