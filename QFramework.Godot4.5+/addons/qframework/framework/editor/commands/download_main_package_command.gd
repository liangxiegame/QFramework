class_name DownloadMainPackageCommand


static func execute(parent:Node,downloadUrl:String):
	
	var request = HTTPRequest.new()
	parent.add_child(request)
	request.request_completed.connect(func(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray):
		if result == OK:
			var path = "res://addons/qframework.zip"
			var file = FileAccess.open(path,FileAccess.WRITE)
			file.store_buffer(body)
			file.close()
			(parent as EditorPlugin).get_editor_interface().get_resource_filesystem().scan()
	)
	request.request(downloadUrl)
	
	await request.request_completed
	
	request.queue_free()
	
	
	pass
