#### API ID: CUDP001 XXX.Instance

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
