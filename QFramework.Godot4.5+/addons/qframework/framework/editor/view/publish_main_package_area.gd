extends AbstractViewBuilder

class_name PublishMainPackageArea

var editor_plugin:QFrameworkEditorPlugin = null

var username_input:LineEdit
var password_input:LineEdit
var version_input:LineEdit
var update_info_input:TextEdit
var btn_upload:Button

func _init(_editor_plugin:QFrameworkEditorPlugin):
	self.editor_plugin = _editor_plugin

func _clear_data():
	username_input.text = ""
	password_input.text = ""
	update_info_input.text = ""
	
func _upload_button_pressed():
	await PublishMainPackageCommand.execute(self.editor_plugin,username_input.text,password_input.text,version_input.text,update_info_input.text)
	_clear_data()
	editor_plugin.show_main()
	editor_plugin._reload_self()

func _update_view():
	if (username_input.text.is_empty() or 
		password_input.text.is_empty() or 
		version_input.text.is_empty() or 
		update_info_input.text.is_empty() or
		not VersionHelper.is_valid_version(version_input.text) or
		not VersionHelper.is_greator(version_input.text,editor_plugin.get_plugin_version())):
		btn_upload.hide()
	else:
		btn_upload.show()

func build():
		
	var root_view = (FluentUI.vbox()
		.child(
			FluentUI.label()
				.text("发布 主体框架")
				.font_size(36)
				.h_aligh_center()
		)
		.child(
			FluentUI.hbox()
				.child(FluentUI.label().text("当前版本:").min_size(150,20))
				.child(FluentUI.label()
					.text(editor_plugin.get_plugin_version())
					.min_size(300,20))
		)
		.child(
			FluentUI.hbox()
				.child(FluentUI.label().text("发布版本:").min_size(150,20))
				.child(FluentUI.line_edit()
					.text_changed(_update_view)
					.default_text(VersionHelper.increase_version(editor_plugin.get_plugin_version()))
					.min_size(300,20)
					.on_build(func(t):version_input = t))
		)
		.child(
			FluentUI.hbox()
				.child(FluentUI.label().text("更新信息:").min_size(150,20))
				.child(FluentUI.text_edit()
					.text_changed(_update_view)
					.min_size(300,20)
					.on_build(func(t):update_info_input = t)
				)
		)
		.child(
			FluentUI.hbox()
				.child(FluentUI.label().text("用户名:").min_size(150,20))
				.child(FluentUI.line_edit()
					.text_changed(_update_view)
					.min_size(300,20)
					.on_build(func(t):username_input = t)
				)
		)
		.child(
			FluentUI.hbox()
				.child(FluentUI.label().text("密码:").min_size(150,20))
				.child(FluentUI.line_edit()
					.text_changed(_update_view)
					.password_mode()
					.min_size(300,20)
					.on_build(func(t):password_input = t)
				)
		)
		.child(
			FluentUI.button()
				.text("上传")
				.pressed(_upload_button_pressed)
				.on_build(func(b):btn_upload = b)
		)
		.child(FluentUI.button()
			.text("取消") 
			.pressed(func():  
				editor_plugin.show_main() 
				_clear_data() \
			)
		) 
		.build()
	)
	
	root_view.hide()
	_update_view()
	return apply_properties(root_view)
