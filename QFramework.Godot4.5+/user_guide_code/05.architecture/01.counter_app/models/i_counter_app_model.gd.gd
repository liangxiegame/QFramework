@abstract
class_name ICounterAppModel extends AbstractModel

const NAME:String = "ICounterAppModel"

func get_model_name() -> String: return NAME

@abstract func get_count()->BindableProperty 
