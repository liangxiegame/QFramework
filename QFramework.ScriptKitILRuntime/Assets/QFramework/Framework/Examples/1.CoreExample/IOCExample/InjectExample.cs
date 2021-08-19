using UnityEngine;

namespace QFramework.Example
{
	public class InjectExample : MonoBehaviour
	{
		[Inject] public A AObj;

		// Use this for initialization
		void Start()
		{
			var container = new QFrameworkContainer();
			container.RegisterInstance(new A());
			container.Inject(this);
			
			container.Resolve<A>().HelloWorld();
		}

		public class A
		{
			public void HelloWorld()
			{
				"This is A obj".LogInfo();
			}
		}
	}
}