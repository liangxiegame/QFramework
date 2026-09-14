class_name VersionHelper


static func is_greator(version_a:String,version_b:String)->bool:
	
	var version_a_array:PackedStringArray = version_a.remove_chars("v").split(".")
	var version_b_array:PackedStringArray = version_b.remove_chars("v").split(".")
	
	for i in version_a_array.size():
		if version_a_array[i].to_int() > version_b_array[i].to_int():
			return true
		elif version_a_array[i].to_int() == version_b_array[i].to_int():
			continue
		else:
			return false
	
	return false
	
	
static func is_valid_version(version:String)->bool:
	
	if not version.begins_with("v"): return false
	var version_array = version.remove_chars("v").split(".")
	if not version_array.size() == 3: return false
	
	for v in version_array:
		if not v.is_valid_int():
			return false
	
	return true

static func increase_version(version:String)->String:
	var version_array:PackedStringArray = version.remove_chars("v").split(".")
	
	var latest_number = version_array[version_array.size() - 1].to_int()
	latest_number += 1
	version_array[version_array.size() - 1] = str(latest_number)
	
	return "v" + ".".join(version_array)
