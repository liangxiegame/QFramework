extends AbstractViewBuilder

class_name FluentLabelBuilder

func text(_text:String)->FluentLabelBuilder:
	property_setters.append(func(label:Label):label.text = _text)
	return self
	
func font_size(_size:int)->FluentLabelBuilder:
	property_setters.append(func(label:Label):
		_make_sure_label_settings(label).font_size = _size
	)
	return self
	
func _make_sure_label_settings(label:Label):
	if not label.label_settings:
		label.label_settings = LabelSettings.new()
		
	return label.label_settings
			
func h_aligh_center()->FluentLabelBuilder:
	property_setters.append(func(label:Label):label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER)
	return self
	
func build():
	return apply_properties(Label.new())
