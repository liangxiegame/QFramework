using UnityEngine;
 
 namespace QFramework.Example.ActionKit
 {
 	public class ActionLifetimeExample : MonoBehaviour
 	{
 		void Start()
 		{
 			var delayAction = DelayAction.Allocate(1.0f, () => { Debug.Log("延时完毕"); });
 
 			delayAction.OnBeganCallback = () => Debug.Log("开始延时");
 
 			delayAction.OnEndedCallback = () => Debug.Log("结束延时");
 
 			this.ExecuteNode(delayAction);
 		}
 	}
 }