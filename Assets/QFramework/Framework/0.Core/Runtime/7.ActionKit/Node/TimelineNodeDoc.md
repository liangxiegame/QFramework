>作者：布鞋     邮箱：827922094@qq.com

## TimelineNode
class in Actionkit / Inherits from:[NodeAction](www.baidu.com) 

## Description
时间轴执行节点，允许执行多个[TimelinePair](www.baidu.com)（```时间-节点```线对）

## Properties
* TimelineQueue        ```时间-节点```线对队列

## Construction
* public TimelineNode(params TimelinePair[] pairs)


| 参数  | 描述                    |
| ----- | ----------------------- |
| pairs | 多个```时间-节点```线对 |

## Action
* OnTimelineBeganCallback  时间轴开始时回调
* OnTimelineEndedCallback  时间轴结束时回调

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

* public void Append(TimelinePair pair)  增加```时间-节点```线对


| 参数 | 描述          |
| ---- | ------------- |
| pair | 时间-节点线对 |

* public void Append(float time, IAction node) 增加```时间-节点```线对多态


| 参数 | 描述     |
| ---- | -------- |
| time | 执行时间 |
| node | 执行节点 |
