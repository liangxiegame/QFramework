@tool
class_name GenerateArchitectureCode

const TEMPLATE: String = """class_name %sArchitecture extends Architecture

func init():
	pass
"""

const DIR = "res://scripts/"

static func build_file_path(architecture_name)->String:
	var architecture_file_name: String = CodeGenHelper.split_camel_to_lower(architecture_name)
	return "%s%s.gd" % [DIR,architecture_file_name]

static func generate(architecture_name:String,editorPlugin:EditorPlugin) -> void:
	var code = TEMPLATE % architecture_name
	
	CodeGenHelper.create_dir_if_not_exists(DIR)

	var architecture_file_name: String = CodeGenHelper.split_camel_to_lower(architecture_name)
	var path = build_file_path(architecture_name)
	var file = FileAccess.open(path,FileAccess.WRITE)
	
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
	
	
	if not ProjectSettings.has_setting("autoload/" + architecture_name):
		ProjectSettings.set_setting("autoload/" + architecture_name, "*" + path)
		ProjectSettings.save()
		print("成功添加 Autoload！")
