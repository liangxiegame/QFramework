using UnityEngine;
using QFramework;

namespace QFramework.Example
{
	public partial class ILKitBehaivourA
	{
		void OnStart()
		{
			// Code Here

			var ilKitBehaivourB = GameObject.Find("ILKitBehaivourB").GetILComponent<ILKitBehaivourB>();

			Debug.Log(ilKitBehaivourB.transform.name);
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}
