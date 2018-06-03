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

    public enum PCConnectMobileEvent
    {
        Begin = QMgrID.PCConnectMobile,
        PCClientCreated,
        MobileServerCreated,
        SocketEvent,
        Debug,
        Ended
    }

    [ProtoBuf.ProtoContract]
    public class SocketMsg : QMsg
    {
        [ProtoBuf.ProtoMember(1)]
        public override int EventID { get; set; }

        [ProtoBuf.ProtoMember(2)] public byte[] ByteData;

        [ProtoBuf.ProtoMember(3)] public int ToEventID;

        [ProtoBuf.ProtoMember(4)] public string Msg;

        public SocketMsg()
        {
        }

        public SocketMsg(int toEventID) : base((int) PCConnectMobileEvent.SocketEvent)
        {
            ToEventID = toEventID;
        }

        public SocketMsg(int toEventID, byte[] byteData) : base((int) PCConnectMobileEvent.SocketEvent)
        {
            ByteData = byteData;
            ToEventID = toEventID;
        }

        public SocketMsg(int toEventID, string msg) : base((int) PCConnectMobileEvent.SocketEvent)
        {
            Msg = msg;
            ToEventID = toEventID;
        }
        
        public SocketMsg(int toEventID, string msg,byte[] byteData) : base((int) PCConnectMobileEvent.SocketEvent)
        {
            Msg = msg;
            ByteData = byteData;
            ToEventID = toEventID;
        }
    }

    /// <summary>
    /// PC Connect Mobile , Share Mobile's API
    /// Socket Msg Center
    /// </summary>
    [QMonoSingletonPath("[Framework]/PCConnectMobileManager")]
    public class PCConnectMobileManager : QMgrBehaviour, ISingleton
    {

        public bool IsPCClient { get; protected set; }
        public bool IsMobileServer { get; protected set; }

        public void OnSingletonInit()
        {
            RegisterEvent(PCConnectMobileEvent.PCClientCreated);
            RegisterEvent(PCConnectMobileEvent.MobileServerCreated);
            RegisterEvent(PCConnectMobileEvent.SocketEvent);
        }

        protected override void ProcessMsg(int eventId, QMsg msg)
        {
            switch (eventId)
            {
                case (int)PCConnectMobileEvent.PCClientCreated:

                    if (IsMobileServer)
                    {
                        throw new Exception(
                            "mobile server already created,cannot create both pc client and mobile server");
                    }
                    IsPCClient = true;
                    IsMobileServer = false;
                    break;

                case (int)PCConnectMobileEvent.MobileServerCreated:
                    if (IsPCClient)
                    {
                        throw new Exception("pc client already created,cannot create both pc client and mobile server");
                    }
                    IsPCClient = false;
                    IsMobileServer = true;
                    break;
                case (int)PCConnectMobileEvent.SocketEvent:
                    SocketMsg socketMsg = msg as SocketMsg;
                    socketMsg.EventID = socketMsg.ToEventID;

                    if (IsPCClient)
                    {
                        PCClient.Instance.SendMsg(msg as SocketMsg);
                    }
                    else if (IsMobileServer)
                    {
                        MobileServer.Instance.SendMsg(msg as SocketMsg);
                    }
                    else
                    {
                        SendMsg(socketMsg);
                    }
                    break;
            }
        }

        public static PCConnectMobileManager Instance
        {
            get { return MonoSingletonProperty<PCConnectMobileManager>.Instance; }
        }

        protected override void SetupMgrId()
        {
            mMgrId = QMgrID.PCConnectMobile;
        }
    }
}