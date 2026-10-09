## 1. 代码修改

- [x] 1.1 `IMGUIGraphUtilities.cs`：`DoCreateCodeFile` 改为 `#if UNITY_6000_4_OR_NEWER` 整类双版本（新分支继承 `AssetCreationEndAction`，override `Action(EntityId, string, string)`），`CreateFromTemplate` 在新分支传 `EntityId.None`。验证：新旧两个分支的类体除基类/参数类型外逐行一致，旧分支与改前代码逐字节相同（git diff 确认改动只落在 `#if` 新增块内）
- [x] 1.2 `IMGUIGraphWindow.cs`：`OnOpen` 方法体内加 `#if UNITY_6000_4_OR_NEWER` 分支，用 `EditorUtility.EntityIdToObject((EntityId)instanceID)` 替代 `InstanceIDToObject`，签名不动。验证：git diff 确认旧分支原样保留

## 2. 编译与验证

- [x] 2.1 本仓库（Unity 2018.4）重新编译通过，无新增警告/错误（旧分支未变，此步守护回归）。验证记录：git diff 确认旧分支逐字节未动；`mcs --parse` 对新旧两个预处理分支均语法通过；运行中的 2018.4.36f1 编辑器于 10:44:09（改动之后）完成 `Assembly-CSharp-Editor.dll` 重编译，Editor.log 零 error、唯一警告为无关文件的既有 CS0162
- [ ] 2.2 （用户侧，非阻塞）在 Unity 6.4+ 项目中导入 QFramework：编译无 CS0619；实测 `Assets/Create/@GraphKit` 两个菜单创建脚本、双击 .asset 打开窗口两条流程
