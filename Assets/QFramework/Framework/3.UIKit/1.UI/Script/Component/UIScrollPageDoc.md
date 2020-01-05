>作者：vin129     邮箱：515019721@qq.com

# UIScrollPage

# 继承关系

继承自：MonoBehaviour

实现接口：IBeginDragHandler, IEndDragHandler

# 描述

> 切页组件
>
> 与 UIScrollPageMark 可共同使用 

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

|                    |          |
| ------------------ | -------- |
| float **smooting** | 滑动速度 |

# 公共方法

|                              |                                           |
| ---------------------------- | ----------------------------------------- |
| **AddPageChangeListener**    | 注册页面变化回调事件                      |
| **RemovePageChangeListener** | 移除页面变化回调事件                      |
| **GetCurrentPageIndex**      | 获取当前页面索引                          |
| **GetPageCount**             | 获取页面数量（根据ScrollRect 中子物体数） |
| **SetPage**                  | 设置当前页面                              |

# 私有方法

|                 |                        |
| --------------- | ---------------------- |
| **UpdatePages** | **Update** 中 更新界面 |

