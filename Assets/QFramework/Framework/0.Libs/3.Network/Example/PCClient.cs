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
    using System.Collections.Generic;
    
    [QMonoSingletonPath("[Framework]/PCClient")]
    public class PCClient : QMonoSingleton<PCClient>
    {
        private ISocketClient mSocketClient;

        public override void OnSingletonInit()
        {
            base.OnSingletonInit();
            Log.Level = LogLevel.Max;
            PCConnectMobileManager.Instance.SendEvent(PCConnectMobileEvent.PCClientCreated);
        }

        public void DisConnect()
        {
            mSocketClient.Close();
            mSocketClient.Disconnect();
        }

        protected override void OnDestroy()
        {
            DisConnect();
        }

        /// <summary>
        /// 连接到移动设备
        /// </summary>
        /// <param name="mobileIPAddress"></param>
        /// <returns></returns>
        public void ConnectToMobile(string mobileIPAddress,ConnectedCallback onConnectionEvent)
        {
            Log.I("mobileIPAddres:{0}", mobileIPAddress);
            mSocketClient = FlexiSocket.Create(mobileIPAddress, 1366, Protocols.BodyLengthPrefix);
            mSocketClient.Connected += onConnectionEvent;
            mSocketClient.Disconnected += OnDisconnected;
            mSocketClient.Received += OnReceived;
            mSocketClient.Sent += OnSent;
            mSocketClient.Connect();
            //this.Repeat()
            //    .Delay(1.0f)
            //    .Event(() => { mSocketClient.Connect(); })
            //    .Begin();
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="success"></param>
        /// <param name="exception"></param>
        /// <param name="error"></param>
        void OnSent(bool success, Exception exception, SocketError error)
        {
            if (success)
            {
                Log.I("OnSent");
            }
            else
            {
                Log.E(exception.ToString());
            }
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="success"></param>
        /// <param name="exception"></param>
        /// <param name="error"></param>
        /// <param name="message"></param>
        void OnReceived(bool success, Exception exception, SocketError error, byte[] message)
        {
            if (success)
            {
                SocketMsg msg = SerializeHelper.FromProtoBuff<SocketMsg>(message);
                Log.I("Received:{0}", msg.EventID);
                mMsgQueue.Enqueue(msg);
            }
        }

        public void SendMsg(SocketMsg msg)
        {
            mSocketClient.Send(SerializeHelper.ToProtoBuff(msg));
        }

        Queue<SocketMsg> mMsgQueue = new Queue<SocketMsg>();
        
        private void Update()
        {
            if (mMsgQueue.Count > 0)
            {
                SocketMsg msg = mMsgQueue.Dequeue();
                PCConnectMobileManager.Instance.SendMsg(msg);
            }
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <param name="success"></param>
        /// <param name="exception"></param>
        void OnDisconnected(bool success, Exception exception)
        {
            if (success)
            {
                Log.I("disconnected");
            }
            else
            {
                Log.I(exception.ToString());
            }
        }
    }
}