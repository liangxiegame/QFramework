@abstract
class_name AbstractSystem extends Node

var architecture:Architecture

func get_system(_name:String)->AbstractSystem: return architecture.get_system(_name)
func get_model(_name:String)->AbstractModel: return architecture.get_model(_name)
func get_utility(_name:String)->AbstractUtility: return architecture.get_utility(_name)

@abstract
func init()

@abstract func get_system_name()->String
