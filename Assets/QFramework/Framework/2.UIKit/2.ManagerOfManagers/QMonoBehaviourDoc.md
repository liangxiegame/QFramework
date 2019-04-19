>作者：vin129     邮箱：515019721@qq.com

# QMonoBehaviour

# 继承关系
继承自: MonoBehaviour

# 描述
> UIKit 中使用脚本的基类
>
> 继承自MonoBehaviour，对MonoBehaviour中部分方法进行了重载。
>
> 提供了一套事件系统。

# 私有属性
| **mReceiveMsgOnlyObjActive**  |  接收事件的条件开关   |
| ---------------------------- | ------------------------ |
| **mPrivateEventIds**			| 事件id列表           |
| **mCachedEventIds**           | get mPrivateEventIds |

# 抽象&虚方法

| **ProcessMsg**      | 处理消息的方法                              |
| ------------------- | ------------------------------------------- |
| **Manager**         | 获取Manager                                 |
| **OnShow**          | MonoBehaviour 执行**Show**方法后被执行      |
| **OnHide**          | MonoBehaviour 执行**Hide**方法后被执行      |
| **OnBeforeDestroy** | MonoBehaviour 执行**OnDestroy**方法后被执行 |
| **SendMsg**         | 发送 **QMsg** 类型消息                      |
| **SendEvent**       | 发送 **IConvertible** 类型事件Id            |



# 私有方法

| **RegisterEvent**      | 通过 **IConvertible EventId** 注册事件   |
| ---------------------- | ---------------------------------------- |
| **RegisterEvents**     | 注册事件组 IConvertible[]                |
| **UnRegisterEvent**    | 通过 **IConvertible EventId** 注销事件   |
| **UnRegisterAllEvent** | 注销 **mPrivateEventIds** 中所有监听事件 |



# 公共方法

|             |                                                              |
| ----------- | ------------------------------------------------------------ |
| **Process** | 接收消息Id，生成 **Qmsg** ，执行**ProcessMsg**，回收**Qmsg** |

