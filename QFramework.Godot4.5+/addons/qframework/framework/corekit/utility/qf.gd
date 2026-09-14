class_name QF

static func get_tree()->SceneTree:return Engine.get_main_loop() as SceneTree

static var wait_one_frame:Signal:
	get:return get_tree().process_frame

static func wait_frames(frame_count:int):
	for _i in frame_count:
		await wait_one_frame
	
static func wait_seconds(seconds:float):
	await get_tree().create_timer(seconds).timeout
