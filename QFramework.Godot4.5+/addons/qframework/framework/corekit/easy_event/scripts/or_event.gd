extends Event

class_name OrEvent

var events:Array[Event] = []

func _init(event_a:Event,event_b:Event) -> void:
	events.append(event_a)
	events.append(event_b)
	
func or_event(event_other:Event) -> OrEvent:
	events.append(event_other)
	return self;
	
func register(callable:Callable) -> UnRegister:
	
	for event in events:
		event.register(self.trigger)
		
	return super.register(callable)
	
func un_register(callable:Callable):
		
	for event in events:
		event.un_register(self.trigger)	
	
	super.un_register(callable)
	
func register_with_a_call(callable:Callable) -> UnRegister:
	callable.call()
	return self.register(callable)
