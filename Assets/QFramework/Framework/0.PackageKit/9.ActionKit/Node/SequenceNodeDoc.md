>作者：布鞋     邮箱：827922094@qq.com

## SequenceNode
class in Actionkit / Inherits from:[NodeAction](www.baidu.com) / Implemented from:[INode](www.baidu.com)

## Description
序列节点，用于添加多个节点。

## Properties
* TotalCount   	当前执行的节点总数            
* CurrentExecutingNode    当前执行的节点

## Construction

* public SequenceNode(params IAction[] nodes)	


| 参数  | 描述         |
| ----- | ------------ |
| nodes | 可选节点参数 |

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

*  public SequenceNode Append(IAction appendedNode)	


| 参数         | 描述                 |
| ------------ | -------------------- |
| appendedNode | 需要加入序列中的节点 |
```
var sequenceNode2 = new SequenceNode(DelayAction.Allocate(1.5f));
sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 1.5s")));
sequenceNode2.Append(DelayAction.Allocate(0.5f));
sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 2.0s")));
this.ExecuteNode(sequenceNode2);
```

