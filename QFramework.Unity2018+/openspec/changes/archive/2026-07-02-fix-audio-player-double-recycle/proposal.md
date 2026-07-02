## Why

`AudioKit.StopAllSound()` 调用后，正在播放的音效所在的 GameObject 被销毁而不是隐藏。

**根因**：`Stop()` 触发 `Recycle2Cache()` 成功回收后，`AudioSourceProxy` 里的 `ActionKit.Condition` 回调在下一帧仍然会触发 `OnPlayFinished()` → `Stop()` → `Recycle2Cache()` 第二次。此时 `IsRecycled` 已为 `true`，`SafeObjectPool.Recycle()` 返回 `false`，进入销毁分支。

```
第一次 Stop() → Recycle2Cache() → 入池成功 → Hide GO
第二帧 OnPlayFinished → Stop() → Recycle2Cache() → IsRecycled=true → Destroy GO 💥
```

## What Changes

在 `AudioPlayer.Recycle2Cache()` 开头增加 `IsRecycled` 守卫，防止重复回收。

## Capabilities

### Modified Capabilities

- `AudioKit Sound 播放`: 修复 `StopAllSound()` 后 GO 被销毁的问题

## Impact

- **修改**: `Assets/QFramework/Toolkits/AudioKit/Scripts/Player/AudioPlayer.cs` 的 `Recycle2Cache()` 方法
- **不动**: AudioKit 公开 API、AudioSourceProxy、AudioManager、SafeObjectPool
- **范围**: 仅影响音效回收路径，无新增依赖
