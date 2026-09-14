@tool
extends AbstractViewBuilder

class_name CreateArchitectureArea

var architecture_name_input:TextEdit = null
var btn_create:Button = null

var preview_label:Label = null
var preview_path_label:Label = null
var already_exist_label:Label = null
var root_view:VBoxContainer = null
var editor_plugin:QFrameworkEditorPlugin = null

func _init(_editor_plugin:QFrameworkEditorPlugin) -> void:
	editor_plugin = _editor_plugin
	
func _on_create_button_pressed():
	GenerateArchitectureCode.generate(architecture_name_input.text,editor_plugin) 
	architecture_name_input.text = ""
	editor_plugin.show_main()
	editor_plugin.hide_bottom_panel()
	
func _clear_data():
	architecture_name_input.text = ""
	_update_preview()
	
	
func _update_preview():
	if architecture_name_input.text == "":  
		preview_label.text = ""  
		preview_path_label.text = "res://scripts/"
		btn_create.hide()
	else: 
		preview_label.text = architecture_name_input.text 
		preview_path_label.text = GenerateArchitectureCode.build_file_path(preview_label.text)
		if FileAccess.file_exists(preview_path_label.text):
			btn_create.hide()
			already_exist_label.show()
		else:
			btn_create.show()
			already_exist_label.hide()

func build():
	var builder = (FluentUI.vbox() 
		.child(FluentUI.label().text("创建 Architecture").h_aligh_center().font_size(36))
		.child(FluentUI.hbox()
			.child(FluentUI.label().text("名字:"))
			.child(FluentUI.text_edit()
				.text_changed(func(): _update_preview()) 
				.min_size(300,20) 
				.on_build(func(t): architecture_name_input = t )
			)
			.child(FluentUI.label()
				.text("已存在")
				.hide()
				.on_build(func(l):already_exist_label = l)
			)
			.child(FluentUI.button()
				.text("创建")
				.pressed(_on_create_button_pressed)
				.hide()
				.on_build(func(b): btn_create = b)
			)
		)
		.child(FluentUI.hbox() 
			.child(FluentUI.label().text("预览").font_size(32))
		) 
		.child(FluentUI.hbox() 
			.child(FluentUI.label().text("Autoload 名:"))
			.child(FluentUI.label().text("").on_build(func(l):preview_label = l))
		) 
		.child(FluentUI.hbox()
			.child(FluentUI.label().text("生成路径:"))
			.child(FluentUI.label().on_build(func(l):preview_path_label = l))
		) 
		.child(FluentUI.button()
				.text("取消")
				.pressed(func(): 
					editor_plugin.show_main() 
					_clear_data() \
				)
		)
	)
	
	root_view = builder.build()
	root_view.hide()
	return apply_properties(root_view)
