extends Node2D

# 定义状态 Id
enum States
{
	Idle,
	Jump,
}

# 定义状态机
var fsm:EasyFSM = EasyFSM.new()

func _ready() -> void:
	# 定义 Idle 状态
	fsm.state(
		States.Idle
	).on_process(func(_delta:float):
		print("idle state current frame count:%d" % fsm.current_frames)
		print("idle state current seconds:%f" % fsm.current_seconds)
		
		# 计时一秒则跳转到 Jump 状态
		if fsm.current_seconds > 1.0:
			fsm.change_state(States.Jump)
	)
	
	# 定义 Jump 状态
	fsm.state(
		States.Jump
	).on_process(func(_delta:float):
		print("jump state current frame count:%d" % fsm.current_frames)
		print("jump state current seconds:%f" % fsm.current_seconds)
		
		# 计时一秒则跳转到 Idle 状态
		if fsm.current_seconds > 1.0:
			fsm.change_state(States.Idle)
	)
	
	# 置始状态
	fsm.start_state(States.Idle)

func _process(delta: float) -> void:
	# 驱动 process
	fsm.process(delta)
	
func _exit_tree() -> void:
	fsm.clear()
	fsm = null
