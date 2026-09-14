extends Control

var count:BindableProperty = BindableProperty.new(0)

@onready var btn_add: Button = $BtnAdd
@onready var btn_sub: Button = $BtnSub
@onready var count_label: Label = $CountLabel
@onready var count_label_2: Label = $CountLabel2

func _ready() -> void:
	btn_add.pressed.connect(func():
		count.value += 1
	)
	
	btn_sub.pressed.connect(func():
		count.value -= 1
	)
	
	count.register_with_init_value(func(c:int):
		count_label.text = str(count.value)
		count_label_2.text = str(count.value)
	).un_register_when_node_exiting_tree(self)
