class_name ListMainPackageQuery

static func do(parent:Node)->Array[MainPackageData]:
	var packages:Array[MainPackageData] = []
	var request =  HTTPRequest.new()
	parent.add_child(request)
	request.request_completed.connect(func(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray):
		if result == OK:
			var json = JSON.parse_string(body.get_string_from_utf8())
			
			var packages_json = json["data"]["packages"]
			
			for package_json in packages_json:
				var package = MainPackageData.new()
				package.version = package_json["version"]
				package.downloadUrl = package_json["downloadUrl"]
				package.createAt = package_json["createAt"]
				package.updateInfo = package_json["updateInfo"]
				packages.append(package)
	)
	request.request("https://api.liangxiegame.com/qf/gdscript/main_package/list")
	await request.request_completed
	return packages
	
