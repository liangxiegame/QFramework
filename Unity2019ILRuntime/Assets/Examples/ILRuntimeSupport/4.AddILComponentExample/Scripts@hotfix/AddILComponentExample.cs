using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class AddILComponentExample
	{
		void OnStart()
		{
			// Code Here
			var someComponent = GameObject.Find("GameObject")
				.AddILComponent<SomeComponent>();
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
	
	public class SomeComponent : ILComponent
	{
		protected override void OnStart()
		{
			Debug.Log("添加了");
		}

		protected override void OnDestroy()
		{
		
		}
	}
}
