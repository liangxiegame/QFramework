extends Node2D

# 定义状态 Id （只定义一个）
enum States
{
	Idle
}

# 定义状态机
var fsm:EasyFSM = EasyFSM.new()

func _ready() -> void:
	
	# 定义 Idle 状态
	fsm.state(
		States.Idle
	).on_physics_process(func(delta:float):
		print("physics_process delta:%f" % delta)	
	)
	
	# 设置初始状态
	fsm.start_state(States.Idle)

func _physics_process(_delta: float) -> void:
	# 每个状态的 physics_process 由脚本的 phycis_process 驱动
	fsm.physics_process(_delta)
	
func _exit_tree() -> void:
	fsm.clear()
	fsm = null
