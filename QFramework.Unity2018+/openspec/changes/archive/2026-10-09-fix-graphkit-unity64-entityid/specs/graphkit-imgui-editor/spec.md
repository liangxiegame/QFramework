## Purpose

GraphKit IMGUI 与 Unity 编辑器（Project 窗口）集成的行为契约：通过菜单从模板创建 Graph 脚本、双击打开 Graph 资产，并保证这些编辑器功能在所有受支持的 Unity 版本（2018.4 至 6.4+）上可用。

## ADDED Requirements

### Requirement: 从 Project 窗口创建 GraphKit 脚本

用户通过 `Assets/Create/@GraphKit` 下的菜单项（IMGUI Graph C# Script / IMGUI Graph Node C# Script）创建脚本时，系统 SHALL 进入 Unity 标准的资源命名编辑流程，确认后基于对应模板生成 `.cs` 文件（`#SCRIPTNAME#` 占位符替换为用户输入的类名），并在 Project 窗口中选中新资产。模板文件缺失时 SHALL 输出警告且不产生异常。

#### Scenario: 在受支持的 Unity 版本上创建脚本
- **WHEN** 用户在任意受支持的 Unity 版本（2018.4 至 6.4+）中点击 `Assets/Create/@GraphKit` 菜单项并确认命名
- **THEN** 生成占位符已替换的 `.cs` 文件并被选中，流程无异常

#### Scenario: 模板缺失
- **WHEN** 对应模板文件不在 AssetDatabase 中
- **THEN** 输出警告日志，不进入命名编辑流程，无异常抛出

### Requirement: 双击打开 Graph 资产

用户在 Project 窗口双击 Graph 资产时，系统 SHALL 打开 GraphKit 图形编辑窗口并加载该资产。非 Graph 资产不受影响（走 Unity 默认行为）。

#### Scenario: 双击 Graph 资产
- **WHEN** 用户双击一个 Graph 资产
- **THEN** 打开图形编辑窗口并加载该资产内容

### Requirement: 编辑器代码跨受支持 Unity 版本可编译

GraphKit 编辑器代码 SHALL 在所有受支持的 Unity 版本上无 error 级弃用（obsolete）编译错误；对 Unity 6000.4 之前版本，上述编辑器行为 SHALL 与修复前完全一致（零行为变化）。

#### Scenario: Unity 6.4+ 项目编译
- **WHEN** 项目使用 Unity 6000.4 或更高版本编译
- **THEN** GraphKit 编辑器代码编译通过，不出现 `EndNameEditAction`/`InstanceIDToObject` 相关的 CS0619 弃用错误

#### Scenario: 旧版本行为保持不变
- **WHEN** 项目使用 Unity 2018.4 至 6000.3 之间任一版本
- **THEN** 创建脚本与双击打开资产的行为与修复前一致
