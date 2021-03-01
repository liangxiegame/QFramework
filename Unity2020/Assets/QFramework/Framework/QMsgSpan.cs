/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2017 liangxie
****************************************************************************/
namespace QFramework
{
	public abstract class QMgrID
	{
		public const int Framework = 0;
		public const int UI = Framework + QMsgSpan.Count; // 3000
		public const int Audio = UI + QMsgSpan.Count; // 6000
		public const int Network = Audio + QMsgSpan.Count;
		public const int UIFilter = Network + QMsgSpan.Count;
		public const int Game = UIFilter + QMsgSpan.Count;
		public const int PCConnectMobile = Game + QMsgSpan.Count;
		public const int FrameworkEnded = PCConnectMobile + QMsgSpan.Count;
		public const int FrameworkMsgModuleCount = 7;
	}
}