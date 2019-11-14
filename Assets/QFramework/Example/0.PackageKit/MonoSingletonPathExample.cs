namespace QFramework.Example
{
    using UnityEngine;
    using QFramework;

    [MonoSingletonPath("[Example]/QMonoSingeltonPath")]
    class ClassUseMonoSingletonPath : MonoSingleton<ClassUseMonoSingletonPath>
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