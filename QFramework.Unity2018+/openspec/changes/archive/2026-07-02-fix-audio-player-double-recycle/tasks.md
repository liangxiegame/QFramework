## 1. 修复 Recycle2Cache 重复回收问题

- [x] 1.1 在 `AudioPlayer.Recycle2Cache()` 开头增加 `if (IsRecycled) return;` 守卫

## 2. 验证

- [x] 2.1 调用 `AudioKit.StopAllSound()` 后，正在播放的音效 GO 被 Hide 而非 Destroy
- [x] 2.2 确认音效播放功能不受影响（正常播放、停止、循环）
- [x] 2.3 确认 Music 和 Voice 播放不受影响
