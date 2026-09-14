class_name EasyFSM

var current_state:EasyFSMState = null
var current_state_id:int:
	get:return current_state.id;

var states:Dictionary[int,EasyFSMState] = {}

var current_physics_seconds:float = 0;
var current_seconds:float = 0;
var current_frames:int = 0

func state(state_id:int)->EasyFSMFluentState:
	var state = EasyFSMFluentState.new(state_id,null,self)
	add_state(state)
	return state;
	
func add_state(state:EasyFSMState):
	states[state.id] = state
	
func start_state(state_id:int):
	current_state = states[state_id]
	current_physics_seconds = 0
	current_seconds = 0
	current_state.enter()
	
	
func change_state(state_id:int):
	if (state_id == current_state_id): return
	
	if (current_state):
		current_state.exit()
		
	current_state = states[state_id]
	current_physics_seconds = 0
	current_seconds = 0
	current_frames = 0
	current_state.enter()
	
func process(delta:float):
	if (current_state):
		current_state.process(delta)
		current_seconds = current_seconds + delta
		current_frames += 1
		
func physics_process(delta:float):
	if (current_state):
		current_state.physics_process(delta)
		current_physics_seconds = current_physics_seconds + delta
		
func clear():
	current_state = null
	for state in states.values():
		state.clear()
		
	states.clear()
