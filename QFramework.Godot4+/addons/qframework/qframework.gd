@tool
extends EditorPlugin

var window

func _enter_tree():
	# Initialization of the plugin goes here.
	window = preload("res://addons/qframework/package_window.tscn").instantiate()
	
	add_control_to_dock(DOCK_SLOT_LEFT_UL,window)

func _exit_tree():
	# Clean-up of the plugin goes here.
	remove_control_from_docks(window)
	window.free()
