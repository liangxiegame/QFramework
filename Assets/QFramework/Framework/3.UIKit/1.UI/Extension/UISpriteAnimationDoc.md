>作者：vin129     邮箱：515019721@qq.com

# UISpriteAnimation

# 继承关系

继承自：MonoBehaviour

# 描述

> 挂载组件
>
> 帧动画组件

# 公共属性

|                  |                             |
| ---------------- | --------------------------- |
| **SpriteFrames** | Sprite List                 |
| **IsPlaying**    | 是否正在播放                |
| **Forward**      | 前进 or 倒退                |
| **AutoPlay**     | 是否自动播放                |
| **Loop**         | 是否循环                    |
| **FrameCount**   | 帧数 **SpriteFrames.Count** |
| **FPS**          | 帧率                        |

# 私有属性

|                  |                              |
| ---------------- | ---------------------------- |
| **mImageSource** | **Image** 组件               |
| **mCurFrame**    | 当前帧                       |
| **mDelta**       | 检测是否达到播放下一帧的时间 |

# 私有方法

|               |            |
| ------------- | ---------- |
| **SetSprite** | 设置帧图片 |

# 公共方法

|                 |                  |
| --------------- | ---------------- |
| **Play**        | 播放             |
| **PlayReverse** | 倒播             |
| **Pause**       | 暂停             |
| **Resume**      | 继续             |
| **Stop**        | 停止，回到第一帧 |
| **Rewind**      | 从头播放         |

