>作者：vin129     邮箱：515019721@qq.com

# QMsgCenter

# 继承关系

继承自: MonoBehaviour

实现接口：ISingleton

# 描述

> 单例
>
> UIKit 消息中心
>
> 转发来自各个Manager的消息至对应Manager


# 静态属性

|              |      |
| ------------ | ---- |
| **Instance** | 单例 |

# 构造方法

|                       |                          |
| --------------------- | ------------------------ |
| **QMsg**              |                          |
| **QMsg(int eventID)** | 在 **Allocate** 中被使用 |

# 公共方法

|                     |                                         |
| ------------------- | --------------------------------------- |
| **OnSingletonInit** | 单例Init方法                            |
| **Dispose**         | 单例Dispose方法                         |
| **SendMsg**         | 根据**Qmsg.ManagerID** 发送对应**Qmsg** |

