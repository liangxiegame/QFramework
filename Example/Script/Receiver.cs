using UnityEngine;
using System.Collections;

using QFramework;

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

	void OnDestroy()
	{
	}
}
