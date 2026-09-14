extends Control

# controller
class_name CounterAppController

# view
@onready var btn_add: Button = $BtnAdd
@onready var btn_sub: Button = $BtnSub
@onready var count_label: Label = $CountLabel

var _model:ICounterAppModel = null # +-

func _ready() -> void:
	# 通过架构获得 model
	_model = CounterApp.get_model(ICounterAppModel.NAME) # +-
	
	# 监听输入
	btn_add.pressed.connect(func(): 
		# 交互逻辑
		CounterApp.send_command(IncreaseCountCommand.new())
	)
	
	# 监听输入 
	btn_sub.pressed.connect(func():
		# 交互逻辑
		CounterApp.send_command(DecreaseCountCommand.new())
	)
	
	# 注册 count 变更事件
	(_model.get_count() # +-
		.register_with_init_value(func(_count:int):_update_view()) 
		.un_register_when_created_child_node_destroyed(self))

func _update_view():
	count_label.text = "%d" % _model.get_count().value # +-
	
func _exit_tree() -> void:
	_model = null
