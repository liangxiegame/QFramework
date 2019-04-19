>作者：布鞋     邮箱：827922094@qq.com

## UntilAction
class in Actionkit / Inherits from:[NodeAction](ActionKitAPI/Action/NodeAction.md)  / Implemented in : [IPoolable](www.baidu.com)

## Description
条件节点，条件达成时节点结束。一般用于[序列执行节点]()

## Properties
* IsRecycled 			```缓存标记```


## Inherits Methods

* Finish			      结束当前节点，

* Break                              设置节点的状态为Finish

* Reset                              重置节点状态

* public bool Execute(float dt)     执行当前节点，需要传入执行一次的时间，返回是否执行结束


  | 参数 | 描述           |
  | ---- | -------------- |
  | dt   | 执行一次的时间 |

* Dispose                          设置节点的状态为Dispose

## Static Methods

* public static UntilAction Allocate(Func<bool> condition)   从```缓存池```中创建条件节点


| 参数      | 描述                         |
| --------- | ---------------------------- |
| condition | 条件委托，条件达成时节点结束 |


## Messages
* OnRecycled		       缓存池回收回调

```      
public static IActionChain Until(this IActionChain selfChain, Func<bool> condition)
{
     return selfChain.Append(UntilAction.Allocate(condition));
}
```
