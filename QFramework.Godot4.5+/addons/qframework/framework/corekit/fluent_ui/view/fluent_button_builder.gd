extends AbstractViewBuilder

class_name FluentButtonBuilder

func text(text:String)->FluentButtonBuilder:
	var s = func(button:Button):  button.text = text
	property_setters.append(s)
	return self

func pressed(on_pressed:Callable)->FluentButtonBuilder:
	var s = func(button:Button): button.pressed.connect(on_pressed)
	property_setters.append(s)
	return self
	
func min_height(_height:float)->FluentButtonBuilder:
	var s = func(button:Button): button.custom_minimum_size.y  = _height
	property_setters.append(s)
	return self
	
func font_size(_font_size:int)->FluentButtonBuilder:
	var s = func(button:Button):  \
		button.add_theme_font_size_override("font_size",_font_size) 
	
	property_setters.append(s)
	return self
	

func build():
	return apply_properties(Button.new())
