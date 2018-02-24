### Extensions 模块简介:

简单来说都是对 .Net 和  Unity 的 API 进行了一层封装

#### 1.IsNull,IsNotNull:

``` csharp
var simpleClass = new object();

if (simpleClass.IsNull()) // simpleClass == null
{

}
else if (simpleClass.IsNotNull()) // simpleClasss != null
{

}
```

#### 2.InvokeGracefully:

**Action:**

``` csharp
Action action = () => Debug.Log("action called");
action.InvokeGracefully(); // if (action != null) action();
```

**Func:**

``` csharp
Func<int> func = () => 1;
func.InvokeGracefully();
			
/*
public static T InvokeGracefully<T>(this Func<T> selfFunc)
{
	return null != selfFunc ? selfFunc() : default(T);
}
*/ 
```

**Delegate:**

``` csharp
public delegate void TestDelegate();
...
TestDelegate testDelegate = () => { };
testDelegate.InvokeGracefully();
```



#### 3.ForEach:

**Array:**

``` csharp
var testArray = new int[] {1, 2, 3};
testArray.ForEach(number => Debug.Log(number));
```

**IEnumrable<T>:**

``` csharp
IEnumerable<int> testIenumerable = new List<int> {1, 2, 3};
testIenumerable.ForEach(number => Debug.Log(number));
```

**List<T>:**

``` csharp
var testList = new List<int> {1, 2, 3};
testList.ForEach(number => Debug.Log(number));
testList.ForEachReverse(number => Debug.Log(number));
```

#### 4.Merge:

**Dictionary<T,K>:**

``` csharp
var dictionary1 = new Dictionary<string, string> {{"1", "2"}};
var dictionary2 = new Dictionary<string, string> {{"3", "4"}};
var dictionary3 = dictionary1.Merge(dictionary2);
dictionary3.ForEach(pair => Debug.LogFormat("{0}:{1}", pair.Key, pair.Value));
```



#### FeatureId:EXDN001
