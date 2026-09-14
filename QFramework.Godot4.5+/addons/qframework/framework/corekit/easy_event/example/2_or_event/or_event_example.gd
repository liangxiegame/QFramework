extends Control


@onready var message_box: Label = $MessageBox
@onready var btn_event_a: Button = $BtnEventA
@onready var btn_event_b: Button = $BtnEventB
@onready var btn_bindable_property_a: Button = $BtnBindablePropertyA

var event_a:EasyEvent = EasyEvent.new()
var event_b:EasyEvent = EasyEvent.new()
var bindable_property_a:BindableProperty = BindableProperty.new("abc")

func _ready() -> void:
	event_a.or_event(event_b) \
		.or_event(bindable_property_a) \
		.register(func(): \
			message_box.text = message_box.text.insert(0,"event_a.or(event_b).or(bindbale_property_a) 触发\n")
		).un_register_when_node_exiting_tree(self)
		
		
	btn_event_a.pressed.connect(func():
		event_a.trigger()	
	)
	
	btn_event_b.pressed.connect(func ():
		event_b.trigger()	
	)
	
	btn_bindable_property_a.pressed.connect(func ():
		bindable_property_a.value = "%f" % randf()
	)
