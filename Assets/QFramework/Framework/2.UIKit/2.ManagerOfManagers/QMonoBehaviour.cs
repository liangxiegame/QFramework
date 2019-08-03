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

		
		public abstract IManager Manager { get; }
			
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
			mCachedEventIds.Add(eventId.ToUInt16(null));
			Manager.RegisterEvent(eventId, Process);
		}
		
		protected void UnRegisterEvent<T>(T eventId) where T : IConvertible
		{
			mCachedEventIds.Remove(eventId.ToUInt16(null));
			Manager.UnRegistEvent(eventId.ToInt32(null), Process);
		}

		protected void UnRegisterAllEvent()
		{
			if (null != mPrivateEventIds)
			{
				mPrivateEventIds.ForEach(id => Manager.UnRegistEvent(id,Process));
			}
		}

		public virtual void SendMsg(QMsg msg)
		{
			Manager.SendMsg(msg);
		}

        public virtual void SendEvent<T>(T eventId) where T : IConvertible
		{
			Manager.SendEvent(eventId);
		}
		
		private List<ushort> mPrivateEventIds = null;
		
		private List<ushort> mCachedEventIds
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
			if (Application.isPlaying) 
			{
				OnBeforeDestroy();
				UnRegisterAllEvent();
			}
		}
		
	    protected virtual void OnBeforeDestroy(){}
	}
}
