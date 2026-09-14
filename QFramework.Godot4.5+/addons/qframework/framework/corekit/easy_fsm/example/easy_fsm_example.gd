extends Control


enum States{
	IDLE,
	RUN,
	JUMP
}

var fsm:EasyFSM = EasyFSM.new()

@onready var message_box: Label = $MessageBox
@onready var hint: Label = $Hint
@onready var state: Label = $State

func _ready() -> void:
	var idle_state = fsm.state(States.IDLE)
	
	idle_state.on_enter(func():
		message("idle state enter")
		state.text = "State:Idle"
	)
	
	idle_state.on_process(func(delta:float):
		if Input.is_key_pressed(KEY_SPACE):
			fsm.change_state(States.JUMP)
			
		if Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_D):
			fsm.change_state(States.RUN)
	)
	idle_state.on_exit(func():
		message("idle state exit")
	)
	
	var run_state = fsm.state(States.RUN)
	
	run_state.on_enter(func():
		message("run state enter")
		state.text = "State:Run"
	)
	
	run_state.on_process(func(delta:float):
		if Input.is_key_pressed(KEY_SPACE):
			fsm.change_state(States.JUMP)
		
		if Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_D):
			pass
		else:
			fsm.change_state(States.IDLE)
	)
	
	run_state.on_exit(func():
		message("run state exit")
	)
	
	var jump_state = fsm.state(States.JUMP)
	jump_state.on_enter(func():
		message("jump state enter")	
		state.text = "State:Jump"
	)
	
	jump_state.on_physics_process(func(delta:float):
		
		# 当前状态进入的时长如果大于 0.5 秒则切换状态
		if fsm.current_physics_seconds > 0.5:
			fsm.change_state(States.IDLE)
	)
	
	jump_state.on_exit(func():
		message("jump state exit")	
	)
	fsm.start_state(States.IDLE)
	
func message(text:String):
	message_box.text =  message_box.text.insert(0,text + "\n")
	
func _process(delta: float) -> void:
	fsm.process(delta)

func _physics_process(delta: float) -> void:
	fsm.physics_process(delta)
	
func _exit_tree() -> void:
	fsm.clear()
