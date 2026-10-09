## 1. 代码修改

- [x] 1.1 `ArchitectureCodeGeneratorView.cs`：`GetProjectNamespace()` 在 `FindAssets` 前加 `AssetDatabase.IsValidFolder("Assets/Scripts")` 判断，不存在则跳过扫描。验证：git diff 确认推断/兜底逻辑（`ResolveDefaultNamespace` 调用）未动，仅加目录判断
- [x] 1.2 `ArchitectureCodeGeneratorView.cs`：`Init()` 命名空间读取改为 `EditorPrefs.HasKey(key) ? EditorPrefs.GetString(key) : GetProjectNamespace()`。验证：git diff 确认其余偏好读取（mOutputRoot、mGenerateInterface、mState）原样保留

## 2. 编译与验证

- [x] 2.1 `mcs --parse -d:UNITY_EDITOR` 语法通过；运行中的 2018.4 编辑器重编译无 error/新警告（Editor.log 第二次重编译完成于修复之后，0 error）
- [x] 2.2 （用户侧）用户确认"好了"；Editor.log 中修复后的程序集重载起再无 `Folder not found: 'Assets/Scripts'`
