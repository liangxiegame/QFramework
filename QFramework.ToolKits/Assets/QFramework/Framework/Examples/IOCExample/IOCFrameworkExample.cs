using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
{
	public class IOCFrameworkExample : MonoBehaviour
	{

		[Inject]
		public INetworkExampleService NetworkExampleService { get; set; }

		// Use this for initialization
		void Start()
		{
			// 将模块注入 
			// 这种方式比较方便
			MainContainer.Container.Inject(this);

			NetworkExampleService.Request();


			// 或者 不通过注入，直接获得 实例
			// 这种方式性能更好
			var networkExampleService = MainContainer.Container.Resolve<INetworkExampleService>();

			networkExampleService.Request();
		}
	}
}