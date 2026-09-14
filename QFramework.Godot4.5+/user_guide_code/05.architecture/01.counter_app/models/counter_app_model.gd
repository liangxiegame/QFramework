class_name CounterAppModel extends ICounterAppModel # +-

var count:BindableProperty = BindableProperty.new(0)

func get_count()->BindableProperty: # +-
	return count #+-

# 持有 storage_utility
var _storage_utility:StorageUtility = null

func init():
	
	# 获取 Utility
	_storage_utility = CounterApp.get_utility(StorageUtility.NAME) as StorageUtility
	
	# 加载初始数据
	count.value = _storage_utility.load_int("count",0)
	
	# 数据变更时 保存数据
	count.register(func(_count:int):
		_storage_utility.save_int("count",_count) 
	).un_register_when_created_child_node_destroyed(self)
	
func _exit_tree() -> void:
	_storage_utility = null
