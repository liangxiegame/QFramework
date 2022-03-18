namespace QFramework.Example
{
	using UnityEngine;

    internal class Class2Singleton : Singleton<Class2Singleton>
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
	
	public class SingletonExample : MonoBehaviour
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