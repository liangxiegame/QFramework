extends Node2D

class_name QueryExampleController

class StudentModel extends AbstractModel:
	var student_names:Array[String] = [
		"张三",
		"李四"
	]
	
	func get_model_name(): return "StudentModel"
	func init(): pass
	
class TeacherModel extends AbstractModel:
	var teacher_names:Array[String] = [
		"王五",
		"赵六"
	]
	
	func get_model_name(): return "TeacherModel"
	func init(): pass
	
class SchoolAllPersonCountQuery extends AbstractQuery:
	
	func do():
		var student_model = get_model("StudentModel") as StudentModel
		var teacher_model = get_model("TeacherModel") as TeacherModel
		var student_count = student_model.student_names.size()
		var teacher_count = teacher_model.teacher_names.size()
		var total_count = student_count + teacher_count
		return total_count
		
		
func _ready() -> void:
	var total_count = await QueryExampleApp.send_query(SchoolAllPersonCountQuery.new())
	print(total_count)
	
