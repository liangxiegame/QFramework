namespace QFramework.Example
{
	using UnityEngine;

	internal class Class2SignetonProperty : ISingleton
	{
		public static Class2SignetonProperty Instance
		{
			get { return SingletonProperty<Class2SignetonProperty>.Instance; }
		}

		private Class2SignetonProperty() {}
		
		private static int mIndex = 0;

		public void OnSingletonInit()
		{
			mIndex++;
		}

		public void Dispose()
		{
			SingletonProperty<Class2SignetonProperty>.Dispose();
		}
		
		public void Log(string content)
		{
			Debug.Log("Class2SingletonProperty" + mIndex + ":" + content);
		}
	}
		
	public class SingletonProperty : MonoBehaviour
	{
		// Use this for initialization
		void Start () 
		{
			Class2SignetonProperty.Instance.Log("Hello World!");	
			
			// delete current instance
			Class2SignetonProperty.Instance.Dispose();
			
			// new instance
			Class2SignetonProperty.Instance.Log("Hello World!");
		}
	}
}