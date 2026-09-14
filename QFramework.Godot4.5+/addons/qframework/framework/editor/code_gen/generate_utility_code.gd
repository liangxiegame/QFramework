@tool
class_name GenerateUtilityCode

const TEMPLATE: String = """class_name %sUtility extends AbstractUtility

const NAME:String = "%sUtility"

func get_utility_name() -> String: return NAME

func init():
	pass
"""

const DIR = "res://scripts/utilities/"


static func build_file_path(system_name:String,indie_folder:bool)->String:
	var system_file_name: String = CodeGenHelper.split_camel_to_lower(system_name)
	if indie_folder:
		return "%s%s_utility/%s_utility.gd" % [DIR,system_file_name,system_file_name]
	
	return "%s%s_utility.gd" % [DIR,system_file_name]

static func generate(system_name:String,editorPlugin:EditorPlugin,indie_folder:bool) -> void:
	var code = TEMPLATE % [system_name,system_name]

	var system_file_name: String = CodeGenHelper.split_camel_to_lower(system_name)
	var folder_path := "%s" % DIR
	
	if indie_folder:
		folder_path = "%s%s_utility" % [DIR,system_file_name]
	
	CodeGenHelper.create_dir_if_not_exists(folder_path)
	
	var path = build_file_path(system_name,indie_folder)
		
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
