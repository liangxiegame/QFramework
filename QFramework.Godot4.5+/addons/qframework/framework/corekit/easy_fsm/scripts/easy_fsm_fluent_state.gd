extends EasyFSMState

class_name EasyFSMFluentState

func on_enter(enter_func:Callable)->EasyFSMFluentState:
	enter_callback = enter_func
	return self
	
func on_process(process_func:Callable)->EasyFSMFluentState:
	process_callback = process_func
	return self
	
func on_physics_process(physics_process_func:Callable)->EasyFSMFluentState:
	physics_process_callback = physics_process_func
	return self
	
func on_exit(exit_func:Callable)->EasyFSMFluentState:
	exit_callback = exit_func
	return self;

func enter():
	if enter_callback:
		enter_callback.call()

func process(delta:float):
	if process_callback:
		process_callback.call(delta)

func physics_process(delta:float):
	if physics_process_callback:
		physics_process_callback.call(delta)

func exit():
	if exit_callback:
		exit_callback.call()
