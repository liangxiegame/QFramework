/****************************************************************************
 * Copyright (c) 2017 ~2021.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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
				var msg = param[0] as IMsg;
				ProcessMsg(eventId, msg as QMsg);
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
			Manager.UnRegisterEvent(eventId.ToInt32(null), Process);
		}

		protected void UnRegisterAllEvent()
		{
			if (null != mPrivateEventIds)
			{
				mPrivateEventIds.ForEach(id => Manager.UnRegisterEvent(id,Process));
			}
		}

		public virtual void SendMsg(IMsg msg)
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
			get { return mPrivateEventIds ?? (mPrivateEventIds = new List<ushort>()); }
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
