

#### Singleton 的调用方式:
``` C#
xxx.Instance
```

#### 如何实现一个单例?

1.C# 类 通过继承 QSingleton<T>
``` C#
namespace QFramework.Example
{
	using UnityEngine;

	class Class2Singleton :QSingleton<Class2Singleton>
	{
		private static int mIndex = 0;

		private Class2Singleton() {}

		public override void OnSingletonInit()
		{
			mIndex++;
		}

		public void Log(string content)
		{
			Debug.Log("Class2Singleton" + mIndex + ":" + content);
		}
	}
	
	public class Singleton : MonoBehaviour
	{
		private void Start()
		{
			Class2Singleton.Instance.Log("Hello World!");
			
			// delete instance
			Class2Singleton.Instance.Dispose();
			
			// a differente instance
			Class2Singleton.Instance.Log("Hello World!");
		}
	}
}
```

2. MonoBehaviour 类 通过继承 QMonoSingleton<T>
``` C#
namespace QFramework.Example
{
	using System.Collections;
	using UnityEngine;
	
	class Class2MonoSingleton : QMonoSingleton<Class2MonoSingleton>
	{
		public override void OnSingletonInit()
		{
			Debug.Log(this.name + ":" + "OnSingletonInit");
		}

		private void Awake()
		{
			Debug.Log(this.name + ":" + "Awake");
		}

		private void Start()
		{
			Debug.Log(this.name + ":" + "Start");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			Debug.Log(this.name + ":" + "OnDestroy");
		}
	}
}
```
