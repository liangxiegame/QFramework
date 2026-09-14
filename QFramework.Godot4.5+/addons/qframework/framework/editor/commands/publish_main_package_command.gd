class_name PublishMainPackageCommand

static func execute(parent:QFrameworkEditorPlugin,username:String,password:String,version:String,update_info:String):
		
	_save_version(parent,version)
	
	var main_package_path = "res://addons/qframework"
	var main_package_zip_path = "res://addons/qframework.zip"
	
	var zip_packer = ZIPPacker.new()
	
	var err = zip_packer.open(main_package_zip_path,ZIPPacker.APPEND_CREATE)
	if err != OK:
		print("打开失败")
		return

	compress_zip(main_package_path,zip_packer)

	zip_packer.close()
	
	
	parent.get_editor_interface().get_resource_filesystem().scan()

	
	var zip_file = FileAccess.open(main_package_zip_path,FileAccess.READ)
	
	var url = "https://api.liangxiegame.com/qf/gdscript/main_package/add"
	
	var buffer = zip_file.get_buffer(zip_file.get_length())
		#
	var boundary_key = "BodyBoundaryHere"
	
	var body = HTTPHelper.start_body()
	HTTPHelper.add_file(body,boundary_key, "File",buffer,"qframework.zip","application/zip")
	HTTPHelper.add_field(body,boundary_key,"Username",username)
	HTTPHelper.add_field(body,boundary_key,"Password",password)
	HTTPHelper.add_field(body,boundary_key,"Version",version)
	HTTPHelper.add_field(body,boundary_key,"UpdateInfo",update_info)
	HTTPHelper.end_body(body,boundary_key)
	
	
	var headers = [
		"Content-Type: multipart/form-data; boundary=%s" % boundary_key,
		"Content-Length: " + str(body.size()),
	]
	
	var request = HTTPRequest.new()
	parent.add_child(request)
	request.request_completed.connect(func(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray):
		pass
		if result == OK:
			print("发布成功")
		else:
			print("发布失败")
	)
	request.request_raw(url,PackedStringArray(headers),HTTPClient.METHOD_POST,body)
	
	await request.request_completed
	zip_file.close()
	DirAccess.remove_absolute(main_package_zip_path)
	request.queue_free()

static func compress_zip(path: String,zip_packer:ZIPPacker):
	var dir := DirAccess.open(path)
	if dir == null:
		return

	dir.list_dir_begin()
	var name := dir.get_next()

	while name != "":
		if name != "." and name != "..":
			var full_path := path.path_join(name)
			if dir.current_is_dir():
				#print("目录:", full_path)
				compress_zip(full_path,zip_packer)
			else:
				#print("文件:", full_path)
				var file = FileAccess.open(full_path,FileAccess.READ)
				var code = file.get_as_text()
				
				var file_name = full_path.replace("res://addons/qframework/","")
				
				zip_packer.start_file(file_name)
				zip_packer.write_file(code.to_utf8_buffer())
				zip_packer.close_file()
				
		name = dir.get_next()

	dir.list_dir_end()
	
static func _save_version(parent:QFrameworkEditorPlugin,version:String):
	var file = ConfigFile.new()
	file.load("res://addons/qframework/framework/plugin.cfg")
	print(file.get_value("plugin","version"))
	file.set_value("plugin","version",version)
	print(file.get_value("plugin","version"))
	file.save("res://addons/qframework/framework/plugin.cfg")
	parent.get_editor_interface().get_resource_filesystem().scan()
