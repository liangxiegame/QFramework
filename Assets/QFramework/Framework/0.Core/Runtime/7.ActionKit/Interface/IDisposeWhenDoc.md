>作者：布鞋     邮箱：827922094@qq.com

## IDisposeWhen

interface in ActionKit /  Implemented in : IDisposeEventRegister

## Description

继承该接口的节点可以传入委托监听结束和销毁,同时可以设置结束条件。

## Public Methods

- IDisposeEventRegister DisposeWhen(Func<bool> condition);

| 参数            | 描述                 |
| --------------- | -------------------- |
| condition | 返回是否结束的条件委托 |
