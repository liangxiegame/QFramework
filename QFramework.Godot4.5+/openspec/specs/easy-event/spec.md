# easy-event Specification

## Purpose
TBD - created by archiving change add-or-event-regression-tests. Update Purpose after archive.
## Requirements
### Requirement: 事件注销操作幂等

事件系统（`Event` 与 `OrEvent`）的注销操作 `un_register` MUST 是幂等的。对同一回调重复注销、对一个从未注册过的回调调用注销，MUST NOT 触发崩溃或导致对象被提前回收。注销实现 MUST NOT 依赖手动 `reference()/unreference()` 这种需要严格配平的计数机制。

#### Scenario: 重复注销同一回调不崩溃（OrEvent）
- **WHEN** 对一个已注册的 `OrEvent` 回调连续调用两次 `un_register`
- **THEN** 程序 MUST NOT 崩溃，且第二次调用为无操作

#### Scenario: 重复注销同一回调不崩溃（基础 Event）
- **WHEN** 对一个已注册的 `Event` 回调连续调用两次 `un_register`
- **THEN** 程序 MUST NOT 崩溃，且第二次调用为无操作

#### Scenario: UnRegister 对象重复注销不崩溃
- **WHEN** 对同一个 `UnRegister` 对象连续调用两次 `un_register()`
- **THEN** 程序 MUST NOT 崩溃

#### Scenario: 对未注册的回调调用注销不崩溃
- **WHEN** 调用 `un_register` 传入一个从未注册过的回调
- **THEN** 程序 MUST NOT 崩溃，且为无操作

#### Scenario: 注销后触发事件不再执行回调
- **WHEN** 注销某回调后触发对应事件
- **THEN** 该回调 MUST NOT 被执行

### Requirement: OrEvent 生命周期不依赖手动引用计数

`OrEvent` 的存活 MUST 由 Godot 的自动 `RefCounted` 机制维持——即子事件持有的 `self.trigger` `Callable` 以及 `UnRegister._event` 引用。`OrEvent` 的 `register`/`un_register` 实现 MUST NOT 调用 `self.reference()` 或 `self.unreference()`。注册后即使丢弃局部 `OrEvent` 引用，触发任一子事件 MUST 仍能执行用户回调。

#### Scenario: 丢弃局部 OrEvent 引用后仍可触发
- **WHEN** 创建 `OrEvent` 并注册回调后，不再持有任何局部 `OrEvent` 变量，随后触发其任一子事件
- **THEN** 用户回调 MUST 仍被执行，证明 `OrEvent` 未被提前回收

#### Scenario: 注册后 weakref 保持有效
- **WHEN** 对一个已注册的 `OrEvent` 取 `weakref`，随后触发其子事件
- **THEN** `weakref.get_ref()` MUST 返回有效对象（非 null）

#### Scenario: 完整注销后无泄漏
- **WHEN** 注销 `OrEvent` 回调并释放对应的 `UnRegister` 引用后
- **THEN** `OrEvent` 的 `weakref.get_ref()` MUST 返回 null，表明对象已被正常回收

#### Scenario: or 语义触发
- **WHEN** 对 `event_a.or_event(event_b)` 注册回调后，分别触发 `event_a` 与 `event_b`
- **THEN** 两次触发 MUST 各执行一次用户回调

