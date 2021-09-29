using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class EventActionExample : MonoBehaviour
	{
		void Start()
		{
			var eventAction = EventAction.Allocate(() => Debug.Log("执行 EventAction"));
			
			this.ExecuteNode(eventAction);
		}

		void Simplify()
		{
			this.Sequence()
				.Delay(1.0f)
				.Event(() => Debug.Log("延时了 1 秒"))
				.Begin();
		}
	}
}
