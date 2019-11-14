>作者：布鞋     邮箱：827922094@qq.com

## RepeatNode
class in Actionkit / Inherits from:[NodeAction](www.baidu.com) / Implemented from:[INode](www.baidu.com) 

## Description
重复节点，用于轮循节点队列。一般由[chain](www.baidu.com)链式类型组合使用。

## Properties
* CurrentExecutingNode        当前正在执行的节点
* RepeatCount                 循环次数,-1默认值为一直循环。

## Construction
* public RepeatNode(IAction node, int repeatCount)


| 参数        | 描述                                                |
| ----------- | --------------------------------------------------- |
| node        | 需要循环的节点，这里是[序列执行节点](www.baidu.com) |
| repeatCount | 执行次数                                            |

## Inherits Methods

* Finish			      结束当前节点，

* Break                              设置节点的状态为Finish

* Reset                              重置节点状态

* public bool Execute(float dt)     执行当前节点，需要传入执行一次的时间，返回是否执行结束


  | 参数 | 描述           |
  | ---- | -------------- |
  | dt   | 执行一次的时间 |

* Dispose                          设置节点的状态为Dispose
