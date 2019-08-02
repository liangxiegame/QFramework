>作者：布鞋     邮箱：827922094@qq.com

## DelayAction
class in ActionKit / Inherits from: [NodeAction ](ActionKitAPI/Action/NodeAction.md)  /Implemented in : [IPoolable]()

## Description
延迟节点，用于异步等待一段时间执行事件。

参考:```IActionChainExtention.Delay```

## Properties
- DelayTime  		    节点延迟总时间
- IsRecycled        缓存标记

## Construction

* public DelayAction(float delayTime)	

| 参数      | 描述     |
| --------- | -------- |
| delayTime | 延迟时间 |

## Inherits Methods

* Finish			      结束当前节点，

* Break             设置节点的状态为Finish

* Reset             重置节点状态

* public bool Execute(float dt)     执行当前节点，需要传入执行一次的时间，返回是否执行结束
  
  
  |参数|描述|
  |:--:|:----:|
  |dt|执行一次的时间|

* Dispose                          设置节点的状态为Dispose

## Static Methods

- public static DelayAction Allocate(float delayTime, System.Action onEndCallback = null)	 从缓存池中创建延迟节点，并设置延迟时间和结束事件


  | 参数          | 描述     |
  | ------------- | -------- |
  | delayTime     | 延迟时间 |
  | onEndCallback | 结束回调 |

## Messages

- OnRecycled                    缓存池回调

```
var delay2s = DelayAction.Allocate(2.0f, () => { Log.I("延时 2s"); });
this.ExecuteNode(delay2s);  /// 这里的this. 由IActionChainExtention扩展
```

