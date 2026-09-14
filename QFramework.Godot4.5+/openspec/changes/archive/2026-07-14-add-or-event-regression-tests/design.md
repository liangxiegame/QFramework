## Context

`OrEvent` 曾使用叠在 Godot `RefCounted` 之上的手动引用计数（`self.reference()` / `self.unreference()`），导致重复注销时计数被打穿、对象在 `un_register` 执行中途被回收、预览闪退。手动计数已移除，`or_event.gd` 现仅做"从子事件移除 `Callable` + 移除用户回调"，天然幂等。

当前项目**无任何测试框架**——现有 `playground/` 下的 "test" 全是手动按键、肉眼观察 `print` 的场景脚本，无法自动断言、无法防回归、无法接入 CI。本设计为项目首次引入单元测试基础设施，并立刻用它守护本次 `OrEvent` 修复的两条契约（见 `specs/easy-event/spec.md`）：注销幂等、生命周期不依赖手动引用计数。

## Goals / Non-Goals

**Goals:**
- 引入 GUT 作为标准化单元测试框架，支持编辑器内与命令行 headless 两种运行方式。
- 用自动断言（而非 `print` 肉眼）固化 `OrEvent` 闪退修复，防止回退到手动引用计数。
- 用 `weakref` + `is_instance_valid` 实证验证"移除手动引用计数后 `OrEvent` 仍被正确保活、且无泄漏"。
- 建立可复用的测试目录与运行约定，供后续模块照搬。

**Non-Goals:**
- 不改动框架业务代码（`or_event.gd` 的修复已在本次之前完成）。
- 不把已有 `playground/` 场景脚本迁移为 GUT 测试。
- 不接入 CI（命令行可跑即满足前提，CI 留作未来 change）。
- 不为整个框架补齐全量测试覆盖——仅覆盖本次涉及的 `EasyEvent` / `OrEvent` 契约。

## Decisions

### 决策 1：选择 GUT，而非手动断言脚本

- **选择**：GUT（bitwes/Gut）。
- **理由**：Godot 4 社区事实标准的单测框架；提供 `assert_eq/assert_true/assert_not_null`、`before_each/after_each`、命令行运行器 `gut_cmdln.gd`，未来可直接接 CI。
- **备选**：在 `playground/` 写带 GDScript `assert()` 的手动脚本——零依赖、贴合现有风格，但无标准化断言/ fixture、无 CI 路径，且学员难以一眼判断通过/失败。被否。
- **备选**：Godot 原生无成熟单测方案，不构成选项。

### 决策 2：GUT 安装方式——release 解压到 `addons/gut`

- **选择**：下载 GUT release zip，解压为 `addons/gut/`，在 `project.godot` 启用插件。
- **理由**：可复现、不依赖编辑器 AssetLib 图形界面、不引入 git submodule 的复杂度。第三方代码隔离在 `addons/gut/` 单一目录，便于版本锁定与回滚。
- **版本**：选定 **GUT v9.5.0**。实测发现 v9.6.0+ 的 `gut_plugin.gd` 使用 `EditorDock`/`add_dock()` 等 Godot **4.6+** 编辑器 API，在 4.5 下编辑器加载会解析失败并禁用插件（命令行 `gut_cmdln.gd` 不受影响）。v9.5.0 是兼容 4.5 的最新版；升级 Godot 4.6+ 后可换用最新 GUT。

### 决策 3：测试分四组，D 组用 weakref 自动断言生命周期

- **选择**：`test_or_event.gd` 内分 A 功能 / B 注销正确性 / C 幂等崩溃回归 / D 生命周期四组。
- **理由**：C 组直接回归学员反馈的闪退；D 组验证"移除手动引用计数"这一假设的安全性，是本 change 的灵魂。
- **生命周期观测手法**：复用项目已有模式 `08.easy_event_exiting_tree_test` 中的 `weakref(obj).get_ref()` + `is_instance_valid(obj)`，但改为自动 `assert_*` 断言而非 `print`。
- **关键技巧**：用"创建 OrEvent → 注册 → 立即丢弃局部变量 → 触发子事件"来证伪"OrEvent 被提前回收"；用"完整注销 + 释放 UnRegister → weakref 变 null"来证伪泄漏。

### 决策 4：命令行运行通过 Godot.app 路径调用

- **选择**：文档与运行脚本中使用 `/Applications/Godot.app/Contents/MacOS/Godot --headless -s res://addons/gut/gut_cmdln.gd -gdir=res://tests/unit -gexit`。
- **理由**：本机 `godot` 未进 PATH，仅有 `.app`。此为环境事实，需在测试说明中显式记录，避免他人复现失败。

## Risks / Trade-offs

- **[引入第三方依赖 GUT]** → 可接受：MIT 协议、Godot 社区主流、隔离在 `addons/gut/` 单目录。Mitigation：锁定版本并在文档登记；回滚仅需删目录 + 还原 `project.godot`。
- **[GUT 版本与 Godot 4.5/4.6 兼容性]** → Mitigation：apply 时选兼容版本并实跑验证；若不兼容则降级到手动断言脚本方案（决策 1 的备选）。
- **["Callable 持有 RefCounted 强引用"是移除手动计数的理论前提]** → 若该前提在某 Godot 版本不成立，D 组测试会捕获失败。**这正是测试的价值**——把隐含假设变成可执行断言。
- **[Godot 不在 PATH]** → Mitigation：文档显式记录 `.app` 调用方式，并提供可复制命令。

## Migration Plan

- **部署**：解压 GUT 到 `addons/gut/` → 编辑 `project.godot` 启用插件 → 新增 `tests/unit/` 与 `.gutconfig.json` → 编写测试 → 命令行实跑。
- **回滚**：删除 `addons/gut/`、`tests/`、`.gutconfig.json`，从 `project.godot` 的 `[editor_plugins] enabled` 移除 `res://addons/gut/plugin.cfg`。框架业务代码不受影响。

## Open Questions

- GUT 具体版本号？→ apply 阶段确定（选 Godot 4 兼容最新稳定版）。
- 是否在 `doc/` 下新增一篇"如何运行测试"文档？→ 倾向是，作为 task 的一部分。
