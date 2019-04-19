>作者：布鞋     邮箱：827922094@qq.com

## DOTweenAction
class in NodeAction/ Inherits from:[NodeAction](ActionKitAPI/Action/NodeAction.md)  / /Implemented in : [IPoolable](https://github.com/827922094/Action-Kit-API/blob/master/www.baidu.com),[IPoolType]()

## Description
```DoTween```扩展节点, 用于执行Dotween的动作，动作OnComplete回调时节点结束。

## Properties

- IsRecycled                      缓存标记

## Construction

* public SequenceNodeChain()	默认构造函数，会对自身存储的序列节点初始化。

## Inherits Methods

* Finish			      结束当前节点，

* Break                              设置节点的状态为Finish

* Reset                              重置节点状态

* public bool Execute(float dt)     执行当前节点，需要传入执行一次的时间，返回是否执行结束


  | 参数 | 描述           |
  | ---- | -------------- |
  | dt   | 执行一次的时间 |

* Dispose                          设置节点的状态为Dispose

## Public Methods

*  public static DOTweenAction Allocate(Func<Tweener> tweenFactory) 从缓存池中创建```DoTween```扩展节点


| 参数         | 描述                      |
| ------------ | ------------------------- |
| tweenFactory | 返回需要执行的DoTween动作 |

* public void Recycle2Cache()  回收节点到缓存池

## Messages

- OnRecycled                    缓存池回调



```
注释：下面是一段DoTween扩展节点的使用方式，一般作为链式节点的一部分使用，用于在链式中插入一段DoTween的动作
public static IActionChain DOTween(this IActionChain selfChain, Func<Tweener> tweenFactory)
{
	return selfChain.Append(DOTweenAction.Allocate(tweenFactory));
}
```
