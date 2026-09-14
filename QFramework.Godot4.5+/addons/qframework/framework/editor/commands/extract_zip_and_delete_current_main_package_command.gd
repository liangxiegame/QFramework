class_name ExtractZipAndDeleteCurrentMainPackageCommand


static func execute(parent:EditorPlugin):
	
	var dir_path = "res://addons/qframework"
	
	DirAccess.remove_absolute(dir_path)
	
	var reader = ZIPReader.new()
	
	extract_all_from_zip()
	
	DirAccess.remove_absolute("res://addons/qframework.zip")
	
	parent.get_editor_interface().get_resource_filesystem().scan()


# 解压 ZIP 压缩包中的所有文件，保持目录结构。
# 功能类似于大多数归档文件管理器中的“全部解压”功能。
static  func extract_all_from_zip():
	var reader = ZIPReader.new()
	reader.open("res://addons/qframework.zip")

	# 解压文件的目标目录（解压前必须存在）。
	# 不是所有的 ZIP 压缩包都会把所有文件都放在根文件夹中，
	# 解压后 `root_dir` 中会创建若干文件/文件夹。
	var root_dir = DirAccess.open("res://addons/qframework")

	var files = reader.get_files()
	for file_path in files:
		# 如果当前条目是目录。
		if file_path.ends_with("/"):
			root_dir.make_dir_recursive(file_path)
			continue

		# 写入文件内容，需要时自动创建文件夹。
		# 不是所有 ZIP 压缩包都遵循特定的顺序，这一步的作用是
		# 防止文件条目出现在文件夹条目之前。
		var full_file_path = root_dir.get_current_dir().path_join(file_path)
		root_dir.make_dir_recursive(full_file_path.get_base_dir())
				
		var file = FileAccess.open(full_file_path, FileAccess.WRITE)
		if file == null:
			pass
		else:
			var buffer = reader.read_file(file_path)
			file.store_buffer(buffer)
			file.close()
	
	reader.close()
