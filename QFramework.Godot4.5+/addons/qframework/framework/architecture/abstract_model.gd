@abstract
class_name AbstractModel extends Node

var architecture:Architecture

func get_system(_name:String)->AbstractSystem: return architecture.get_system(_name)
func get_model(_name:String)->AbstractModel: return architecture.get_model(_name)
func get_utility(_name:String)->AbstractUtility: return architecture.get_utility(_name)

@abstract
func get_model_name()

@abstract
func init()
