extends GutTest

# ============================================================
# EasyEvent（基础 Event）回归测试 —— 对照组
# 验证基础事件注册/触发/注销主路径，以及注销幂等性
# 注：GDScript lambda 对 int 按值捕获，计数器须用数组（引用语义）
# ============================================================

func test_event_register_and_trigger():
	# 注册 -> 触发 -> 注销 主路径
	var count = [0]
	var ev = EasyEvent.new()
	var unreg = ev.register(func(): count[0] += 1)
	ev.trigger()
	assert_eq(count[0], 1, "触发应执行回调一次")
	unreg.un_register()
	ev.trigger()
	assert_eq(count[0], 1, "注销后再次触发不应执行回调")

func test_event_trigger_with_argument():
	# 带参触发应把参数传给回调
	var got = [null]
	var ev = EasyEvent.new()
	var unreg = ev.register(func(x): got[0] = x)
	ev.trigger(42)
	assert_eq(got[0], 42, "带参触发应传递参数")
	unreg.un_register()

func test_event_unregister_idempotent():
	# 基础 Event 注销幂等：重复注销 + 对未注册回调注销均不崩溃
	var ev = EasyEvent.new()
	var cb = func(): pass
	ev.register(cb)
	ev.un_register(cb)
	ev.un_register(cb)               # 重复注销同一回调
	ev.un_register(func(): pass)     # 对未注册的回调注销
	assert_true(is_instance_valid(ev), "基础 Event 重复/未注册注销不应崩溃")

func test_event_unregister_object_twice_no_crash():
	# UnRegister 对象重复注销不崩溃
	var ev = EasyEvent.new()
	var unreg = ev.register(func(): pass)
	unreg.un_register()
	unreg.un_register()
	assert_true(is_instance_valid(unreg), "UnRegister 重复注销不应崩溃")
