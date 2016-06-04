using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework {
	/// <summary>
	/// 消息分发器
	/// C# this扩展 需要静态类
	/// </summary>
	public static class QMsgDispatcher  {

		/// <summary>
		/// 消息捕捉器
		/// </summary>
		class LogicMsgHandler {

			public IMsgReceiver receiver;
			public  VoidDelegate.WithParams callback;

			/*
			 * VoidDelegate.WithParams 是一种委托 ,定义是这样的 
			 * 
			 *  public class VoidDelegate{
			 *  	public delegate void WithParams(params object[] paramList);
			 *  }
			 */
			public LogicMsgHandler(IMsgReceiver receiver,VoidDelegate.WithParams callback)
			{
				this.receiver = receiver;
				this.callback = callback;
			}
		}

		/// <summary>
		/// 每个消息名字维护一组消息捕捉器。
		/// </summary>
		static Dictionary<string,List<LogicMsgHandler>> mMsgHandlerDict = new Dictionary<string,List<LogicMsgHandler>> ();


		/// <summary>
		/// 注册消息,
		/// 注意第一个参数,使用了C# this的扩展,
		/// 所以只有实现IMsgReceiver的对象才能调用此方法
		/// </summary>
		public static void RegisterLogicMsg(this IMsgReceiver self, string msgName,VoidDelegate.WithParams callback)
		{
			// 略过
			if (string.IsNullOrEmpty(msgName)) {
				QPrint.FrameworkWarn("RegisterMsg:" + msgName + " is Null or Empty");
				return;
			}

			// 略过
			if (null == callback) {
				QPrint.FrameworkWarn ("RegisterMsg:" + msgName + " callback is Null");
				return;
			}

			// 略过
			if (!mMsgHandlerDict.ContainsKey (msgName)) {
				mMsgHandlerDict [msgName] = new List<LogicMsgHandler> ();
			}

			// 看下这里
			var handlers = mMsgHandlerDict [msgName];

			// 略过
			// 防止重复注册
			foreach (var handler in handlers) {
				if (handler.receiver == self && handler.callback == callback) {
					QPrint.FrameworkWarn ("RegisterMsg:" + msgName + " ayready Register");
					return;
				}
			}

			// 再看下这里
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
		/// 注意第一个参数
		/// </summary>
		public static void SendLogicMsg(this IMsgSender sender, string msgName,params object[] paramList )
		{
			// 略过,不用看
			if (string.IsNullOrEmpty(msgName)) {
				QPrint.FrameworkError("SendMsg is Null or Empty");
				return;
			} 

			// 略过,不用看
			if (!mMsgHandlerDict.ContainsKey(msgName)){
				QPrint.FrameworkWarn("SendMsg is UnRegister");
				return;
			}

			// 开始看!!!!
			var handlers = mMsgHandlerDict[msgName];


			var handlerCount = handlers.Count;

			// 之所以是从后向前遍历,是因为  从前向后遍历删除后索引值会不断变化
			// 参考文章,http://www.2cto.com/kf/201312/266723.html
			for (int index = handlerCount - 1;index >= 0;index--)
			{
				var handler = handlers[index];

				if (handler.receiver != null) {
					QPrint.FrameworkLog ("SendLogicMsg:" + msgName + " Succeed");
					handler.callback (paramList);
				} else {
					handlers.Remove (handler);
				}
			}
		}





	}
}
