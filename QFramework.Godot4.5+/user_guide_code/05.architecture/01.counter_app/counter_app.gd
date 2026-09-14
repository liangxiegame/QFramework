class_name CounterAppArchitecture extends Architecture

func init():
	register_system(AchievementSystem.new())
	register_model(CounterAppModel.new())
	register_utility(StorageUtility.new())
