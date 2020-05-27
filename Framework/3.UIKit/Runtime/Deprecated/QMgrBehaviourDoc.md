>作者：vin129     邮箱：515019721@qq.com

# QMgrBehaviour

# 继承关系

继承自: QMonoBehaviour

实现接口：IManager

# 描述

> UIKit  中创建Manager脚本的基类
>
> 继承自QMonoBehaviour，提供了一套事件系统的管理模式
>

# 私有属性

|                  |                |
| ---------------- | -------------- |
| **mEventSystem** | 独自的事件系统 |

# 公共属性

|               |                          |
| ------------- | ------------------------ |
| **ManagerId** | 独自的ManagerId          |
| Manager       | 返回**IManager**类型自身 |

# 公共方法

|                   |                                                              |
| ----------------- | ------------------------------------------------------------ |
| **RegisterEvent** | 向自身**QEventSystem**注册事件                               |
| **UnRegistEvent** | 向自身**QEventSystem**注销事件                               |
| **SendMsg**       | 根据**QMsg**中  **ManagerID** 执行消息或转发至 **QMsgCenter** |
| **SendEvent**     | 将 **IConvertible** 包装成 **QMsg**类型并执行 **SendMsg**    |

# 私有方法

|                     |                       |
| ------------------- | --------------------- |
| **ProcessMsg**      | 执行消息              |
| **OnBeforeDestroy** | 回收 **mEventSystem** |

