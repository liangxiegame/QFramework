using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.IO;
/// <summary>
/// 网络消息库
/// </summary>
//using SLua;
//using Thrift.Protocol;
//using Thrift.Transport;
//
//[CustomLuaClass]
public class NetMgr : QSingleton<NetMgr>
{
    private Socket mSocket = null;
    private Thread mSendThread = null;
    private Thread mRecvThread = null;
    private volatile bool mIsRunning = false;
    // 发送
    private object mSendLock = null;
    private Queue<NetMsg> mSendingMsgQueue = null;
    private Queue<NetMsg> mSendWaitingMsgQueue = null;
    // 接收
    private BufferedStream mRecvStream = null;
    private object mRecvLock = null;
    private Queue<NetMsg> mRecvingMsgQueue = null;
    private Queue<NetMsg> mRecvWaitingMsgQueue = null;

    private NetMgr()
    {
//        Game.Instance().onApplicationQuit += Disconnect;
    }

    public void Connect(string host, int port)
    {
        if (this.mIsRunning)
            return;

        if (string.IsNullOrEmpty(host))
        {
            Debug.LogError("NetMgr.Connect host is null");
            return;
        }

        IPEndPoint ipEndPoint = null;
        Regex regex = new Regex("((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)\\.){3}(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|[1-9])");
        Match match = regex.Match(host);
        if (match.Success)
        {
            // IP
            ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }
        else
        {
            // 域名
            IPAddress[] addresses = Dns.GetHostAddresses(host);
            ipEndPoint = new IPEndPoint(addresses[0], port);
        }
        this.mSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.mSendLock = new object();
        this.mSendingMsgQueue = new Queue<NetMsg>();
        this.mSendWaitingMsgQueue = new Queue<NetMsg>();
        this.mRecvLock = new object();
        this.mRecvingMsgQueue = new Queue<NetMsg>();
        this.mRecvWaitingMsgQueue = new Queue<NetMsg>();
//        Game.Instance().onUpdate += Update;
        try
        {
            this.mIsRunning = true;
            this.mSocket.Connect(ipEndPoint);
            this.mSendThread = new Thread(new ThreadStart(Send));
            this.mRecvThread = new Thread(new ThreadStart(Receive));
            this.mSendThread.Start();
            this.mRecvThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            Disconnect();
        }
    }

    public void Disconnect()
    {
        if (!this.mIsRunning)
            return;
        try
        {
            if (this.mSocket.Available != 0)
                this.mSocket.Shutdown(SocketShutdown.Both);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            this.mIsRunning = false;
            this.mSocket.Close();
        }
    }

    /// <summary>
    /// 提供给Lua框架的接口，运行在主线程
    /// FUCK lua中没有byte[]，所以用string
    /// </summary>
    /// <param name="msgType"></param>
    /// <param name="msgData"></param>
    public void SendNetMsg(int msgType, byte[] msgData)
    {
        NetMsg msg = new NetMsg(msgType, msgData);
        // 发送
        lock (this.mSendLock)
        {
            this.mSendWaitingMsgQueue.Enqueue(msg);
            Monitor.Pulse(this.mSendLock);
        }
    }

    void Send()
    {
        Debug.Log("NetMgr Send Thread start!");
        while (this.mIsRunning)
        {
            if (this.mSendingMsgQueue.Count == 0)
            {
                lock (this.mSendLock)
                {
                    while (this.mSendWaitingMsgQueue.Count == 0)
                        Monitor.Wait(this.mSendLock);
                    Queue<NetMsg> temp = this.mSendingMsgQueue;
                    this.mSendingMsgQueue = this.mSendWaitingMsgQueue;
                    this.mSendWaitingMsgQueue = this.mSendingMsgQueue;
                }
            }
            else
            {
                while (this.mSendingMsgQueue.Count > 0)
                {
                    NetMsg msg = this.mSendingMsgQueue.Dequeue();
                    try
                    {
                        // 拼接TLV
                        MemoryStream stream = new MemoryStream();
//                        TProtocol proto = new TBinaryProtocol(new TStreamTransport(stream, stream));
                        // 类型
//                        proto.WriteI32(msg.Type);
                        // 长度
//                        proto.WriteI32(msg.Length);
                        // 值
                        stream.Write(msg.Data, 0, msg.Length);
                        byte[] data = stream.ToArray();
                        this.mSocket.Send(data);
                        stream.Close();
                    }
                    catch (System.Exception e)
                    {
                        Disconnect();
                    }
                }
            }
        }
        this.mSendingMsgQueue.Clear();
        this.mSendWaitingMsgQueue.Clear();
        Debug.Log("NetMgr Send Thread is over!");
    }

    void Receive()
    {
        this.mRecvStream = new BufferedStream(new NetworkStream(this.mSocket), 4096);
        while (this.mIsRunning)
        {
            try
            {
//                TProtocol proto = new TBinaryProtocol(new TStreamTransport(this.mRecvStream, this.mRecvStream));
//                int type = proto.ReadI32();
//                int len = proto.ReadI32();
//                byte[] data = new byte[len];
//                this.mRecvStream.Read(data, 0, len);
//                NetMsg msg = new NetMsg(type, data);
                // 丢到主线程，由主线程交由Lua解析
//                SendMsgInMainThread(msg);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                Disconnect();
            }
        }
    }

    void SendMsgInMainThread(NetMsg msg)
    {
        lock (this.mRecvLock)
        {
            this.mRecvWaitingMsgQueue.Enqueue(msg);
        }
    }

    void Update()
    {
        if (this.mRecvingMsgQueue.Count == 0)
        {
            lock (this.mRecvLock)
            {
                if (this.mRecvWaitingMsgQueue.Count > 0)
                {
                    Queue<NetMsg> temp = this.mRecvingMsgQueue;
                    this.mRecvingMsgQueue = this.mRecvWaitingMsgQueue;
                    this.mRecvWaitingMsgQueue = temp;
                }
            }
        }
        else
        {
            while (this.mRecvingMsgQueue.Count > 0)
            {
                NetMsg msg = this.mRecvingMsgQueue.Dequeue();
                // 交由Lua解析并处理逻辑
//                LuaMgr.Instance().CallTableFunction("NetMsgMgr", "HandleNetMsg", msg.Type, msg.Data);
            }
        }
    }
}
