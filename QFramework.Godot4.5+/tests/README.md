# 单元测试

本项目使用 [GUT（Godot Unit Test）](https://github.com/bitwes/Gut) **v9.5.0** 进行单元测试，目标引擎 **Godot 4.5**。

> **版本说明**：GUT v9.6.0+ 的 `gut_plugin.gd` 使用了 `EditorDock` / `add_dock()` 等 Godot **4.6+** 才有的编辑器 API，在 4.5 下编辑器加载会报解析错误并禁用插件。v9.5.0 是兼容 Godot 4.5 的最新版本——命令行 `gut_cmdln.gd` 不受影响，但编辑器面板需要 4.5 兼容版。**升级到 Godot 4.6+ 后可换用 GUT 最新版。**

## 目录结构

- `tests/unit/` —— 单元测试脚本（`extends GutTest`）
- `.gutconfig.json` —— GUT 命令行运行配置
- `addons/gut/` —— GUT 插件本体

## 运行测试

### 命令行（headless，可接入 CI）

macOS 下若 `godot` 未进 PATH，用 `.app` 内的可执行文件：

```bash
/Applications/Godot_v4.5.2.app/Contents/MacOS/Godot --headless \
  -s res://addons/gut/gut_cmdln.gd \
  -gdir=res://tests/unit -gexit
```

若已把 `godot` 加入 PATH：

```bash
godot --headless -s res://addons/gut/gut_cmdln.gd -gdir=res://tests/unit -gexit
```

退出码非 0 表示有测试失败，便于 CI 判定。

### 编辑器内

启用 GUT 插件后，编辑器底部出现 GUT 面板，点 Run 即可。

面板的测试目录配置存储在 `user://gut_temp_directory/gut_editor_config.json`（即 `~/Library/Application Support/Godot/app_userdata/QFramework.GDScript/gut_temp_directory/`，**不在项目 git 内**，每台机器独立）。首次使用若面板提示 `no directories set`：

- 在面板 **Settings** 标签里把 `res://tests/unit` 加到 Directories，或
- 用面板的"加载 `.gutconfig.json`"功能导入项目根目录的 `.gutconfig.json`，或
- 重启编辑器（本仓库已预置上述 `gut_editor_config.json`）。

> 命令行（`.gutconfig.json`）与编辑器面板（`gut_editor_config.json`）是两套独立配置，需分别设置。

## 编写约定

- 测试脚本置于 `tests/unit/`，`extends GutTest`，测试函数以 `test_` 开头。
- **GDScript lambda 对 `int` 按值捕获**：需要在回调内累加的计数器必须用数组引用语义，例如 `var count = [0]`，回调内 `count[0] += 1`；直接用 `var count = 0` 会在 lambda 内改到副本，外部读不到变化。
- 涉及对象生命周期的断言，用 `weakref(obj).get_ref()` 配合 `is_instance_valid(obj)` 观察是否被回收。

## 现有覆盖

| 文件 | 覆盖 |
|------|------|
| `test_easy_event.gd` | 基础 Event 注册/触发/带参触发/注销 + 注销幂等 |
| `test_or_event.gd` | OrEvent 功能（or 语义）/ 注销正确性 / **幂等闪退回归** / **生命周期（不依赖手动引用计数）** |
