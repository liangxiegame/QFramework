/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QF;
using QF.Res;

namespace QFramework
{
	using UnityEngine;

	[QMonoSingletonPath("[Event]/QMsgCenter")]
	public partial class QMsgCenter : MonoBehaviour, ISingleton
	{
		public static QMsgCenter Instance
		{
			get { return MonoSingletonProperty<QMsgCenter>.Instance; }
		}

		public void OnSingletonInit()
		{

		}

		public void Dispose()
		{
			MonoSingletonProperty<QMsgCenter>.Dispose();
		}

		void Awake()
		{
			DontDestroyOnLoad(this);
		}

		public void SendMsg(QMsg tmpMsg)
		{
			// Framework Msg
			switch (tmpMsg.ManagerID)
			{
				case QMgrID.UI:
					UIManager.Instance.SendMsg(tmpMsg);
					return;
				case QMgrID.Audio:
					AudioManager.Instance.SendMsg(tmpMsg);
					return;
			}

			// ForwardMsg(tmpMsg);
		}
	}
}