using UnityEngine;
using System.Collections;
using QFramework;

/// <summary>
/// 1.发送者需要,实现IMsgSender接口
/// 2.调用this.SendLogicMsg发送Receiver Show Sth消息,并传入两个参数
/// </summary>
public class Sender : MonoBehaviour,IMsgSender {

	// Update is called once per frame
	void Update () {
		this.SendLogicMsg ("Receiver Show Sth","你好","世界");
	}
}
