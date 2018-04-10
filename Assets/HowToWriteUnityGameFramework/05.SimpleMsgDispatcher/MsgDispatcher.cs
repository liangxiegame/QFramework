/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
	using System;
	using System.Collections.Generic;

	public interface IMsgReceiver
	{
	}

	public interface IMsgSender
	{
	}

	/// <summary>
	/// 消息分发器
	/// C# this扩展 需要静态类
	/// 教程地址:http://liangxiegame.com/post/5/
	public static class MsgDispatcher
	{
		/// <summary>
		/// 消息捕捉器
		/// </summary>
		private class LogicMsgHandler
		{
			public readonly IMsgReceiver Receiver;
			public readonly Action<object[]> Callback;

			public LogicMsgHandler(IMsgReceiver receiver, Action<object[]> callback)
			{
				Receiver = receiver;
				Callback = callback;
			}
		}

		/// <summary>
		/// 每个消息名字维护一组消息捕捉器。
		/// </summary>
		static readonly Dictionary<string, List<LogicMsgHandler>> mMsgHandlerDict =
			new Dictionary<string, List<LogicMsgHandler>>();


		/// <summary>
		/// 注册消息,
		/// 注意第一个参数,使用了C# this的扩展,
		/// 所以只有实现IMsgReceiver的对象才能调用此方法
		/// </summary>
		public static void RegisterLogicMsg(this IMsgReceiver self, string msgName, Action<object[]> callback)
		{
			// 略过
			if (string.IsNullOrEmpty(msgName))
			{
				Log.W("RegisterMsg:" + msgName + " is Null or Empty");
				return;
			}

			// 略过
			if (null == callback)
			{
				Log.W("RegisterMsg:" + msgName + " callback is Null");
				return;
			}

			// 略过
			if (!mMsgHandlerDict.ContainsKey(msgName))
			{
				mMsgHandlerDict[msgName] = new List<LogicMsgHandler>();
			}

			// 看下这里
			var handlers = mMsgHandlerDict[msgName];

			// 略过
			// 防止重复注册
			foreach (var handler in handlers)
			{
				if (handler.Receiver == self && handler.Callback == callback)
				{
					Log.W("RegisterMsg:" + msgName + " ayready Register");
					return;
				}
			}

			// 再看下这里
			handlers.Add(new LogicMsgHandler(self, callback));
		}

		/// <summary>
		/// 发送消息
		/// 注意第一个参数
		/// </summary>
		public static void SendLogicMsg(this IMsgSender sender, string msgName, params object[] paramList)
		{
			// 略过,不用看
			if (string.IsNullOrEmpty(msgName))
			{
				Log.E("SendMsg is Null or Empty");
				return;
			}

			// 略过,不用看
			if (!mMsgHandlerDict.ContainsKey(msgName))
			{
				Log.W("SendMsg is UnRegister");
				return;
			}

			// 开始看!!!!
			var handlers = mMsgHandlerDict[msgName];
			var handlerCount = handlers.Count;

			// 之所以是从后向前遍历,是因为  从前向后遍历删除后索引值会不断变化
			// 参考文章,http://www.2cto.com/kf/201312/266723.html
			for (var index = handlerCount - 1; index >= 0; index--)
			{
				var handler = handlers[index];

				if (handler.Receiver != null)
				{
					Log.W("SendLogicMsg:" + msgName + " Succeed");
					handler.Callback(paramList);
				}
				else
				{
					handlers.Remove(handler);
				}
			}
		}

		/// <summary>
		/// 注销消息
		/// 注意第一个参数,使用了C# this的扩展,
		/// 所以只有实现IMsgReceiver的对象才能调用此方法
		/// </summary>
		public static void UnRegisterLogicMsg(this IMsgReceiver self, string msgName, Action<object[]> callback)
		{
			if (msgName.IsNullOrEmpty() || null == callback)
			{
				return;
			}

			var handlers = mMsgHandlerDict[msgName];

			// 删除List需要从后向前遍历
			for (var index = handlers.Count - 1; index >= 0; index--)
			{
				var handler = handlers[index];
				if (handler.Receiver == self && handler.Callback == callback)
				{
					handlers.Remove(handler);
					break;
				}
			}
		}
	}
}