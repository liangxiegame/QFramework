extends Node2D

class_name EasyEventTriggerWithObj

# 定义事件 模拟当学生添加成功事件
var on_student_added:EasyEvent = EasyEvent.new()

# 定义学生对象，
class Student:
	var name:String
	var age:int
	
func _ready() -> void:
	# 注册事件
	on_student_added.register(func(student:Student):
		print("学生添加成功: 姓名 %s 年龄 %d" % [student.name,student.age])
	).un_register_when_node_exiting_tree(self)
	
func _process(_delta: float) -> void:
	
	# 当空格键按下时
	if Input.is_key_pressed(KEY_SPACE):
		
		# 定义要发送的数据
		var student = Student.new()
		student.name = "凉鞋"
		student.age = 33
		
		# 触发事件
		on_student_added.trigger(student)
