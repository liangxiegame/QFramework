extends AbstractViewBuilder

class_name FluentPanelBuilder

func build():	
	return apply_properties(build_children(PanelContainer.new()))
