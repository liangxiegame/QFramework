extends Node2D


func _ready() -> void:
	test_func("a",1)
	pass
	
func test_func(...abc:Array):
	test_funcb(abc)
	
	pass
func test_funcb(...abc:Array):
	for a in abc:
		print(a)
