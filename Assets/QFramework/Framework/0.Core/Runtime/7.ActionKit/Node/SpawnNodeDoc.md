>作者：布鞋     邮箱：827922094@qq.com

## SpawnNode
class in Actionkit / Inherits from:[NodeAction](www.baidu.com)

## Description
并发节点，用于存储需要并发执行的节点。加入的节点将在同一帧执行。

## Construction

*  public SpawnNode(params NodeAction[] nodes)


| 参数  | 描述               |
| ----- | ------------------ |
| nodes | 需要并发执行的节点 |

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

*   public void Add(params NodeAction[] nodes)	


| 参数  | 描述               |
| ----- | ------------------ |
| nodes | 需要加入链中的节点 |

