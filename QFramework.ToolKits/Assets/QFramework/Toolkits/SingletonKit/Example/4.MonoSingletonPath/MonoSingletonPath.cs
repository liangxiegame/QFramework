namespace QFramework.Example
{
	using UnityEngine;

	[QFramework.MonoSingletonPath("[Example]/MonoSingletonPath")]
    internal class ClassUseMonoSingletonPath : MonoSingleton<ClassUseMonoSingletonPath>
	{
		
	}
	
	public class MonoSingletonPath : MonoBehaviour
	{
		private void Start()
		{
			var intance = ClassUseMonoSingletonPath.Instance;
		}
	}
}
