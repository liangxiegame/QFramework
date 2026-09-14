extends AbstractViewBuilder

class_name FluentHBoxBuilder

func build():	
	return apply_properties(build_children(HBoxContainer.new()))
