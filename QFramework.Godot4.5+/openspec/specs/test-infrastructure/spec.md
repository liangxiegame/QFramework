# test-infrastructure Specification

## Purpose
TBD - created by archiving change add-or-event-regression-tests. Update Purpose after archive.
## Requirements
### Requirement: 集成 GUT 单元测试框架

项目 SHALL 集成 [GUT（Godot Unit Test）](https://github.com/bitwes/Gut) 作为单元测试框架，并在 `project.godot` 中启用对应插件。所有单元测试脚本 MUST 继承 `GutTest`，使用 `assert_eq` / `assert_true` / `assert_not_null` 等断言。

#### Scenario: 测试脚本继承 GutTest
- **WHEN** 检查项目中的单元测试脚本
- **THEN** 每个测试脚本 MUST `extends GutTest`

#### Scenario: 插件已启用
- **WHEN** 检查 `project.godot`
- **THEN** `[editor_plugins]` 的 enabled 列表 MUST 包含 `res://addons/gut/plugin.cfg`

### Requirement: 单元测试目录约定

单元测试 SHALL 集中放置在 `tests/unit/` 目录下，按被测模块组织文件。根目录 SHALL 提供一份 `.gutconfig.json` 用于配置命令行运行的测试目录与选项。

#### Scenario: 测试位于统一目录
- **WHEN** 查看项目测试文件
- **THEN** 单元测试 MUST 位于 `tests/unit/` 下

#### Scenario: 存在 gutconfig 配置
- **WHEN** 检查项目根目录
- **THEN** MUST 存在 `.gutconfig.json`，且 `dirs` 指向 `res://tests/unit`

### Requirement: 可通过命令行 headless 运行测试

测试套件 SHALL 能在无图形界面下通过 Godot 命令行运行，以便未来接入 CI。

#### Scenario: 命令行运行全部测试
- **WHEN** 执行 Godot headless 命令并指向 GUT 命令行运行器与 `tests/unit`
- **THEN** 全部测试 MUST 被收集并执行，退出码反映通过/失败

