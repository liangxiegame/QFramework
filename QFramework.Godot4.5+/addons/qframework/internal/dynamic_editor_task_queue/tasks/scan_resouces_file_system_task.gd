class_name ScanResourcesFileSystemTask extends EditorTask

func start():
	status = Status.Started
	await Engine.get_main_loop().create_timer(0.1).timeout
	editor_plugin.get_editor_interface().get_resource_filesystem().scan()
	status = Status.Finished
