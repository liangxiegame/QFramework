namespace QFramework.Example
{
	using UnityEngine;

	[MonoSingletonPath("[Example]/MonoSingletonPath")]
    internal class ClassUseMonoSingletonPath : MonoSingleton<ClassUseMonoSingletonPath>
	{
		
	}
	
	public class MonoSingletonPathExample : MonoBehaviour
	{
		private void Start()
		{
			var intance = ClassUseMonoSingletonPath.Instance;
		}
	}
}
