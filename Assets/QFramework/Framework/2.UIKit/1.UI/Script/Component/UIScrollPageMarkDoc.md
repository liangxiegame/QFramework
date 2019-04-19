>作者：vin129     邮箱：515019721@qq.com

# UIScrollPageMark

# 继承关系

继承自：MonoBehaviour

# 描述

> 切页Toggle组件
>
> 与 UIScrollPage 共同使用 

# 私有属性

|                                             |                                                  |
| ------------------------------------------- | ------------------------------------------------ |
| ScrollRect **rect**                         | 物体 ScrollRect 组件                             |
| List<float>  **pages**                      | 每页占的比列：0/3=0  1/3=0.333  2/3=0.6666 3/3=1 |
| int **currentPageIndex**                    | 当前页索引                                       |
| float **targethorizontal**                  | 滑动的起始坐标                                   |
| bool **isDrag**                             | 是否拖拽结束                                     |
| UIScrollPageChangeEvent  **mOnPageChanged** | 页面变动时的回调函数                             |

# 公共属性

|                              |                          |
| ---------------------------- | ------------------------ |
| UIScrollPage **scrollPage**  | 切页组件                 |
| ToggleGroup **toggleGroup**  | ToggleGroup              |
| Toggle **togglePrefab**      | 用于Clone 的 toggle 预设 |
| Vector2  **centerPos**       | 页签中心位置             |
| Vector2  **interval**        | 每个页签之间的间距       |
| List<Toggle>  **toggleList** | toggle 组件列表          |

# 公共方法

|                         |                                                              |
| ----------------------- | ------------------------------------------------------------ |
| **OnScrollPageChanged** | **Awake** 时注册进**UIScrollPage**中的回调事件，用于改变**toggleList**的状态 |

# 私有方法

|                     |                  |
| ------------------- | ---------------- |
| **AdjustTogglePos** | 更新 Toggle 位置 |

