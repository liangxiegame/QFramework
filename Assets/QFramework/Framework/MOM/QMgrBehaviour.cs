/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework 
{
	using System;
	using System.Collections.Generic;
	
	/// <summary>
	/// manager基类
	/// </summary>
	public abstract class QMgrBehaviour : QMonoBehaviour,IManager
	{
		private QEventSystem mEventSystem = NonPublicObjectPool<QEventSystem>.Instance.Allocate();

		#region IManager
		public virtual void Init() {}
		#endregion

		protected int mMgrId = 0;

		protected abstract void SetupMgrId ();

		protected override void SetupMgr ()
		{
			mCurMgr = this;
		}

		protected QMgrBehaviour() 
		{
			SetupMgrId ();
		}

		public void RegisterEvents<T>(List<T> eventIds,OnEvent process) where T: IConvertible
		{
			for (int i = 0;i < eventIds.Count;i++)
			{
				RegisterEvent(eventIds[i],process);
			}
		}

		public void RegisterEvent<T>(T msgId,OnEvent process) where T:IConvertible
		{
			mEventSystem.Register (msgId, process);
		}

		public void UnRegisterEvents(List<ushort> msgs,OnEvent process)
		{
			for (int i = 0;i < msgs.Count;i++)
			{
				UnRegistEvent(msgs[i],process);
			}
		}

		public void UnRegistEvent(int msgEvent,OnEvent process)
		{
			mEventSystem.UnRegister (msgEvent, process);
		}

		public void SendMsg(QMsg msg)
		{
			if (msg.GetMgrID() == mMgrId)
			{
				Process(msg.msgId,msg);
			}
			else 
			{
				QMsgCenter.Instance.SendMsg (msg);
			}
		}

		public void SendEvent<T>(T eventId) where T : IConvertible
		{
			SendMsg(QMsg.Allocate(eventId));
		}

		// 来了消息以后,通知整个消息链
		protected override void ProcessMsg(int eventId,QMsg msg)
		{
			mEventSystem.Send(msg.msgId,msg);
		}
	}
}