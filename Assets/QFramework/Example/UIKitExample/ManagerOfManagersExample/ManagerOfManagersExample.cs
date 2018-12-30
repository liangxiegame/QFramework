using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace QFramework.Example
{
	
	
	

	public class ManagerOfManagersExample : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}


		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				SendEvent(PlayerEvent.Run);
			}
		}
	}
}