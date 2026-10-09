## Why

Unity 6.4 (6000.4+) 把 `ProjectWindowCallback.EndNameEditAction`、`StartNameEditingIfProjectWindowExists(int, ...)` 和 `EditorUtility.InstanceIDToObject(int)` 标记为弃用，其中 `EndNameEditAction` 在实际发布的 6000.4 补丁版本中为 error 级（CS0619），导致 GraphKit IMGUI 的编辑器代码在 Unity 6.4+ 项目里编译失败（用户实测上报）。QFramework 承诺支持 2018.4+，必须同时保住新旧两端。

## What Changes

- `IMGUIGraphUtilities.cs`：`DoCreateCodeFile` 在 `UNITY_6000_4_OR_NEWER` 下改继承 `AssetCreationEndAction` 并 override `Action(EntityId, ...)`；`CreateFromTemplate` 传 `EntityId.None` 替代 `0`。旧版本分支代码保持原样。
- `IMGUIGraphWindow.cs`：`OnOpen` 在 `UNITY_6000_4_OR_NEWER` 下改用 `EditorUtility.EntityIdToObject((EntityId)instanceID)`，方法签名保持 `int` 不变。
- `ShowCreatedAsset` 未被弃用，不动；不迁移无关 API。

## Capabilities

### New Capabilities
- `graphkit-imgui-editor`: GraphKit IMGUI 编辑器集成（Project 窗口创建脚本菜单、双击打开 Graph 资产）在所有受支持的 Unity 版本（2018.4 至 6.4+）上的行为契约。

### Modified Capabilities

（无 —— `openspec/specs/` 尚不存在，本 change 首建该能力。）

## Impact

- 受影响代码：`Assets/QFramework/Toolkits/_CoreKit/GraphKit/IMGUI/Scripts/Editor/IMGUIGraphUtilities.cs`（2 处）、`IMGUIGraphWindow.cs`（1 处）。全仓库扫描确认无其他文件使用受弃用影响的 API。
- 运行时程序集零影响：三处改动均在 `#if UNITY_EDITOR` 代码内，且旧版本编译路径与现状逐字节一致。
- 验证约束：本仓库 Unity 为 2018.4，`UNITY_6000_4_OR_NEWER` 分支本地无法编译验证，需在 Unity 6.4+ 项目实测（由维护者手动完成）。
