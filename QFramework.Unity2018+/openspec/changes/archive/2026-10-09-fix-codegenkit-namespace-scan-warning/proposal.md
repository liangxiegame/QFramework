## Why

v1.0.257 引入的架构代码生成器在 PackageKit 窗口每次初始化时无条件调用 `AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/Scripts" })`。用户项目没有 `Assets/Scripts` 目录时（大多数项目的初始状态），Unity 会打 `Folder not found: 'Assets/Scripts'` 警告；且由于默认值参数立即求值，即使用户已保存过命名空间偏好，扫描与警告仍每次触发。功能本身不受影响（空扫描走 productName 兜底），属于纯日志噪音的体验回归。

## What Changes

- `ArchitectureCodeGeneratorView.GetProjectNamespace()`：`FindAssets` 前增加 `AssetDatabase.IsValidFolder("Assets/Scripts")` 判断，目录不存在时跳过扫描（命名空间列表为空，走既有兜底逻辑）。
- `ArchitectureCodeGeneratorView.Init()`：命名空间偏好的默认值改为惰性求值——仅当 EditorPrefs 无存值时才调用 `GetProjectNamespace()`，消除已保存偏好场景下的无谓扫描。

## Capabilities

### New Capabilities
- `codegenkit-architecture-namespace`: 架构代码生成器的默认命名空间解析行为——从既有脚本推断默认命名空间、productName 兜底、已保存偏好优先，以及扫描前置条件的约束。

### Modified Capabilities

（无 —— `openspec/specs/` 尚无既有 spec。）

## Impact

- 受影响代码：`Assets/QFramework/Toolkits/_CoreKit/CodeGenKit/Editor/Architecture/ArchitectureCodeGeneratorView.cs`（2 处，均在编辑器代码内）。全库扫描确认只有这一处对可能不存在的硬编码目录做 `FindAssets`。
- 返回值行为完全等价：`HasKey ? GetString(key) : GetProjectNamespace()` 与原 `GetString(key, GetProjectNamespace())` 在所有输入下返回相同结果，仅去除无存值/已存值两种场景下的副作用（警告与扫描）。
- 运行时程序集零影响。
