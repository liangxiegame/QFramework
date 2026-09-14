extends GutTest

# ============================================================
# OrEvent 回归测试
# 起因：学员反馈手动重复注销 -> 内部 unreference() 多次执行 -> 预览闪退。
# 修复已移除 or_event.gd 的手动 reference()/unreference()，
# 本套件固化以下契约（见 specs/easy-event/spec.md）：
#   A. 功能正确性（or 语义）
#   B. 注销正确性
#   C. 幂等性 / 闪退回归（核心）
#   D. 生命周期不依赖手动引用计数（核心）
# 注：GDScript lambda 对 int 按值捕获，计数器须用数组（引用语义）
# ============================================================


# ---------------- A. 功能正确性 ----------------

func test_or_event_triggers_on_any_child():
	# or 语义：任一子事件触发都应执行回调
	var count = [0]
	var event_a = EasyEvent.new()
	var event_b = EasyEvent.new()
	var unreg = event_a.or_event(event_b).register(func(): count[0] += 1)
	event_a.trigger()
	assert_eq(count[0], 1, "触发 event_a 应执行回调一次")
	event_b.trigger()
	assert_eq(count[0], 2, "触发 event_b 也应执行回调")
	unreg.un_register()

func test_three_chain_or_event():
	# 三连 or_event：a / b / c 任一触发都执行
	var count = [0]
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var c = EasyEvent.new()
	var unreg = a.or_event(b).or_event(c).register(func(): count[0] += 1)
	a.trigger()
	assert_eq(count[0], 1, "触发 a")
	b.trigger()
	assert_eq(count[0], 2, "触发 b")
	c.trigger()
	assert_eq(count[0], 3, "触发 c")
	unreg.un_register()


# ---------------- B. 注销正确性 ----------------

func test_unregister_stops_callback():
	var count = [0]
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var unreg = a.or_event(b).register(func(): count[0] += 1)
	unreg.un_register()
	a.trigger()
	b.trigger()
	assert_eq(count[0], 0, "注销后触发子事件不应再执行回调")

func test_unregister_removes_trigger_from_children():
	# 注销后子事件的 callables 不应残留 self.trigger
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var or_ev = a.or_event(b)
	var cb = func(): pass
	or_ev.register(cb)
	assert_eq(a.callables.size(), 1, "注册后 event_a 持有 trigger callable")
	assert_eq(b.callables.size(), 1, "注册后 event_b 持有 trigger callable")
	or_ev.un_register(cb)
	assert_eq(a.callables.size(), 0, "注销后 event_a 不再持有 trigger")
	assert_eq(b.callables.size(), 0, "注销后 event_b 不再持有 trigger")


# ---------------- C. 幂等性 / 闪退回归（学员反馈的 bug）----------------

func test_unregister_same_callable_twice_no_crash():
	# 曾因 OrEvent.un_register 内部 unreference() 被多次调用而闪退
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var or_ev = a.or_event(b)
	var cb = func(): pass
	or_ev.register(cb)
	or_ev.un_register(cb)
	or_ev.un_register(cb)  # 重复注销：必须不崩溃
	assert_true(is_instance_valid(or_ev), "OrEvent 重复注销同一回调不应崩溃")

func test_unregister_object_twice_no_crash():
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var unreg = a.or_event(b).register(func(): pass)
	unreg.un_register()
	unreg.un_register()  # UnRegister 对象重复注销：必须不崩溃
	assert_true(is_instance_valid(unreg), "UnRegister 重复注销不应崩溃")

func test_unregister_never_registered_callable_no_crash():
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var or_ev = a.or_event(b)
	or_ev.un_register(func(): pass)  # 对从未注册的回调注销：必须不崩溃
	assert_true(is_instance_valid(or_ev), "对未注册回调注销不应崩溃")


# ---------------- D. 生命周期（验证移除手动引用计数的安全性）----------------

func _make_or_and_register(a: EasyEvent, b: EasyEvent, cb: Callable) -> UnRegister:
	# 在独立作用域创建 OrEvent 并注册；返回后局部 OrEvent 引用即释放，
	# 仅靠子事件的 Callable 与 UnRegister._event 维持其存活。
	return a.or_event(b).register(cb)

func test_or_event_survives_after_dropping_local_ref():
	# 关键：OrEvent 不再被任何局部变量持有，触发子事件仍应执行回调，
	# 证明 Callable + UnRegister 已自动保活，无需手动 reference()
	var count = [0]
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var unreg = _make_or_and_register(a, b, func(): count[0] += 1)
	a.trigger()
	assert_eq(count[0], 1, "丢弃局部 OrEvent 引用后触发 a 仍应执行（未被提前回收）")
	b.trigger()
	assert_eq(count[0], 2, "触发 b 也应执行")
	unreg.un_register()

func test_weakref_valid_after_register():
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var or_ev = a.or_event(b)
	var wr = weakref(or_ev)
	var unreg = or_ev.register(func(): pass)
	a.trigger()  # 触发一次，确认对象存活
	assert_not_null(wr.get_ref(), "注册并触发后 OrEvent 的 weakref 应仍有效")
	unreg.un_register()
	or_ev = null

func test_no_leak_after_full_unregister():
	# 完整注销并释放 UnRegister 后，OrEvent 应被回收（无泄漏）
	var a = EasyEvent.new()
	var b = EasyEvent.new()
	var or_ev = a.or_event(b)
	var wr = weakref(or_ev)
	var unreg = or_ev.register(func(): pass)
	or_ev = null            # 释放局部引用
	unreg.un_register()     # 从子事件移除 trigger callable
	unreg = null            # 释放 UnRegister -> _event 断开 -> OrEvent 无强引用
	assert_null(wr.get_ref(), "完整注销后 OrEvent 应被回收，weakref 变 null（无泄漏）")
