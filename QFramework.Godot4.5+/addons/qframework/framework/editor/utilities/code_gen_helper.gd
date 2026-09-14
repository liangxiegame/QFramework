@tool
class_name CodeGenHelper

static func is_upper(c:String) -> bool:
	return c >= 'A' and c <= 'Z'

static func split_camel_to_lower(t: String) -> String:
	var result : Array[String]= []
	var current := ""
	var previous_character_is_upper: bool = false
	
	for i in t.length():
		var c := t[i]
	
		if i > 0 and is_upper(c) and not previous_character_is_upper:
			result.append(current)
			current = c
		else:
			current += c
		
		previous_character_is_upper = is_upper(c)
			
	if current != "":
		result.append(current)
		
	var result_string: String = ""
	
	for word in result:
		
		if result_string == "":
			result_string += word.to_lower()
		else:
			result_string += "_" + word.to_lower()
			
	return result_string

static func create_dir_if_not_exists(folder_path:String):
	if not DirAccess.dir_exists_absolute(folder_path):
		DirAccess.make_dir_recursive_absolute(folder_path)
