using System.Collections;
using System.Collections.Generic;
using QFramework.Example;
using UnityEngine;


namespace QFramework
{
	public partial class QMsgCenter 
	{

		public static void ForwardMsg(QMsg msg)
		{
			switch (msg.ManagerID)
			{
				case QMgrID.Game:
					Player.GameManager.Instance.SendMsg(msg);
					break;
			}
		}

	}
}