@abstract
class_name AbstractQuery

var architecture:Architecture

@abstract
func do()

func get_system(_name:String)->AbstractSystem: return architecture.get_system(_name)

func get_model(_name:String)->AbstractModel: return architecture.get_model(_name)

func get_utility(_name:String)->AbstractUtility: return architecture.get_utility(_name)
