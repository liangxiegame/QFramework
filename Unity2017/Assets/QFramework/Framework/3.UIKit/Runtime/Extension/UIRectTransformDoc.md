>作者：vin129     邮箱：515019721@qq.com

# UIRectTransformExtension

# 描述

> 静态类
>
> 针对 RectTransform 提供静态方法拓展

# 静态方法

|                                     |                                                              |
| ----------------------------------- | ------------------------------------------------------------ |
| **GetLocalPosInRect**               | 屏幕点坐标（**mousePosition**）转该UI（**this RectTransform**）的本地坐标 |
| **InRect**                          | 屏幕点坐标（**mousePosition**）是否在该UI（**this RectTransform**）中 |
| **ToScreenPoint**                   | 世界坐标（**this RectTransform.position**） 转 屏幕坐标      |
| **ToScreenPoint**                   | 坐标（**Vector2**） 转 屏幕坐标                              |
| **InRootTransRect**                 | **this RectTransform** 是否在  **RectTransform rootTrans** 内 |
| **ConvertWorldPosToLocalPosInSelf** | **worldPos** 转该UI（**this RectTransform**）的本地坐标      |

