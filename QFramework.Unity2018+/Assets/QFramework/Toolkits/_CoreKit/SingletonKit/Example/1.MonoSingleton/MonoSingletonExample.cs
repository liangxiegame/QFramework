namespace QFramework.Example
{
	using System.Collections;
	using UnityEngine;

	internal class Class2MonoSingleton : MonoSingleton<Class2MonoSingleton>
	{
		public override void OnSingletonInit()
		{
			Debug.Log(name + ":" + "OnSingletonInit");
		}

		private void Awake()
		{
			Debug.Log(name + ":" + "Awake");
		}

		private void Start()
		{
			Debug.Log(name + ":" + "Start");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			Debug.Log(name + ":" + "OnDestroy");
		}
	}

	public class MonoSingletonExample : MonoBehaviour
	{
		private IEnumerator Start()
		{
			var instance = Class2MonoSingleton.Instance;

			yield return new WaitForSeconds(3.0f);
			
			instance.Dispose();
		}
	}
}