@tool
class_name GenerateModelCode

const TEMPLATE: String = """class_name %sModel extends AbstractModel

const NAME:String = "%sModel"

func get_model_name() -> String: return NAME

func init():
	pass
"""

const DIR = "res://scripts/models/"

static func build_file_path(model_name:String,indie_folder:bool)->String:
	var model_file_name: String = CodeGenHelper.split_camel_to_lower(model_name)
	
	if indie_folder:
		return "%s%s_model/%s_model.gd" % [DIR,model_file_name,model_file_name]
		
	return "%s%s_model.gd" % [DIR,model_file_name]	

static func generate(model_name:String,editorPlugin:EditorPlugin,indie_folder:bool) -> void:
	var 	code = TEMPLATE % [model_name,model_name]

	var model_file_name: String = CodeGenHelper.split_camel_to_lower(model_name)
	var folder_path = "%s" % DIR
	
	if indie_folder:
		folder_path = "%s%s_model" % [DIR,model_file_name]
	
	CodeGenHelper.create_dir_if_not_exists(folder_path)
	
	var path = build_file_path(model_name,indie_folder)
		
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
