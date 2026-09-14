class_name AchievementSystem extends AbstractSystem

const NAME:String = "AchievementSystem"

func get_system_name() -> String: return NAME

func init():
	var model = CounterApp.get_model(ICounterAppModel.NAME) as ICounterAppModel

	model.get_count().register(func(_count:int): # +-
		if _count == 10: 
			print("触发 点击达人 成就")
		elif _count == 20: 
			print("触发 点击专家 成就")	
	).un_register_when_created_child_node_destroyed(self)
	
