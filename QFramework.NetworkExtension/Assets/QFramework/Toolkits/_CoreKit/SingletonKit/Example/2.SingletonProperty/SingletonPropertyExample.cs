namespace QFramework.Example
{
	using UnityEngine;

	internal class Class2SingletonProperty : ISingleton
	{
		public static Class2SingletonProperty Instance
		{
			get { return SingletonProperty<Class2SingletonProperty>.Instance; }
		}

		private Class2SingletonProperty() {}
		
		private static int mIndex = 0;

		public void OnSingletonInit()
		{
			mIndex++;
		}

		public void Dispose()
		{
			SingletonProperty<Class2SingletonProperty>.Dispose();
		}
		
		public void Log(string content)
		{
			Debug.Log("Class2SingletonProperty" + mIndex + ":" + content);
		}
	}
		
	public class SingletonPropertyExample : MonoBehaviour
	{
		// Use this for initialization
		void Start () 
		{
			Class2SingletonProperty.Instance.Log("Hello World!");	
			
			// delete current instance
			Class2SingletonProperty.Instance.Dispose();
			
			// new instance
			Class2SingletonProperty.Instance.Log("Hello World!");
		}
	}
}