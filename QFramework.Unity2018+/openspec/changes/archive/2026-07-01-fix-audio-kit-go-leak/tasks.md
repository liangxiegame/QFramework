## 1. 修复 Recycle2Cache 回收逻辑

- [x] 1.1 修改 `AudioPlayer.Recycle2Cache()`：Pool 未满时调用 `OnParentRecycled()` 隐藏 GO
- [x] 1.2 修改 `AudioPlayer.Recycle2Cache()`：Pool 已满时 `Destroy` AudioSource GO
- [x] 1.3 确认 `AudioSourceProxy.AudioSourceIsNull()` 和 `AudioSourceProxy.AudioSource` 可正常访问

## 2. 验证

- [x] 2.1 Unity 编辑器中播放多个音效，确认 Hierarchy 中不再堆积空 GO
- [x] 2.2 确认音效播放功能不受影响（正常播放、停止、循环）
- [x] 2.3 确认 Music 和 Voice 播放不受影响
