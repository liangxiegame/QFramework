@abstract
class_name AbstractViewBuilder

var child_builders:Array[AbstractViewBuilder] = []

var property_setters:Array[Callable] = []

func apply_properties(node):
	for property_setter in property_setters:
		property_setter.call(node)
	return node

func child(child_builder:AbstractViewBuilder):
	child_builders.append(child_builder)
	return self
	
func build_children(parent):
	for child_builder in child_builders:
		parent.add_child(child_builder.build())
	return parent
	
func on_build(_on_build:Callable):
	property_setters.append(_on_build)
	return self
	
func for_each(_data_list:Array,_on_each:Callable):
	
	for data in _data_list:
		child(_on_each.call(data))
	
	return self
	
func name(_name:String):
	property_setters.append(func(n:Node):n.name = _name)
	return self
	
func parent(_parent:Node):
	property_setters.append(func(n:Node):_parent.add_child(n))
	return self
	
func h_size_expand_fill():
	property_setters.append(func(v:Control):v.size_flags_horizontal = Control.SIZE_EXPAND_FILL)
	return self
	
func min_size(w:float,h:float):
	var property_setter = func(v:Control): v.custom_minimum_size = Vector2(w,h)
	property_setters.append(property_setter)
	return self
	
func hide():
	property_setters.append(func(n:Control):n.hide())
	return self
	
@abstract
func build()
