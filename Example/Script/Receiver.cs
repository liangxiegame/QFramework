using UnityEngine;
using System.Collections;

using QFramework;

/// <summary>
/// 1.接收者需要实现IMsgReceiver接口。
/// 2.使用this.RegisterLogicMsg注册消息和回调函数。
/// </summary>
public class Receiver : MonoBehaviour,IMsgReceiver {

	void Awake()
	{
		this.RegisterLogicMsg ("Receiver Show Sth", ReceiverMsg);

//		this.UnRegisterLogicMsg ("Receiver Show Sth", ReceiverMsg);

	}


	void ReceiverMsg(params object[] paramList)
	{
		foreach (var sth in paramList) {
			QPrint.Warn (sth.ToString());
		}
	}
}
