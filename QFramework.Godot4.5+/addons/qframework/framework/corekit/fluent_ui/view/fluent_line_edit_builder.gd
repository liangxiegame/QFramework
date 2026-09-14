extends AbstractViewBuilder

class_name FluentLineEditBuilder

func default_text(text:String)->FluentLineEditBuilder:
	var p = func(t:LineEdit): t.text = text
	property_setters.append(p)
	return self
	
func text_changed(changed:Callable)->FluentLineEditBuilder:
	var p = func(t:LineEdit): t.text_changed.connect(func(_new_text:String):changed.call())
	property_setters.append(p)
	return self
	

func font_size(font_size:int)->FluentLineEditBuilder:
	var p = func(t:LineEdit): t.add_theme_font_size_override("font_size",font_size)
	property_setters.append(p)
	return self
	
func password_mode()->FluentLineEditBuilder:
	var p = func(t:LineEdit): t.secret = true
	property_setters.append(p)
	return self

func build():
	return apply_properties(LineEdit.new())
