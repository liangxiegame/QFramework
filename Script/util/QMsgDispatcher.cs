using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {
	/// <summary>
	/// 消息分发器
	/// </summary>
	public static class QMsgDispatcher  {

		/// <summary>
		/// 消息捕捉器
		/// </summary>
		class LogicMsgHandler {

			public IMsgReceiver receiver;
			public  VoidDelegate.WithParams callback;

			public LogicMsgHandler(IMsgReceiver receiver,VoidDelegate.WithParams callback)
			{
				this.receiver = receiver;
				this.callback = callback;
			}
		}

		static Dictionary<string,List<LogicMsgHandler>> mMsgHandlerDict = new Dictionary<string,List<LogicMsgHandler>> ();


		/// <summary>
		/// 注册消息
		/// </summary>
		public static void RegisterLogicMsg(this IMsgReceiver self, string msgName,VoidDelegate.WithParams callback)
		{
			if (string.IsNullOrEmpty(msgName)) {
				QPrint.FrameworkError("RegisterMsg is Null or Empty");
				return;
			}

			if (null == callback) {
				QPrint.FrameworkError ("RegisterMsg callback is Null");
				return;
			}


			if (!mMsgHandlerDict.ContainsKey (msgName)) {
				mMsgHandlerDict [msgName] = new List<LogicMsgHandler> ();
			}

			var handlers = mMsgHandlerDict [msgName];

			// 防止重复注册
			foreach (var handler in handlers) {
				if (handler.receiver == self && handler.callback == callback) {
					return;
				}
			}

			handlers.Add (new LogicMsgHandler (self, callback));
		}

		/// <summary>
		/// 注销逻辑消息
		/// </summary>
		public static void UnRegisterLogicMsg(this IMsgReceiver self,string msgName,VoidDelegate.WithParams callback)
		{
			if (string.IsNullOrEmpty(msgName)) {
				QPrint.FrameworkError("UnRegisterMsg is Null or Empty");
				return;
			} 

			if (null == callback) {
				QPrint.FrameworkError ("RegisterMsg callback is Null");
				return;
			}

			var handlers = mMsgHandlerDict [msgName];

			int handlerCount = handlers.Count;

			// 删除List需要从后向前遍历
			for (int index = handlerCount - 1; index >= 0; index--) {
				var handler = handlers [index];
				if (handler.receiver == self && handler.callback == callback) {
					handlers.Remove (handler);
					break;
				}
			}


		}



		/// <summary>
		/// 发送消息
		/// </summary>
		public static void SendLogicMsg(this IMsgSender sender, string msgName,params object[] paramList )
		{
			if (string.IsNullOrEmpty(msgName)) {
				QPrint.FrameworkError("SendMsg is Null or Empty");
				return;
			} 

			if (!mMsgHandlerDict.ContainsKey(msgName)){
				QPrint.FrameworkWarn("SendMsg is UnRegister");
				return;
			}

			var handlers = mMsgHandlerDict[msgName];
			var handlerCount = handlers.Count;
			for (int index = handlerCount - 1;index >= 0;index--)
			{
				var handler = handlers[index];

				if (handler.receiver != null) {
					handler.callback (paramList);
				} else {
					handlers.Remove (handler);
				}
			}
		}





	}
}
