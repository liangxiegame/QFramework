extends Node

class_name NodeWithDestroyEvent

signal destroy

func _notification(what: int) -> void:
	if what == NOTIFICATION_PREDELETE:
		_destroy()
		destroy.emit()
		
func _destroy():
	pass
 
