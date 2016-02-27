using UnityEngine;
using System.Collections;

namespace QFramework {
	public class QScene : QNode {

		[SerializeField]
		protected GameObject[] managedObjs;

		public void Enter()
		{
			OnEnter ();
		}

		public virtual void OnEnter()
		{

		}
	}
}
