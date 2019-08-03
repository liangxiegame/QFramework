/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QF;

namespace QFramework
{
	using System;

	/// <summary>
	/// msgbody
	/// </summary>
	public class QMsg : IPoolable, IPoolType
	{	
		/// <summary>
		/// EventID
		/// TODO: raname 2 Id
		/// </summary>
		public virtual int EventID { get; set; }
		
		/// <summary>
		/// Processed or not
		/// </summary>
		public bool Processed { get; set; }
		
		/// <summary>
		/// reusable or not 
		/// </summary>
		public bool ReuseAble { get; set; }
		
		public int ManagerID
		{
			get { return EventID / QMsgSpan.Count * QMsgSpan.Count; }
		}

		public QMsg(){}

		#region Object Pool
		public static QMsg Allocate<T>(T eventId) where T : IConvertible
		{
			QMsg msg = SafeObjectPool<QMsg>.Instance.Allocate();
			msg.EventID = eventId.ToInt32(null);
			msg.ReuseAble = true;
			return msg;
		}

		public virtual void Recycle2Cache()
		{
			SafeObjectPool<QMsg>.Instance.Recycle(this);
		}

		void IPoolable.OnRecycled()
		{
			Processed = false;
		}
		
		bool IPoolable.IsRecycled { get; set; }
		#endregion

		#region deprecated since v0.0.5
		// for proto buf;
		[Obsolete("deprecated since 0.0.5,use EventID instead")]
		public int msgId
		{
			get { return EventID; }
			set { EventID = value; }
		}
		
		[Obsolete("GetMgrID() is deprecated,please use ManagerID Property instead")]
		public int GetMgrID()
		{
			return ManagerID;
		}
		
        //[Obsolete("deprecated,use allocate instead")]
		public QMsg(int eventID)
		{
			EventID = eventID;
		}
		#endregion
	}
}