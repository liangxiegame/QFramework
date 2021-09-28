using UnityEngine;

namespace QFramework.Example.ActionKit
{
	public class TimelineNodeExample : MonoBehaviour
	{
		void Start()
		{
			var timelineNode = new Timeline();
			
			// 第一秒输出 HelloWorld
			timelineNode.Append(1.0f, EventAction.Allocate(() => Debug.Log("HelloWorld")));
			
			// 第二秒输出 延时了 2 秒
			timelineNode.Append(2.0f, EventAction.Allocate(()=>Debug.Log("延时了 2 秒")));
			
			// // 第三秒发送 一个事件
			// timelineNode.Append(3.0f,new KeyEventAction("someEventA", timelineNode));
			//
			// // 第四秒发送 一个事件
			// timelineNode.Append(4.0f, new KeyEventAction("someEventB", timelineNode));
			
			// 监听 timeline 的 key 事件
			timelineNode.OnKeyEventsReceivedCallback = keyEvent => Debug.Log(keyEvent);

			// 执行 timeline
			this.ExecuteNode(timelineNode);
		}
	}
}