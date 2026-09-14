@tool
class_name GenerateQueryCode

const TEMPLATE: String = """class_name %sQuery extends AbstractQuery

func do():
	pass
"""

const DIR = "res://scripts/queries/"

static func build_file_path(query_name:String)->String:
	var command_file_name: String = CodeGenHelper.split_camel_to_lower(query_name)
	return "%s%s_query.gd" % [DIR,command_file_name]	

static func generate(query_name:String,editorPlugin:EditorPlugin) -> void:
	var code = TEMPLATE % query_name
	
	CodeGenHelper.create_dir_if_not_exists(DIR)

	var path = build_file_path(query_name)
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
