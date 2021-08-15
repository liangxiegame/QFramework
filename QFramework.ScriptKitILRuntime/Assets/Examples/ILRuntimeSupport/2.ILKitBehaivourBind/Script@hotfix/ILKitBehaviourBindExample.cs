using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class ILKitBehaviourBindExample
	{
		void OnStart()
		{
			// Code Here
			Button.onClick.AddListener(() =>
			{
				Debug.Log("点击 按钮");
			});
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}
