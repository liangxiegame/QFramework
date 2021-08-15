using UnityEngine;
 
 namespace QFramework.Example.ActionKit
 {
 	public class RepeatNodeExample : MonoBehaviour
 	{
 		void Start()
 		{
 			var delay = DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒"));
 			
 			var repeatNode = new RepeatNode(delay,10);
 
 			this.ExecuteNode(repeatNode);
        }


        void Simplefy()
        {
	        // 不填次数就是无限循环
	        this.Repeat()
		        .Delay(1.0f)
		        .Event(() => Debug.Log("延时 1 秒"))
		        .Begin();
        }
 	}
 }