extends Event

class_name EasyEvent
	
func register_with_a_call(method:Callable)->UnRegister:
	method.call()
	return register(method)
