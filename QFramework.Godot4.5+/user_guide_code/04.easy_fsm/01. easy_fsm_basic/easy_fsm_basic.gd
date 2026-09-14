extends Node2D

# 使用枚举 定义 状态 Id
enum States
{
	Idle,
	Jump
}

# 定义状态机对象
var fsm:EasyFSM = EasyFSM.new()

func _ready() -> void:
	# 定义 idle 状态
	fsm.state(
		States.Idle
	).on_enter(func():
		print("idle state enter")
	).on_exit(func():
		print("idle state exit")
	)
	
	# 定义 jump 状态
	fsm.state(
		States.Jump
	).on_enter(func():
		print("jump state enter")	
	).on_exit(func():
		print("jump state exit")	
	)
	
	# 设置初始状态
	fsm.start_state(States.Idle)
	
func _process(_delta: float) -> void:
	
	if Input.is_key_pressed(KEY_1):
		print("数字 1 按下")
		fsm.change_state(States.Idle)
		
	if Input.is_key_pressed(KEY_2):
		print("数字 2 按下")
		fsm.change_state(States.Jump)

func _exit_tree() -> void:
	fsm.clear()
	fsm = null
