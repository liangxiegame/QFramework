extends AbstractViewBuilder

class_name FluentVBoxBuilder

func build():	
	return apply_properties(build_children(VBoxContainer.new()))
