class_name QFEditorGlobal

static var _latest_main_package:BindableProperty = null
static var latest_main_package:BindableProperty:
	get:
		if _latest_main_package == null: _latest_main_package = BindableProperty.new(null)
		return _latest_main_package

static func init():
	latest_main_package.value = null
