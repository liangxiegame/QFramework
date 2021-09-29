using System;
 using UnityEngine;
 
 namespace QFramework.Example.ActionKit
 {
 	public class DelaySequenceExample : MonoBehaviour
 	{
 		void Start()
 		{
 			// 创建一个序列容器
 			var sequenecNode = new SequenceNode();
 
 			// 添加子节点
 			sequenecNode.Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)));
 			sequenecNode.Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)));
 			sequenecNode.Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)));
 			sequenecNode.Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)));
 			sequenecNode.Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)));
 			sequenecNode.Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)));
 
 			// 执行节点
 			this.ExecuteNode(sequenecNode);
 		}


        /// <summary>
        /// 简化版本
        /// </summary>
        void Simplify()
        {
	        this.Sequence()
		        .Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)))
		        .Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)))
		        .Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)))
		        .Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)))
		        .Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)))
		        .Append(DelayAction.Allocate(1.0f, () => Debug.Log("延时 1 秒" + DateTime.Now)))
		        .Begin();
        }

        /// <summary>
        /// 最终简化版本
        /// </summary>
        void FinalSimplify()
        {
	        this.Sequence()
		        .Delay(1.0f).Event(() => Debug.Log("延时 1 秒" + DateTime.Now))
		        .Delay(1.0f).Event(() => Debug.Log("延时 1 秒" + DateTime.Now))
		        .Delay(1.0f).Event(() => Debug.Log("延时 1 秒" + DateTime.Now))
		        .Delay(1.0f).Event(() => Debug.Log("延时 1 秒" + DateTime.Now))
		        .Delay(1.0f).Event(() => Debug.Log("延时 1 秒" + DateTime.Now))
		        .Delay(1.0f).Event(() => Debug.Log("延时 1 秒" + DateTime.Now))
		        .Begin();
        }
    }
 }