## Why

AudioKit 的 Sound 播放（`PlaySound`）在 v1.0.187 重构时，把原来的 `root.AddComponent<AudioSource>()` 改成了 `new GameObject(name).AddComponent<AudioSource>()`，每个音效创建一个独立 GameObject。但回收逻辑没有同步更新：

- Pool 未满时：AudioPlayer 入池复用，但 AudioSource GO 没有隐藏/销毁，残留在 Hierarchy 中
- Pool 已满时：AudioPlayer 丢弃，但 AudioSource GO 被隐藏却永远不会复用（泄漏）

导致场景中播放音效越多，Hierarchy 中堆积的空 GameObject 越多。

## What Changes

修改 `AudioPlayer.Recycle2Cache()` 的回收逻辑：

- Pool 未满（入池成功）：调用 `OnParentRecycled()` 隐藏 GO，挂到 AudioManager 下等待复用
- Pool 已满（入池失败）：直接 `Destroy` GO，彻底清理

改动范围仅 `AudioPlayer.cs` 一个方法，不新增 API，不改公开接口。

## Capabilities

### Modified Capabilities

- `AudioKit Sound 播放`: 回收行为修复，不再产生 GO 泄漏

## Impact

- **修改**: `Assets/QFramework/Toolkits/AudioKit/Scripts/Player/AudioPlayer.cs` 的 `Recycle2Cache()` 方法
- **不动**: AudioKit 公开 API、PlaySoundCommand、FluentSoundAPI、AudioSourceProxy、AudioManager
- **范围**: 仅影响编辑器和运行时的音效回收路径，无新增依赖
