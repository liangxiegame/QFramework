extends Node2D

# 使用枚举定义状态 id
enum States
{
	Idle,
	Jump
}

# 定义 FSM
var fsm:EasyFSM = EasyFSM.new()

func _ready() -> void:
	
	# 定义 Idle 状态
	fsm.state(
		States.Idle
	).on_enter(func():
		print("idle state enter")	
	).on_process(func(_delta:float):
		# 数字 2 按下，跳转到 Jump 状态
		if Input.is_key_pressed(KEY_2):
			fsm.change_state(States.Jump)	
	).on_exit(func():
		print("idle state exit")	
	)
	
	
	# 定义 Jump 状态
	fsm.state(
		States.Jump
	).on_enter(func():
		print("jump state exit")	
	).on_process(func(_delta:float):
		# 数字 1 按下，跳转到 Idle 状态
		if Input.is_key_pressed(KEY_1):
			fsm.change_state(States.Idle)	
	).on_exit(func():
		print("jump state enter")	
	)
	
	# 设置初始状态 
	fsm.start_state(States.Idle)

func _process(delta: float) -> void:
	# 使用 process 生命周期，需要 Node 的 process 生命周期驱动
	fsm.process(delta)
	
func _exit_tree() -> void:
	fsm.clear()
	fsm = null
