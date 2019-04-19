>作者：vin129     邮箱：515019721@qq.com

# UIElement

# 继承关系

继承自：MonoBehaviour

实现接口：IMark

# 描述

> Abstract Class
>
> 可被UICodeGenerator 处理
>
> UI元素类脚本基类

# 抽象&重载方法

|                   |                           |
| ----------------- | ------------------------- |
| **GetUIMarkType** | return UIMarkType.Element |
| **ComponentName** | 组件命名                  |

# 私有属性

|                   |                                    |
| ----------------- | ---------------------------------- |
| **mCustomComent** | UICodeGenerator 生成脚本文件的注释 |

# 私有方法

|               |            |
| ------------- | ---------- |
| **SetSprite** | 设置帧图片 |

# 公共方法

|               |                           |
| ------------- | ------------------------- |
| **Comment**   | return mCustomComent      |
| **Transform** | return transform          |
| **Manager**   | return UIManager.Instance |

