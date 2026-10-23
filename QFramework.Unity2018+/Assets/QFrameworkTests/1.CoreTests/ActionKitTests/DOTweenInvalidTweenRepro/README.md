# ActionKit / DOTween 已失效 Tween 警告复现与修复验证

本目录最初用真实 DOTween 和 ActionKit 执行链复现以下警告。当前测试副本已在用户删除重复 getter 的基础上修正状态判断，用于验证正常完成且不再出现此警告；原始示例包未修改。

```text
DOTWEEN ► This Tween has been killed and is now invalid
```

## 依赖

- Unity 2018.4.36f1（本项目版本）。
- 项目现有的 QFramework ActionKit。
- [DOTween 1.2.790 官方下载](https://dotween.demigiant.com/downloads/DOTween_1_2_790.zip)。解压到 `Assets/Plugins/Demigiant/DOTween`；第三方文件不包含在此测试目录中，其他 checkout 需要单独安装。
- 打开 `Tools > Demigiant > DOTween Utility Panel`，执行 `Setup DOTween`，保持 UI 模块启用；确认当前平台的 Scripting Define Symbols 包含 `DOTWEEN`，等待编译完成。

测试组件始终可编译；未安装 DOTween / 未启用 `DOTWEEN` 时，进入 Play 只会提示安装和 Setup，不会假造复现结果。

## 运行

1. 打开本目录的 `DOTweenInvalidTweenRepro.unity`。
2. 打开 Console，启用 Warning 显示，然后进入 Play。
3. 场景自动创建一个水平填充的 uGUI Image，以 `Ease.OutCubic` 在 5 秒内从 0 填充到 1。
4. 开始时，Console 应显示 getter 被调用一次。
5. 约 5 秒后，动作正常完成，不应出现上面的 invalid tween warning。
6. 随后的完成日志应显示 `CreationCount=1, WarningCount=0, FillAmount=1`。根物体组件也保留三个序列化统计字段。

`WarningCount` 仅监听这次运行期间实际收到的匹配 warning，不会吞掉或生成该警告。场景只运行这一项测试，避免其他 Tween 的同名警告影响计数。

若需要重新生成场景，使用 `QFramework > Tests > Create DOTween Invalid Tween Repro Scene`。命令行生成入口为：

```text
-executeMethod QFramework.Tests.DOTweenInvalidTween.Editor.DOTweenInvalidTweenReproSceneBuilder.CreateSceneBatch
```

## 原始缺陷与当前修正

`DOTweenAction.cs` 来自 `Assets/QFramework/Toolkits/_CoreKit/ActionKit/Example.unitypackage` 内嵌的 `10.DOTween(Need DOTween Installed-需要先装好 DOTween).unitypackage`。原始缺陷是：

- `OnStart()` 调用两次 `mTweenGetter()`，创建两个 Tween，但仅跟踪第一个。
- `OnExecute()` 仅判断引用不为 null，随后对已经自动 Kill 的 Tween 调用 `IsPlaying()`，因此触发实库 warning。

当前副本保留用户对 `OnStart()` 的修改，只调用一次 getter。`OnExecute()` 改用 `mExecutingTween == null || !mExecutingTween.IsActive() || mExecutingTween.IsComplete()`：通过 `||` 短路，已失效时不再查询完成状态；仍活跃但暂停的 Tween 也不会被误判为完成。`Deinit()` 仅对仍活跃的 Tween 调用 `Kill()`，不再用 `IsPlaying()` 检查无效引用。

测试没有修复原始示例包。它显式设置 `SetAutoKill(true)` 和 `SetRecyclable(false)`，保证结束后能稳定观察无效引用，并安全清理自己创建的 Tween。日志级别只在测试期间临时设为 Default，完成或销毁后恢复；不会调用 `KillAll` / `Clear`。

测试入口的只读属性 `DependencyAvailable`、`Completed`、`CreationCount`、`WarningCount`、`FillAmount` 可供外部验证脚本读取。

## 验证记录

2026-10-23，原始缺陷副本在 Unity 2018.4.36f1 + DOTween 1.2.790 的真实 Play 模式中复现成功：getter 调用 2 次，DOTween 实际输出 1 次 invalid tween 警告，最终 `Completed=True, CreationCount=2, WarningCount=1, FillAmount=1`。本次机器已安装本地 DOTween 依赖；其他 checkout 仍需按上面的依赖步骤安装。

当前修正版本仅完成局部 C# 状态回放验证：getter 调用 1 次，已 Kill 的 Tween 无警告并正常结束，暂停时保持等待，仍活跃且完成时正常结束；对已 Kill 或暂停的 Tween 执行 `Deinit()` 均无警告。修正后尚未重新执行 Unity Play 验证，上面的原始复现记录不代表当前修正版本的实机结果。
