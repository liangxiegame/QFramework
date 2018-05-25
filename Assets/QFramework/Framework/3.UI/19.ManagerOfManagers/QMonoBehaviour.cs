/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework 
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	public abstract class QMonoBehaviour : MonoBehaviour
	{
		protected bool mReceiveMsgOnlyObjActive = true;
		
		public void Process (int eventId, params object[] param)  
		{
			if (mReceiveMsgOnlyObjActive && gameObject.activeInHierarchy || !mReceiveMsgOnlyObjActive)
			{
				QMsg msg = param[0] as QMsg;
				ProcessMsg(eventId, msg);
				msg.Processed = true;
				
				if (msg.ReuseAble)
				{
					msg.Recycle2Cache();
				}
			}
		}

		protected virtual void ProcessMsg (int eventId,QMsg msg) {}

		protected abstract void SetupMgr ();
		
		private QMgrBehaviour mPrivateMgr = null;
		
		protected QMgrBehaviour mCurMgr 
		{
			get 
			{
				if (mPrivateMgr == null ) 
				{
					SetupMgr ();
				}

				if (mPrivateMgr == null) 
				{
					Debug.LogError ("not set mgr yet");
				}

				return mPrivateMgr;
			}

			set { mPrivateMgr = value; }
		}
			
		public virtual void Show()
		{
			gameObject.SetActive (true);

			OnShow ();
		}

		protected virtual void OnShow() {}

		public virtual void Hide()
		{
			OnHide ();

			gameObject.SetActive (false);
			Log.I("On Hide:{0}",name);
		}

		protected virtual void OnHide() {}

		protected void RegisterEvents<T>(params T[] eventIDs) where T : IConvertible
		{
			foreach (var eventId in eventIDs)
			{
				RegisterEvent(eventId);
			}
		}

		protected void RegisterEvent<T>(T eventId) where T : IConvertible
		{
			mEventIds.Add(eventId.ToUInt16(null));
			mCurMgr.RegisterEvent(eventId, Process);
		}
		
		protected void UnRegisterEvent<T>(T eventId) where T : IConvertible
		{
			mEventIds.Remove(eventId.ToUInt16(null));
			mCurMgr.UnRegistEvent(eventId.ToInt32(null), Process);
		}

		protected void UnRegisterAllEvent()
		{
			if (null != mPrivateEventIds)
			{
				mCurMgr.UnRegisterEvents(mEventIds, Process);
			}
		}

		public virtual void SendMsg(QMsg msg)
		{
			mCurMgr.SendMsg(msg);
		}

        public virtual void SendEvent<T>(T eventId) where T : IConvertible
		{
			mCurMgr.SendEvent(eventId);
		}
		
		private List<ushort> mPrivateEventIds = null;
		
		private List<ushort> mEventIds
		{
			get
			{
				if (null == mPrivateEventIds)
				{
					mPrivateEventIds = new List<ushort>();
				}

				return mPrivateEventIds;
			}
		}

		protected virtual void OnDestroy()
		{
		    OnBeforeDestroy();
			mCurMgr = null;
			
			if (Application.isPlaying) 
			{
				UnRegisterAllEvent();
			}
		}
		
	    protected virtual void OnBeforeDestroy(){}
	}
}
