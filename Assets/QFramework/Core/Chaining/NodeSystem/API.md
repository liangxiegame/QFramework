### NodeSystem 逻辑节点:



``` C#
xxx.Instance
```



## 基础节点

#### 1.延时节点: DelayNode

通过 this(MonoBehaviour) 触发延时回调

``` csharp
this.Delay(1.0f, () =>
{
	Log.I("延时 1s");
});
```

通过申请 DelayNode 对象，使用 this(MonoBehaviour) 触发延时回调

``` csharp
var delay2s = DelayNode.Allocate(2.0f, () => { Log.I("延时 2s"); });
this.ExecuteNode(delay2s);
```

使用 Update 驱动延时回调

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





## 容器节点

#### 1.Sequence

