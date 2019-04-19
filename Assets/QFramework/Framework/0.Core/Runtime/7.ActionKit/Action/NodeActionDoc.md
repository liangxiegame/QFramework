>作者：布鞋     邮箱：827922094@qq.com

## NodeAction 

class in ActionKit / Implemented in : [IAction](https://github.com/827922094/Action-Kit-API/blob/master/www.baidu.com)

## Description

- 所有Actionkit节点的`抽象`父类。

## Properties

* Disposed   		    获得当前节点是否销毁
* Finished                        节点执行是否结束

## Action

   * OnBeganCallback 	节点第一次调用Execute时发出

   * OnEndedCallback         节点Finish时回调

   * OnDisposedCallback     节点Disposed时回调


## Public Methods

* Finish			      结束当前执行

* Break                              设置节点的状态为Finish

* Reset                              重置节点状态

* public bool Execute(float dt)     执行当前节点，需要传入执行一次的时间，返回是否执行结束


  | 参数 | 描述           |
  | ---- | -------------- |
  | dt   | 执行一次的时间 |

* Dispose                          设置节点的状态为Dispose

## Messages

- OnReset                         重置回调虚函数
- OnBegin                         开始回调虚函数
- OnExecute                      执行回调虚函数
- OnEnd                            结束回调虚函数
- OnDispose                     销毁回调虚函数



