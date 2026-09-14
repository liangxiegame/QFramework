extends Node2D

class_name EasyFSMClassStateExample

# 定义状态 Id
enum States
{
	Idle,
	Jump
}

# 定义状态机
var fsm:EasyFSM = EasyFSM.new()

# 定义 Idle 状态类
class IdleState extends EasyFSMState:
	
	func enter():
		print("idle state enter")
		# 可以在状态类内部访问目标对象
		# 因为会在构造的时候传进来
		# 但是 target 类型是 Object，需要转成具体的类型
		print(target as EasyFSMClassStateExample)
		
	func exit():
		print("idle state exit")
		
	func process(_delta:float):
		# 当按下数字 2 按下则跳转到 Jump 状态
		if Input.is_key_pressed(KEY_2):
			# 可以在状态类内部访问 fsm
			# 因为会在构造的时候传进来
			fsm.change_state(States.Jump)
	
	# 类状态必须覆写 enter exit process physics_process 抽象函数
	func physics_process(_delta:float): pass
	
func _ready() -> void:
	# 类状态通过 add_state 的方式添加
	# 类状态的构造需要传入 状态 Id | 目标对象 | 状态机对象
	fsm.add_state(IdleState.new(States.Idle,self,fsm))
	
	# 定义 Jump 状态
	# 类状态和链式状态可以混合使用
	fsm.state(
		States.Jump
	).on_enter(func():
		print("jump state enter")	
	).on_exit(func():
		print("jump state exit")	
	).on_process(func(_delta:float):
		# 当 数字 1 按下则跳转到 Idle 状态
		if Input.is_key_pressed(KEY_1):
			fsm.change_state(States.Idle)
	)
	
	# 设置初始状态
	fsm.start_state(States.Idle)

func _process(delta: float) -> void:
	# 驱动状态机的 process
	fsm.process(delta)
	
func _exit_tree() -> void:
	fsm.clear()
	fsm = null
	
