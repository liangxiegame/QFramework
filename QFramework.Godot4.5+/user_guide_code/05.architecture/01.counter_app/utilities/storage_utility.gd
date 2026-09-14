class_name StorageUtility extends AbstractUtility

const NAME:String = "StorageUtility"

func get_utility_name() -> String: return NAME

# config file 的方式存储
var config_file:ConfigFile = ConfigFile.new()

# 存储路径
const SAVE_PATH:String = "user://save.cfg"

func init():
	# 初始化时 加载存储文件
	config_file.load(SAVE_PATH)
	
# 加载 int API
func load_int(key:String,defaultValue:int = 0)->int:
	return config_file.get_value("storage",key,defaultValue)

# 保存 int API
func save_int(key:String,value:int):
	config_file.set_value("storage",key,value)
	config_file.save(SAVE_PATH)
