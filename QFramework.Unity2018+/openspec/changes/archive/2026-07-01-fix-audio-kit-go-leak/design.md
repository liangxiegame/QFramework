## Context

v1.0.187 重构 AudioKit 时，把 AudioSource 从 `AddComponent` 改为 `new GameObject`，但 `Recycle2Cache()` 的回收路径没有适配：

```csharp
// 当前代码（有问题）
public void Recycle2Cache()
{
    if (!SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
    {
        AudioSourceProxy.OnParentRecycled();
    }
    // Pool 未满时 → GO 残留
    // Pool 已满时 → GO 隐藏但永不复用（泄漏）
}
```

## Design

修改 `Recycle2Cache()` 为两个分支：

```csharp
public void Recycle2Cache()
{
    if (SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
    {
        // 入池成功 → 隐藏 GO 等待复用
        AudioSourceProxy.OnParentRecycled();
    }
    else
    {
        // Pool 满 → 销毁 GO
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
| Pool 未满 | GO 残留在 Hierarchy | GO 隐藏，挂到 AudioManager 等复用 |
| Pool 已满 | GO 隐藏但永不复用 | GO Destroy，彻底清理 |

### 不改动的部分

- `AudioSourceProxy.OnParentRecycled()` — 已有逻辑，隐藏 GO 并挂到 AudioManager
- `SafeObjectPool.Recycle()` — 池子逻辑不变
- AudioKit 公开 API — 无变化
