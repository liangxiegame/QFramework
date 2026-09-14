extends AbstractViewBuilder

class_name FluentTextEditBuilder

func default_text(text:String)->FluentTextEditBuilder:
	var p = func(t:TextEdit): t.text = text
	property_setters.append(p)
	return self
	
func text_changed(changed:Callable)->FluentTextEditBuilder:
	var p = func(t:TextEdit): t.text_changed.connect(changed)
	property_setters.append(p)
	return self

func build():
	return apply_properties(TextEdit.new())
