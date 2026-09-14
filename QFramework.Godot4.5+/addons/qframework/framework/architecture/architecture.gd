@abstract
class_name Architecture extends Node

func send_command(command:AbstractCommand):
	command.architecture = self
	var ret_value = await command.execute()
	command.architecture = null
	return ret_value

func send_query(query:AbstractQuery):
	query.architecture = self
	var ret_value = await query.do()
	query.architecture = null
	return ret_value
	
@abstract
func init()

var models:Dictionary[String,AbstractModel] = {}
var systems:Dictionary[String,AbstractSystem] = {}
var utilities:Dictionary[String,AbstractUtility] = {}

var system_layer:Node
var model_layer:Node
var utility_layer:Node

func register_model(model:AbstractModel)->AbstractModel:
	models[model.get_model_name()] = model
	return model
	
func register_system(system:AbstractSystem)->AbstractSystem:
	systems[system.get_system_name()] = system
	return system
	
func register_utility(utility:AbstractUtility)->AbstractUtility:
	utilities[utility.get_utility_name()] = utility
	return utility
	
func get_model(model_name:String)->AbstractModel:
	return models[model_name]
	
func get_system(system_name:String)->AbstractSystem:
	return systems[system_name]
	
func get_utility(utility_name:String)->AbstractUtility:
	return utilities[utility_name]

func _ready() -> void:
	
	init()
	system_layer = Node.new()
	system_layer.name = "System"
	self.add_child(system_layer)
	
	model_layer = Node.new()
	model_layer.name = "Model"
	self.add_child(model_layer)
	
	utility_layer = Node.new()
	utility_layer.name = "Utility"
	self.add_child(utility_layer)
	
	for utility in utilities.values():
		utility.architecture = self
		utility_layer.add_child(utility)	
		utility.name = utility.get_utility_name()
		utility.init()
	
	for model in models.values():
		model.architecture = self
		model_layer.add_child(model)	
		model.name = model.get_model_name()
		model.init()
		
	for system in systems.values():
		system.architecture = self
		system_layer.add_child(system)
		system.name = system.get_system_name()
		system.init()
