## 1. 引入 GUT 测试框架

- [x] 1.1 下载 GUT（Godot 4 兼容最新稳定版）release，解压到 `addons/gut/`
- [x] 1.2 在 `project.godot` 的 `[editor_plugins] enabled` 中加入 `res://addons/gut/plugin.cfg`
- [x] 1.3 创建 `tests/unit/` 目录与根目录 `.gutconfig.json`（`dirs` 指向 `res://tests/unit`）

## 2. EasyEvent 基础回归（对照组）

- [x] 2.1 创建 `tests/unit/test_easy_event.gd`（`extends GutTest`），覆盖基础 `Event` 的注册 → 触发 → 注销主路径
- [x] 2.2 验证基础 `Event` 注销幂等：重复注销同一回调、对未注册回调调用 `un_register`，均不崩溃且为无操作

## 3. OrEvent 功能与注销正确性

- [x] 3.1 创建 `tests/unit/test_or_event.gd`（`extends GutTest`）A 组：`event_a.or_event(event_b).or_event(event_c)` 注册后，分别触发 a/b/c，断言回调各执行一次
- [x] 3.2 B 组：注销后触发子事件断言回调不再执行；并断言子事件 `callables` 中已不含 `self.trigger`（无残留）

## 4. OrEvent 幂等性 / 闪退回归（核心）

- [x] 4.1 C 组：对已注册的 `OrEvent` 回调连续调用两次 `un_register`，断言不崩溃
- [x] 4.2 C 组：对同一 `UnRegister` 对象连续调用两次 `un_register()`，断言不崩溃
- [x] 4.3 C 组：对从未注册过的回调调用 `OrEvent.un_register`，断言不崩溃且为无操作

## 5. OrEvent 生命周期验证（核心）

- [x] 5.1 D 组：创建 `OrEvent` 注册回调后立即丢弃局部变量，触发子事件，断言回调仍执行（证伪提前回收）
- [x] 5.2 D 组：注册后对其取 `weakref`，触发子事件，断言 `get_ref()` 返回非 null
- [x] 5.3 D 组：完整注销并释放 `UnRegister` 引用后，断言 `weakref.get_ref()` 返回 null（证伪泄漏）

## 6. 运行验证与文档

- [x] 6.1 通过 `/Applications/Godot.app/Contents/MacOS/Godot --headless -s res://addons/gut/gut_cmdln.gd -gdir=res://tests/unit -gexit` 运行全部测试，确认全绿
- [x] 6.2 在 `doc/` 下新增"如何运行单元测试"说明，登记 GUT 版本与 `.app` 命令行调用方式
- [x] 6.3 运行 `openspec validate add-or-event-regression-tests` 通过
