extends AbstractViewBuilder

class_name FluentCheckBoxBuilder

func toggled(_toggled:Callable)->FluentCheckBoxBuilder:
	property_setters.append(func(c:CheckBox):c.toggled.connect(_toggled))
	return self
	
	
func is_on(_is_on:bool)->FluentCheckBoxBuilder:
	property_setters.append(func(c:CheckBox):c.button_pressed = _is_on)
	return self
	

func build():
	
	
	return apply_properties(CheckBox.new())
