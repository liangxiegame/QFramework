class_name QueryExampleAppArchitecture extends Architecture

func init():
	register_model(QueryExampleController.StudentModel.new())
	register_model(QueryExampleController.TeacherModel.new())
	
