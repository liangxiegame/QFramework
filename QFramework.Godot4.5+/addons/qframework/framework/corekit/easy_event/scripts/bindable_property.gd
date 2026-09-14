extends Event

class_name BindableProperty
	
func _init(init_value):
	_value = init_value;

var _value;
var value:
	get:
		return _value;
	set(v):
		if (v != _value):
			_value = v;
			trigger(v);
			
func set_value_without_event(new_value):
	self._value = new_value
	
func register_with_init_value(method:Callable)->UnRegister:
	method.call(_value)
	return register(method)
