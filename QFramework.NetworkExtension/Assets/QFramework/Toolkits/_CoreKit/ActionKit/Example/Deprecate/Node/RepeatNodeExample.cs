using System;
using UnityEngine;
 
 namespace QFramework.Example
 {
 	public class RepeatNodeExample : MonoBehaviour
 	{
	    private IDeprecateAction mRepeat;
        private void Update()
        {
	        if (Input.GetMouseButtonDown(0))
	        {
		        if (mRepeat != null)
		        {
			        mRepeat.Dispose();
			        mRepeat = null;
		        }

		        var delay = DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒"));

		        mRepeat = new RepeatNode(delay, 10);

		        this.ExecuteNode(mRepeat);
	        }

	        if (Input.GetMouseButtonDown(1))
	        {
		        Simplefy();
	        }
        }

        void Simplefy()
        {
	        
	        if (mRepeat != null)
	        {
		        mRepeat.Dispose();
		        mRepeat = null;
	        }
	        
	        // 不填次数就是无限循环
	        var node = this.Repeat()
		        .Delay(1.0f)
		        .Event(() => Debug.Log("延时 1 秒"));

	        node.Begin();
	        mRepeat = node;
        }
 	}
 }