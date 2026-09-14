@tool
extends EditorPlugin

class_name DynamicEditorTaskQueue

static var default:DynamicEditorTaskQueue = null

const PLUGIN_NAME = "qframework/internal/dynamic_editor_task_queue"

const dynamic_config_file_path = "res://addons/qframework/internal/dynamic_editor_task_queue/dynamic_plugin.txt"
const plugin_file_path = "res://addons//qframework/internal/dynamic_editor_task_queue/plugin.cfg"

static func _active_plugin(executor:EditorPlugin):
	
	if FileAccess.file_exists(plugin_file_path):
		DirAccess.remove_absolute(plugin_file_path)
		
	executor.get_editor_interface().get_resource_filesystem().scan()
	
	var dynamic_config_file = FileAccess.open(dynamic_config_file_path,FileAccess.READ)
	var plugin_file = FileAccess.open(plugin_file_path,FileAccess.WRITE)
	plugin_file.store_buffer(dynamic_config_file.get_buffer(dynamic_config_file.get_length()))
	plugin_file.close()
	dynamic_config_file.close()
	
	executor.get_editor_interface().get_resource_filesystem().scan()
	executor.get_editor_interface().call_deferred("set_plugin_enabled",PLUGIN_NAME,true)

func _deactive_plugin():
	get_editor_interface().set_plugin_enabled(PLUGIN_NAME,false)
	
	if FileAccess.file_exists(plugin_file_path):
		DirAccess.remove_absolute(plugin_file_path)
		
	QFrameworkEditorPlugin.editor_task_queue.push_task(ScanResourcesFileSystemTask.new())
		
static var _editor_task_queue:EditorTaskQueue = null

static var editor_task_queue:EditorTaskQueue:
	get:
		if _editor_task_queue == null:
			_editor_task_queue = EditorTaskQueue.new()
		return _editor_task_queue
	
static func push_task(executor:EditorPlugin,task:EditorTask):
	editor_task_queue.push_task(task)
	_active_plugin(executor)
	
static func push_callable(callable:Callable):
	editor_task_queue.push_callable(callable)
	
func _process(delta: float) -> void:
	if editor_task_queue.process(delta,self):
		pass
	else:
		_deactive_plugin()
			
func _enter_tree() -> void:
	default = self
	
func _exit_tree() -> void:
	default = null
