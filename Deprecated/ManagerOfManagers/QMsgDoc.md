>作者：vin129     邮箱：515019721@qq.com

# QMsg

# 继承关系

实现接口：IPoolable、IPoolType

# 描述

> UIKit 中消息传递的基础类型
>
> 可被池管理

# 公共属性

|               |                          |
| ------------- | ------------------------ |
| **EventID**   | 自定义的事件Id           |
| **Processed** | Processed or not         |
| **ReuseAble** | reusable or not          |
| **ManagerID** | 对应Manager的Id          |
| **msgId**     | get EventID  set EventID |

# 静态方法

|              |                                |
| ------------ | ------------------------------ |
| **Allocate** | 获得**QMsg**类型实例，被池管理 |

# 构造方法

|                       |                          |
| --------------------- | ------------------------ |
| **QMsg**              |                          |
| **QMsg(int eventID)** | 在 **Allocate** 中被使用 |

# 公共方法

|              |                  |
| ------------ | ---------------- |
| **GetMgrID** | return ManagerID |

