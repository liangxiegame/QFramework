## Context

v1.0.187 重构 AudioKit 后，`AudioSourceProxy` 使用 `ActionKit.Condition` 监听播放完成：

```csharp
mAction = ActionKit.Condition(() => !Paused && !AudioSource.isPlaying, () =>
{
    mOnSoundPlayFinish?.Invoke();
    ...
});
```

当 `Stop()` 被调用时，`StopAndClearClip()` 设 `Paused = false` 并停止 AudioSource，条件满足，下一帧触发 `OnPlayFinished()` → `Stop()` → `Recycle2Cache()`。

此时 AudioPlayer 已经被回收（`IsRecycled = true`），`SafeObjectPool.Recycle()` 返回 `false`，进入销毁分支。

## Design

在 `Recycle2Cache()` 开头加守卫：

```csharp
public void Recycle2Cache()
{
    if (IsRecycled) return;  // ← 防止重复回收

    if (SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
    {
        AudioSourceProxy.OnParentRecycled();
    }
    else
    {
        if (!AudioSourceProxy.AudioSourceIsNull())
        {
            GameObject.Destroy(AudioSourceProxy.AudioSource.gameObject);
        }
    }
}
```

### 行为对照

| 场景 | 修改前 | 修改后 |
|------|--------|--------|
| 第一次回收 | 入池成功，Hide GO | 入池成功，Hide GO |
| 第二次回收（OnPlayFinished 触发） | IsRecycled=true → Destroy GO | IsRecycled=true → return，GO 保持 Hide 状态 |

### 不改动的部分

- `AudioSourceProxy` — ActionKit.Condition 回调逻辑不变
- `SafeObjectPool.Recycle()` — 池子逻辑不变
- AudioKit 公开 API — 无变化
