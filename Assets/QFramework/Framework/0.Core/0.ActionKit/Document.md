### 可执行节点系统:NodeActionSystem 

**NodeSystem** 的设计初衷是为了解决异步逻辑的管理问题，异步逻辑在日常开发中往往比较难以管理，而且代码的风格差异很大。诸如 "播放一段音效并获取播放完成的事件","当 xxx 为 true 时触发"，包括我们常用的 Tween 动画都是异步逻辑，以上的异步逻辑都可以用 **ExecuteNode** 来封装他们。由此设计出了 **NodeSystem**，灵感来自于 **cocos2d** 的 **CCAction**。

## 基础节点

#### 1.延时节点: DelayNode

通过 **this**(MonoBehaviour) 触发延时回调。

**快捷方式**

``` csharp
this.Delay(1.0f, () =>
{
	Log.I("延时 1s");
});
```



**面向对象**

通过申请 **DelayNode** 对象，使用 **this**(MonoBehaviour) 触发延时回调。

``` csharp
var delay2s = DelayNode.Allocate(2.0f, () => { Log.I("延时 2s"); });
this.ExecuteNode(delay2s);
```

使用 **Update** 驱动延时回调。

**Update 方式**

``` csharp
private DelayNode mDelay3s = DelayNode.Allocate(3.0f, () => { Log.I("延时 3s"); });

private void Update()
{
	if (mDelay3s != null && !mDelay3s.Finished && mDelay3s.Execute(Time.deltaTime))
	{
		Log.I("Delay3s 执行完成");
	}
}
```

**FeatureId:CEDN001**

#### 2.事件节点: EventNode

​	字如其意,**EventNode**,也就是分发事件。也许单独使用并不会发挥它的价值，但是在 **容器节点** 里他是不可或缺的。

通过申请 **EventNode** 对象，使用 **this**(MonoBehaviour) 触发事件执行。

``` csharp
var eventNode = EventNode.Allocate(() => { Log.I("event 1 called"); }, () => { Log.I("event 2 called"); });
this.ExecuteNode(eventNode);
```

使用 **Update** 驱动回调。

``` csharp
private EventNode mEventNode2 = EventNode.Allocate(() => { Log.I("event 3 called"); }, () => { Log.I("event 4 called"); });

private void Update()
{
	if (mEventNode2 != null && !mEventNode2.Finished && mEventNode2.Execute(Time.deltaTime))
	{
		Log.I("eventNode2 执行完成");
	}
}
```

**FeatureId:CEEN001**

## 容器节点

#### 1.Sequence

**SequenceNode** 字如其意就是序列节点，是一种 **容器节点** 可以将孩子节点按顺序依次执行，每次执行完一个节点再进行下一个节点。

通过 **this**(MonoBehaviour) 触发延时回调。

``` csharp
this.Sequence()
	.Delay(1.0f)
	.Event(() => Log.I("Sequence1 延时了 1s"))
	.Begin()
	.DisposeWhenFinished() // Default is DisposeWhenGameObjDestroyed
	.OnDisposed(() => { Log.I("Sequence1 destroyed"); });
```

通过申请 **SequenceNode** 对象，使用 **this**(MonoBehaviour) 触发节点执行。

``` csharp
	var sequenceNode2 = SequenceNode.Allocate(DelayNode.Allocate(1.5f));
	sequenceNode2.Append(EventNode.Allocate(() => Log.I("Sequence2 延时 1.5s")));
	sequenceNode2.Append(DelayNode.Allocate(0.5f));
	sequenceNode2.Append(EventNode.Allocate(() => Log.I("Sequence2 延时 2.0s")));

	this.ExecuteNode(sequenceNode2);

	/* 这种方式需要自己手动进行销毁
	sequenceNode2.Dispose();
	sequenceNode2 = null;
	*/

	// 或者 OnDestroy 触发时进行销毁
	sequenceNode2.AddTo(this);
```

使用 **Update** 驱动执行。

``` csharp
private SequenceNode mSequenceNode3 = SequenceNode.Allocate(
			DelayNode.Allocate(3.0f),
			EventNode.Allocate(() => { Log.I("Sequence3 延时 3.0f"); }));

private void Update()
{
    if (mSequenceNode3 != null 
        && !mSequenceNode3.Finished 
        && mSequenceNode3.Execute(Time.deltaTime))
	{
		Log.I("SequenceNode3 执行完成");
	}
}

private void OnDestroy()
{
	mSequenceNode3.Dispose();
	mSequenceNode3 = null;
}
```

