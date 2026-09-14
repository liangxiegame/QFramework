class_name FluentGridBuilder extends AbstractViewBuilder

func columns(_columns:int)->FluentGridBuilder:
	property_setters.append(func(g:GridContainer): g.columns = _columns)
	return self

func build():	
	return apply_properties(build_children(GridContainer.new()))
