// ----------------------------------------------------------------------------
// <copyright file="PhotonPing.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// This file includes various PhotonPing implementations for different APIs,
// platforms and protocols.
// The RegionPinger class is the instance which selects the Ping implementation
// to use.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Realtime
{
    using System;
    using System.Collections;
    using System.Threading;

    #if NETFX_CORE
    using System.Diagnostics;
    using Windows.Foundation;
    using Windows.Networking;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    #endif

    #if !NO_SOCKET && !NETFX_CORE
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Sockets;
    #endif

    #if UNITY_WEBGL
    // import WWW class
    using UnityEngine;
    #endif

    /// <summary>
    /// Abstract implementation of PhotonPing, ase for pinging servers to find the "Best Region".
    /// </summary>
    public abstract class PhotonPing : IDisposable
    {
        public string DebugString = "";
        
        public bool Successful;

        protected internal bool GotResult;

        protected internal int PingLength = 13;

        protected internal byte[] PingBytes = new byte[] { 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x7d, 0x00 };

        protected internal byte PingId;

        private static readonly System.Random RandomIdProvider = new System.Random();

        public virtual bool StartPing(string ip)
        {
            throw new NotImplementedException();
        }

        public virtual bool Done()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        protected internal void Init()
        {
            this.GotResult = false;
            this.Successful = false;
            this.PingId = (byte)(RandomIdProvider.Next(255));
        }
    }


    #if !NETFX_CORE && !NO_SOCKET
    /// <summary>Uses C# Socket class from System.Net.Sockets (as Unity usually does).</summary>
    /// <remarks>Incompatible with Windows 8 Store/Phone API.</remarks>
    public class PingMono : PhotonPing
    {
        private Socket sock;

        /// <summary>
        /// Sends a "Photon Ping" to a server.
        /// </summary>
        /// <param name="ip">Address in IPv4 or IPv6 format. An address containing a '.' will be interpreted as IPv4.</param>
        /// <returns>True if the Photon Ping could be sent.</returns>
        public override bool StartPing(string ip)
        {
            this.Init();

            try
            {
                if (this.sock == null)
                {
                    if (ip.Contains("."))
                    {
                        this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    }
                    else
                    {
                        this.sock = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                    }

                    this.sock.ReceiveTimeout = 5000;
                    int port = (RegionHandler.PortToPingOverride != 0) ? RegionHandler.PortToPingOverride : 5055;
                    this.sock.Connect(ip, port);
                }


                this.PingBytes[this.PingBytes.Length - 1] = this.PingId;
                this.sock.Send(this.PingBytes);
                this.PingBytes[this.PingBytes.Length - 1] = (byte)(this.PingId+1);  // this buffer is re-used for the result/receive. invalidate the result now.
            }
            catch (Exception e)
            {
                this.sock = null;
                Console.WriteLine(e);
            }

            return false;
        }

        public override bool Done()
        {
            if (this.GotResult || this.sock == null)
            {
                return true;    // this just indicates the ping is no longer waiting. this.Successful value defines if the roundtrip completed
            }

            int read = 0;
            try
            {
                if (!this.sock.Poll(0, SelectMode.SelectRead))
                {
                    return false;
                }

                read = this.sock.Receive(this.PingBytes, SocketFlags.None);
            }
            catch (Exception ex)
            {
                if (this.sock != null)
                {
                    this.sock.Close();
                    this.sock = null;
                }
                this.DebugString += " Exception of socket! " + ex.GetType() + " ";
                return true;    // this just indicates the ping is no longer waiting. this.Successful value defines if the roundtrip completed
            }

            bool replyMatch = this.PingBytes[this.PingBytes.Length - 1] == this.PingId && read == this.PingLength;
            if (!replyMatch)
            {
                this.DebugString += " ReplyMatch is false! ";
            }


            this.Successful = replyMatch;
            this.GotResult = true;
            return true;
        }

        public override void Dispose()
        {
            try
            {
                this.sock.Close();
            }
            catch
            {
            }

            this.sock = null;
        }

    }
    #endif


    #if NETFX_CORE
    /// <summary>Windows store API implementation of PhotonPing, based on DatagramSocket for UDP.</summary>
    public class PingWindowsStore : PhotonPing
    {
        private DatagramSocket sock;
        private readonly object syncer = new object();

        public override bool StartPing(string host)
        {
            lock (this.syncer)
            {
                this.Init();

                int port = (RegionHandler.PortToPingOverride != 0) ? RegionHandler.PortToPingOverride : 5055;
                EndpointPair endPoint = new EndpointPair(null, string.Empty, new HostName(host), port.ToString());
                this.sock = new DatagramSocket();
                this.sock.MessageReceived += this.OnMessageReceived;

                IAsyncAction result = this.sock.ConnectAsync(endPoint);
                result.Completed = this.OnConnected;
                this.DebugString += " End StartPing";
                return true;
            }
        }

        public override bool Done()
        {
            lock (this.syncer)
            {
                return this.GotResult || this.sock == null; // this just indicates the ping is no longer waiting. this.Successful value defines if the roundtrip completed
            }
        }

        public override void Dispose()
        {
            lock (this.syncer)
            {
                this.sock = null;
            }
        }

        private void OnConnected(IAsyncAction asyncinfo, AsyncStatus asyncstatus)
        {
            lock (this.syncer)
            {
                if (asyncinfo.AsTask().IsCompleted && !asyncinfo.AsTask().IsFaulted && this.sock != null && this.sock.Information.RemoteAddress != null)
                {
                    this.PingBytes[this.PingBytes.Length - 1] = this.PingId;

                    DataWriter writer;
                    writer = new DataWriter(this.sock.OutputStream);
                    writer.WriteBytes(this.PingBytes);
                    DataWriterStoreOperation res = writer.StoreAsync();
                    res.AsTask().Wait(100);

                    this.PingBytes[this.PingBytes.Length - 1] = (byte)(this.PingId + 1); // this buffer is re-used for the result/receive. invalidate the result now.

                    writer.DetachStream();
                    writer.Dispose();
                }
                else
                {
                    this.sock = null; // will cause Done() to return true but this.Successful defines if the roundtrip completed
                }
            }
        }

        private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            lock (this.syncer)
            {
                DataReader reader = null;
                try
                {
                    reader = args.GetDataReader();
                    uint receivedByteCount = reader.UnconsumedBufferLength;
                    if (receivedByteCount > 0)
                    {
                        byte[] resultBytes = new byte[receivedByteCount];
                        reader.ReadBytes(resultBytes);

                        //TODO: check result bytes!


                        this.Successful = receivedByteCount == this.PingLength && resultBytes[resultBytes.Length - 1] == this.PingId;
                        this.GotResult = true;
                    }
                }
                catch
                {
                    // TODO: handle error
                }
            }
        }
    }
    #endif


    #if NATIVE_SOCKETS
	/// <summary>Abstract base class to provide proper resource management for the below native ping implementations</summary>
	public abstract class PingNative : PhotonPing
	{
		// Native socket states - according to EnetConnect.h state definitions
		protected enum NativeSocketState : byte
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2,
			ConnectionError = 3,
			SendError = 4,
			ReceiveError = 5,
			Disconnecting = 6
		}

		protected IntPtr pConnectionHandler = IntPtr.Zero;

		~PingNative()
		{
			Dispose();
		}
	}

    /// <summary>Uses dynamic linked native Photon socket library via DllImport("PhotonSocketPlugin") attribute (as done by Unity Android and Unity PS3).</summary>
    public class PingNativeDynamic : PingNative
    {
        public override bool StartPing(string ip)
        {
            lock (SocketUdpNativeDynamic.syncer)
            {
                base.Init();

				if(pConnectionHandler == IntPtr.Zero)
				{
					pConnectionHandler = SocketUdpNativeDynamic.egconnect(ip);
					SocketUdpNativeDynamic.egservice(pConnectionHandler);
					byte state = SocketUdpNativeDynamic.eggetState(pConnectionHandler);
					while (state == (byte) NativeSocketState.Connecting)
					{
						SocketUdpNativeDynamic.egservice(pConnectionHandler);
						state = SocketUdpNativeDynamic.eggetState(pConnectionHandler);
					}
				}

                PingBytes[PingBytes.Length - 1] = PingId;
                SocketUdpNativeDynamic.egsend(pConnectionHandler, PingBytes, PingBytes.Length);
                SocketUdpNativeDynamic.egservice(pConnectionHandler);

                PingBytes[PingBytes.Length - 1] = (byte) (PingId - 1);
                return true;
            }
        }

        public override bool Done()
        {
            lock (SocketUdpNativeDynamic.syncer)
            {
                if (this.GotResult || pConnectionHandler == IntPtr.Zero)
                {
                    return true;
                }

                int available = SocketUdpNativeDynamic.egservice(pConnectionHandler);
                if (available < PingLength)
                {
                    return false;
                }

                int pingBytesLength = PingBytes.Length;
                int bytesInRemainginDatagrams = SocketUdpNativeDynamic.egread(pConnectionHandler, PingBytes, ref pingBytesLength);
                this.Successful = (PingBytes != null && PingBytes[PingBytes.Length - 1] == PingId);
                //Debug.Log("Successful: " + this.Successful + " bytesInRemainginDatagrams: " + bytesInRemainginDatagrams + " PingId: " + PingId);

                this.GotResult = true;
                return true;
            }
        }

        public override void Dispose()
        {
            lock (SocketUdpNativeDynamic.syncer)
            {
                if (this.pConnectionHandler != IntPtr.Zero)
                    SocketUdpNativeDynamic.egdisconnect(this.pConnectionHandler);
                this.pConnectionHandler = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    #if NATIVE_SOCKETS && NATIVE_SOCKETS_STATIC
    /// <summary>Uses static linked native Photon socket library via DllImport("__Internal") attribute (as done by Unity iOS and Unity Switch).</summary>
    public class PingNativeStatic : PingNative
    {
		public override bool StartPing(string ip)
        {
            base.Init();

            lock (SocketUdpNativeStatic.syncer)
			{
				if(pConnectionHandler == IntPtr.Zero)
				{
					pConnectionHandler = SocketUdpNativeStatic.egconnect(ip);
					SocketUdpNativeStatic.egservice(pConnectionHandler);
					byte state = SocketUdpNativeStatic.eggetState(pConnectionHandler);
					while (state == (byte) NativeSocketState.Connecting)
					{
						SocketUdpNativeStatic.egservice(pConnectionHandler);
						state = SocketUdpNativeStatic.eggetState(pConnectionHandler);
						Thread.Sleep(0); // suspending execution for a moment is critical on Switch for the OS to update the socket
					}
				}

                PingBytes[PingBytes.Length - 1] = PingId;
                SocketUdpNativeStatic.egsend(pConnectionHandler, PingBytes, PingBytes.Length);
                SocketUdpNativeStatic.egservice(pConnectionHandler);

                PingBytes[PingBytes.Length - 1] = (byte) (PingId - 1);
                return true;
            }
        }

        public override bool Done()
        {
            lock (SocketUdpNativeStatic.syncer)
            {
                if (this.GotResult || pConnectionHandler == IntPtr.Zero)
                {
                    return true;
                }

                int available = SocketUdpNativeStatic.egservice(pConnectionHandler);
                if (available < PingLength)
                {
                    return false;
                }

                int pingBytesLength = PingBytes.Length;
                int bytesInRemainginDatagrams = SocketUdpNativeStatic.egread(pConnectionHandler, PingBytes, ref pingBytesLength);
                this.Successful = (PingBytes != null && PingBytes[PingBytes.Length - 1] == PingId);
                //Debug.Log("Successful: " + this.Successful + " bytesInRemainginDatagrams: " + bytesInRemainginDatagrams + " PingId: " + PingId);

                this.GotResult = true;
                return true;
            }
        }

        public override void Dispose()
        {
            lock (SocketUdpNativeStatic.syncer)
            {
                if (pConnectionHandler != IntPtr.Zero)
                    SocketUdpNativeStatic.egdisconnect(pConnectionHandler);
                pConnectionHandler = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }
    #endif
    #endif


    #if UNITY_WEBGL
    public class PingHttp : PhotonPing
    {
        private WWW webRequest;

        public override bool StartPing(string address)
        {
            base.Init();

            address = "https://" + address + "/photon/m/?ping&r=" + UnityEngine.Random.Range(0, 10000);
            this.webRequest = new WWW(address);
            return true;
        }

        public override bool Done()
        {
            if (this.webRequest.isDone)
            {
                Successful = true;
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            this.webRequest.Dispose();
        }
    }
    #endif
}