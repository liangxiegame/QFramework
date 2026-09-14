@tool
class_name GenerateCommandCode

const TEMPLATE: String = """class_name %sCommand extends AbstractCommand

func execute():
	pass
"""

const DIR = "res://scripts/commands/"

static func build_file_path(command_name:String)->String:
	var command_file_name: String = CodeGenHelper.split_camel_to_lower(command_name)
	return "%s%s_command.gd" % [DIR,command_file_name]

static func generate( command_name:String,editorPlugin:EditorPlugin) -> void:
	CodeGenHelper.create_dir_if_not_exists(DIR)
	
	var code = TEMPLATE % command_name
	var path = build_file_path(command_name)
	var file := FileAccess.open(path,FileAccess.WRITE)
	if file == null:
		push_error("无法创建脚本:%s" % path)
		return
	file.store_string(code)
	file.close()
	
	print("已生成:",path)
	
	var script := load(path)
	if script:
		editorPlugin.get_editor_interface().edit_resource(script)

	editorPlugin.get_editor_interface().get_resource_filesystem().scan()
