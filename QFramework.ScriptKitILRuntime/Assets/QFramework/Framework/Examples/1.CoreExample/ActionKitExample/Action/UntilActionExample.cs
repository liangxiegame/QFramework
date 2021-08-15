using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class UntilActionExample : MonoBehaviour
	{
		void Start()
		{
			var untilAction = UntilAction.Allocate(() => Input.GetMouseButtonDown(0));

			untilAction.OnEndedCallback = () =>
			{
				Debug.Log("鼠标按钮点击了"); 
				
			};

			this.ExecuteNode(untilAction);

		}

		void Simplify()
		{
			this.Sequence()
				.Until(() => Input.GetMouseButtonDown(0))
				.Event(() => Debug.Log("鼠标按钮点击了"))
				.Begin();
		}
	}
}