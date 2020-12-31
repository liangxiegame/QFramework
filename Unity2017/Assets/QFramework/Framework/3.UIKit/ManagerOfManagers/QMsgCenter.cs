/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/


using System;
using System.Collections.Generic;

namespace QFramework
{
	using UnityEngine;

	[MonoSingletonPath("[Event]/QMsgCenter")]
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
			mRegisteredManagers.Clear();
			
			MonoSingletonProperty<QMsgCenter>.Dispose();
		}

		void Awake()
		{
			DontDestroyOnLoad(this);
		}


		public void SendMsg(QMsg tmpMsg)
		{

			foreach (var manager in mRegisteredManagers)
			{
				if (manager.Key == tmpMsg.ManagerID)
				{
					manager.Value().SendMsg(tmpMsg);
					return;
				}
			}

			ForwardMsg(tmpMsg);
		}

		private static Dictionary<int, Func<QMgrBehaviour>> mRegisteredManagers =
			new Dictionary<int, Func<QMgrBehaviour>>();
		
		public static void RegisterManagerFactory(int mgrId, Func<QMgrBehaviour> managerFactory)
		{
			if (mRegisteredManagers.ContainsKey(mgrId))
			{
				mRegisteredManagers[mgrId] = managerFactory;
			}
			else
			{
				mRegisteredManagers.Add(mgrId, managerFactory);
			}
		}


		partial void ForwardMsg(QMsg tmpMsg);
	}
}