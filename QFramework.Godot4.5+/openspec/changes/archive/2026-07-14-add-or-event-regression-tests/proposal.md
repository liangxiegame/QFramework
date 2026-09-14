## Why

有学员反馈：在使用 `or_event` 合并注册后手动注销事件时，重复调用注销会导致 `OrEvent.un_register` 内部的 `unreference()` 被多次执行，引用计数被打到 0、对象在方法执行中途被 Godot 回收，预览直接闪退（use-after-free）。

根因是 `or_event.gd` 用了一个叠在 Godot 自动 `RefCounted` 之上的**手动引用计数**（`self.reference()` / `self.unreference()`），它必须严格配平，任何重复注销都会击穿。该手动计数本身也很可能冗余——`Callable` 与 `UnRegister._event` 已经会自动持有 `OrEvent`。

手动计数已在本次之前移除（`or_event.gd` 现只做"从子事件移除 Callable + 移除用户回调"，天然幂等）。但当前项目**没有任何测试框架**，既无法防止该闪退回归，也无法证伪"移除手动引用计数后 `OrEvent` 仍能被正确保活"这一关键假设。现在引入测试基础设施并固化契约，正是时机。

## What Changes

- **首次引入 GUT（Godot Unit Test）单元测试框架**，作为项目的测试基础设施。
- 建立测试目录约定 `tests/unit/` 与运行配置 `.gutconfig.json`，支持编辑器内与命令行 headless 两种运行方式。
- 为 `EasyEvent` / `OrEvent` 编写回归测试套件，覆盖四组场景：
  - A. 功能正确性（`or_event` 合并语义、任一子事件触发即执行回调）
  - B. 注销正确性（注销后不再触发、子事件无 Callable 残留）
  - C. **幂等性 / 闪退回归**（重复注销、对未注册回调注销均不崩溃）
  - D. **生命周期**（丢弃局部 `OrEvent` 引用仍可触发；`weakref` 验证无提前回收、无泄漏）
- 固化两条行为契约，防止未来再次回退到"手动引用计数"这种脆弱实现。

## Capabilities

### New Capabilities
- `easy-event`: 事件注册/注销的行为契约——注销操作幂等（重复注销、对未注册回调注销均安全，不崩溃）；`OrEvent` 的生命周期由 `Callable` 与 `UnRegister` 自动持有，不依赖手动 `reference()/unreference()`。
- `test-infrastructure`: GUT 单元测试基础设施——目录约定、配置文件、命令行与编辑器运行方式。

### Modified Capabilities
<!-- 现有 openspec/specs/ 为空，无已存在的能力被修改。 -->

## Impact

- **新增依赖**：`addons/gut/`（第三方插件 [bitwes/Gut](https://github.com/bitwes/Gut)），需在 `project.godot` 中启用。
- **新增产物**：`tests/unit/`、`.gutconfig.json`、`tests/unit/test_easy_event.gd`、`tests/unit/test_or_event.gd`。
- **框架业务代码**：不改动。`or_event.gd` 移除手动引用计数的修复已在本次之前完成，本 change 只负责为其建立回归守护。
- **运行环境**：命令行跑测试需 Godot 可执行文件（本机为 `/Applications/Godot.app`，`godot` 未进 PATH，需在文档中说明调用方式）。
