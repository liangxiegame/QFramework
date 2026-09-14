extends Node2D

# 定义状态 id
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
	).on_physics_process(func(_delta:float):
		print("idle state physics seconds:%f" % fsm.current_physics_seconds)	
	)
	
	# 设置初始态
	fsm.start_state(States.Idle)
	
func _physics_process(delta: float) -> void:
	# 驱动 physics_process 生命周期
	fsm.physics_process(delta)
	
func _exit_tree() -> void:
	fsm.clear()
	fsm = null
