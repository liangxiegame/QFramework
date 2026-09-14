class_name LatestMainPackageQuery

static func do(editor_plugin:QFrameworkEditorPlugin)->MainPackageData:
	var packages = await ListMainPackageQuery.do(editor_plugin)
	var latest_package:MainPackageData
	
	for package in packages:
		if latest_package == null:
			latest_package = package
		elif VersionHelper.is_greator(package.version,latest_package.version):
			latest_package = package	
			
	QFEditorGlobal.latest_main_package.value = latest_package
	return latest_package
