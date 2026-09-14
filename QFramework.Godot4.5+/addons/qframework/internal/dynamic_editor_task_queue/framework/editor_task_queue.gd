class_name EditorTaskQueue

var task_queue:Array[EditorTask] = []

func push_task(task:EditorTask):
	task_queue.push_back(task)
	
func push_callable(callable:Callable):
	var task = EditorTask.new()
	task.callable = callable
	task_queue.push_back(task)
	
func process(delta: float,editor_plugin:EditorPlugin) -> bool:
	if task_queue.size() > 0:
		if task_queue[0].status == EditorTask.Status.NotStart:
			task_queue[0].editor_plugin = editor_plugin
			task_queue[0].start()
		elif task_queue[0].status == EditorTask.Status.Finished:
			task_queue[0].editor_plugin = null
			task_queue.pop_back()
		return true
	else:
		return false
