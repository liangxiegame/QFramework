>作者：布鞋     邮箱：827922094@qq.com

## OnlyBeginAction
class in Actionkit / Inherits from:[NodeAction](ActionKitAPI/Action/NodeAction.md) / Implemented from:[IPoolable](www.baidu.com),[IPoolType](www.baidu.com)

## Description
可控事件节点，和EventAction节点类似，但是通过回调可以控制节点结束时机。

## Implemented Properties
* IsRecycled   缓存标记

## Inherits Methods

* Finish			      结束当前节点，

* Break             设置节点的状态为Finish

* Reset             重置节点状态

* public bool Execute(float dt)     执行当前节点，需要传入执行一次的时间，返回是否执行结束


  | 参数 | 描述           |
  | ---- | -------------- |
  | dt   | 执行一次的时间 |

* Dispose                          设置节点的状态为Dispose

## Public Methods
* public static OnlyBeginAction Allocate(```Action<OnlyBeginAction> beginAction```)  


| 参数        | 描述     |
| ----------- | -------- |
| beginAction | 执行回调 |

## Messages
* OnRecycled  回收到缓存池时调用

```
   this.Sequence()
	   .Delay(1.0f)
	   .OnlyBegin(action =>
	   {
           this.transform.DOLocalMove(new Vector3(-5, -5), 0.5f).OnComplete(() => { action.Finish(); });
	   })
	   .Event(()=> Debug.Log("动画结束")
	   .Begin();
```
