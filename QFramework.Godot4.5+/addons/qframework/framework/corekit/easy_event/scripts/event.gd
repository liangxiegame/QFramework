@abstract
class_name Event

var callables:Array[Callable] = []

func trigger(arg = null):
	for callable in callables:
		if callable.get_argument_count() == 0:
			callable.call()
		else:
			callable.call(arg)

func register(method:Callable) -> UnRegister:
	callables.push_back(method)
	return UnRegister.new(self,method);
	
func or_event(other_event:Event)->OrEvent:
	return OrEvent.new(self,other_event)
	
func un_register(method:Callable):
	var index = callables.find(method);
	if index != -1:
		callables.remove_at(index);
