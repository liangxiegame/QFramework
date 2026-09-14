class_name EditorTask

enum Status {
	NotStart,
	Started,
	Finished
}

var editor_plugin:EditorPlugin
var status:Status = Status.NotStart
var callable:Callable

func start():
	status == Status.Started
	await callable.call()
	status == Status.Finished
