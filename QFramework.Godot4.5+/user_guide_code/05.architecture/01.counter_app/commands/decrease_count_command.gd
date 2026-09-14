class_name DecreaseCountCommand extends AbstractCommand

func execute():
	var model = get_model(ICounterAppModel.NAME) as ICounterAppModel # +-
	model.get_count().value -= 1 # +-
