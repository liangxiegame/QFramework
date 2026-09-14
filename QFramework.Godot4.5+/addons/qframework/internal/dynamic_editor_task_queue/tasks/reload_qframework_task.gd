class_name  ReloadQFrameworkTask extends EditorTask

func start():
	status = Status.Started
	var plugin_name = "qframework/framework"
	var editor_interface = editor_plugin.get_editor_interface()
	editor_interface.set_plugin_enabled(plugin_name,false)
	while editor_interface.is_plugin_enabled(plugin_name):
		await QF.wait_seconds(0.1)
	editor_interface.set_plugin_enabled(plugin_name,true)
	status = Status.Finished
