/****************************************************************************
 * Copyright (c) 2018.3 布鞋 827922094@qq.com
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
	using System;
	using System.Net.Sockets;
	using System.Collections;
	using System.Text;
    //using System;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// 其实是FlexiSocket 的 Server 端
	/// </summary>
	[QMonoSingletonPath("[Framework]/MobileServer")]
	public class MobileServer : QMonoSingleton<MobileServer>
	{
		private ISocketServer mSocketServer;
		private string mMessage = string.Empty;

		private void Awake()
		{
            ResMgr.Init();
			Log.Level = LogLevel.Max;
			PCConnectMobileManager.Instance.SendEvent(PCConnectMobileEvent.MobileServerCreated);
		}

		private IEnumerator Start()
		{
            UIMgr.OpenPanel<UIMsg>();

            mHostIp = NetworkUtil.GetAddressIP();

			mSocketServer = FlexiSocket.Create(1366, Protocols.BodyLengthPrefix, false);
			mSocketServer.ClientConnected += delegate(ISocketClientToken client)
			{             
				Log.I("OnClientConnected ID:{0} Count:{1}", client.ID, mSocketServer.Clients.Count);
			};
			mSocketServer.SentToClient += delegate(bool success, ISocketClientToken client)
			{
				if (success)
				{

				}
			};

			mSocketServer.ReceivedFromClient += delegate(ISocketClientToken client, byte[] message)
			{
				SocketMsg msg = SerializeHelper.FromProtoBuff<SocketMsg>(message);
				mMessage = msg.EventID + ":" + msg.ToEventID;
				if (!string.IsNullOrEmpty(msg.Msg))
				{
					mMessage += ":" + msg.Msg;
				}
				Log.I("OnReceivedFromClient:{0}", mMessage);
				mMsgQueue.Enqueue(msg);
			};
			
			mSocketServer.ClientDisconnected += delegate(ISocketClientToken client)
			{

			};
			mSocketServer.StartListen(10);

			yield return 0;
		}

		public void SendMsg(SocketMsg msg)
		{
			mSocketServer.SendToAll(msg.ToProtoBuff());
		}

		private Queue<SocketMsg> mMsgQueue = new Queue<SocketMsg>();

		private string mHostIp = "10.1.1.1";

		private void OnGUI()
		{
			GUIStyle guiStyle = new GUIStyle();
			guiStyle.fontSize = 40;
			guiStyle.normal.textColor = Color.red;
			GUI.Label(new Rect(100, 20, 100, 50), mHostIp,guiStyle);
			GUI.Label(new Rect(100, 100, 100, 50), mMessage,guiStyle);
		}

		void Update()
		{
			if (mMsgQueue.Count > 0)
			{
				SocketMsg msg = mMsgQueue.Dequeue();
				PCConnectMobileManager.Instance.SendMsg(msg);
			}
		}

	    protected override void OnDestroy()
		{
            mSocketServer.Close();
			mSocketServer = null;
		}
	}
}