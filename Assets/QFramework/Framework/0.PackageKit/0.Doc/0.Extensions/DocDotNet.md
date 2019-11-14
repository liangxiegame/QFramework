### DotNet Extensions:

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

#### 5.CreateDirIfNotExists,DeleteDirIfExists,DeleteFileIfExists

``` csharp
var testDir = Application.persistentDataPath.CombinePath("TestFolder");
testDir.CreateDirIfNotExists();

Debug.Log(Directory.Exists(testDir));
testDir.DeleteDirIfExists();
Debug.Log(Directory.Exists(testDir));

var testFile = testDir.CombinePath("test.txt");
testDir.CreateDirIfNotExists();
File.Create(testFile);
testFile.DeleteFileIfExists();
```

#### 6.ImplementsInterface<T>

``` csharp
this.ImplementsInterface<ISingleton>();
```

#### 7.ReflectionExtension.GetAssemblyCSharp()

``` csharp
var selfType = ReflectionExtension.GetAssemblyCSharp().GetType("QFramework.Example.ExtensionExample");
Debug.Log(selfType);
```

#### 8.string's extension

``` csharp
var emptyStr = string.Empty;

Debug.Log(emptyStr.IsNotNullAndEmpty());
Debug.Log(emptyStr.IsNullOrEmpty());
emptyStr = emptyStr.Append("appended").Append("1").ToString();
Debug.Log(emptyStr);
Debug.Log(emptyStr.IsNullOrEmpty());
```

#### FeatureId:EXDN001