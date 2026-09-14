@tool
extends EditorPlugin

class_name QFrameworkEditorPlugin

static var default:QFrameworkEditorPlugin

func _enable_plugin() -> void:
	# Add autoloads here.
	pass

func _disable_plugin() -> void:
	# Remove autoloads here.
	pass

var root_area: PanelContainer
var dock_shortcut: Shortcut

class FeatureItem:
	var feature_name:String
	var detail_area_builder:AbstractViewBuilder
	
	var side_bar_button:Button
	var detail_area:Control
	
	func _init(_feature_name,_detail_area_builder:AbstractViewBuilder) -> void:
		feature_name = _feature_name
		detail_area_builder = _detail_area_builder
	
var feature_items:Array[FeatureItem] = []

var main_area:HBoxContainer

static var _editor_task_queue:EditorTaskQueue = null
static var editor_task_queue:EditorTaskQueue:
	get:
		if _editor_task_queue == null: _editor_task_queue = EditorTaskQueue.new()
		return _editor_task_queue
		
func _process(delta: float) -> void:
	editor_task_queue.process(delta,self)

func _reload_self():
	DynamicEditorTaskQueue.push_task(self,ReloadQFrameworkTask.new())
	
func _download_and_update():
	var latest_package = await LatestMainPackageQuery.do(self)
	
	await DownloadMainPackageCommand.execute(self,latest_package.downloadUrl)
	
	await ExtractZipAndDeleteCurrentMainPackageCommand.execute(self)
	
	show_main()
	_reload_self()

var _server_version_label:Label = null
var _btn_update:Button = null
var _server_version_update_info:Label = null
	
func _enter_tree() -> void:
	QFEditorGlobal.init()
	
	feature_items.append(FeatureItem.new("创建 Command",CreateCommandArea.new(self)))
	feature_items.append(FeatureItem.new("创建 Query",CreateQueryArea.new(self)))
	feature_items.append(FeatureItem.new("创建 System",CreateSystemArea.new(self)))
	feature_items.append(FeatureItem.new("创建 Model",CreateModelArea.new(self)))
	feature_items.append(FeatureItem.new("创建 Utility",CreateUtilityArea.new(self)))
	feature_items.append(FeatureItem.new("创建 Architecture",CreateArchitectureArea.new(self)))
	feature_items.append(FeatureItem.new("发布 主体框架",PublishMainPackageArea.new(self)))
	
	# --- 创建一个简单的面板并放到 dock ---
	root_area = (FluentUI.panel()
		.name("QFramework")
		.child(FluentUI.vbox()
			.child(FluentUI.hbox()
				.child(FluentUI.label()
					.text("当前版本:" + get_plugin_version())
				)
				.child(FluentUI.label()
					.text("服务器版本:" + get_plugin_version())
					.on_build(func(l):_server_version_label = l)
				)
				.child(FluentUI.button()
					.text("更新")
					.pressed(_download_and_update)
					.on_build(func(b): _btn_update = b) 
				)
				.child(FluentUI.label()
					.text("版本信息:")
					.on_build(func(l):_server_version_update_info = l)
				)
				.child(FluentUI.button()
					.text("重载")
					.pressed(_reload_self)
				)
		
			)
			.child(FluentUI.hbox().on_build(func(a):main_area = a))
		)
	.build())
	
	(FluentUI.vbox()
		.child(
			FluentUI.hbox()
		)
		.for_each(feature_items, func(feature_item:FeatureItem):  \
				return FluentUI.button() \
					.text(feature_item.feature_name) \
					.pressed(func(): _show_detail_area_by_name(feature_item.feature_name)) \
					.on_build(func(button):feature_item.side_bar_button = button) \
		)
		.parent(main_area)
		.build()
	)
	
	
	(FluentUI.panel()
		.child(
			FluentUI.vbox()
				.for_each(feature_items,func(feature_item:FeatureItem): \
					return feature_item.detail_area_builder.on_build(func(area):feature_item.detail_area = area)
				)
		)
		.h_size_expand_fill()
		.parent(main_area)
		.build()
	)
	
	# --- 创建一个快捷键：Command + E ---
	var ev := InputEventKey.new()
	ev.keycode = Key.KEY_E
	ev.ctrl_pressed = true  # Mac 上 Command 键
	ev.command_or_control_autoremap = true

	dock_shortcut = Shortcut.new()
	dock_shortcut.events = [ev]
		
	add_control_to_bottom_panel(root_area,"QFramework",dock_shortcut)
	
	QFEditorGlobal.latest_main_package.register_with_init_value(func(_latest_main_package:MainPackageData):
		if _latest_main_package == null:
			_btn_update.hide()
			_server_version_label.hide()
			_server_version_update_info.hide()
		else:
			_server_version_label.show()
			_server_version_label.text = "服务器版本:" + _latest_main_package.version
			
			if VersionHelper.is_greator(_latest_main_package.version,get_plugin_version()):
				_server_version_update_info.text = "版本信息：" + _latest_main_package.updateInfo
				_server_version_update_info.show()
				_btn_update.show()
			else:
				_btn_update.hide()
				_server_version_update_info.hide()
	).un_regsiter_when_node_existing_tree(self)
	
	LatestMainPackageQuery.do(self)
	
	root_area.visibility_changed.connect(func():
		if root_area.visible:
			LatestMainPackageQuery.do(self)
	)

func _show_detail_area_by_name(_name:String):
	
	show_main()
	
	for feature_item in feature_items:
		if feature_item.feature_name == _name:
			feature_item.side_bar_button.disabled = true
			feature_item.detail_area.show()
			
func show_main():
	for feature_item in feature_items:
		feature_item.side_bar_button.disabled = false
		feature_item.detail_area.hide()
		
func _exit_tree() -> void:
	# 从 dock 移除面板
	if root_area:
		remove_control_from_bottom_panel(root_area)
		root_area.free()
		root_area = null
