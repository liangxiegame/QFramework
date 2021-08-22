// *************************************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2016 Sean
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *************************************************************************************************
// Project source: https://github.com/theoxuan/FlexiSocket

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace QFramework
{ 
    public sealed class FlexiSocket : ISocketClient, ISocketServer, ISocketClientToken
    {
        private readonly Socket _socket;
        private readonly IProtocol _protocol;

        public int Port { get; private set; }
        public event ClosedCallback Closed;

        #region client

        public IPAddress Address { get; private set; }

        public bool IsConnected { get; private set; }

        public event ConnectedCallback Connected;

        public event ReceivedCallback Received;

        public event ReceivedStringCallback ReceivedAsString;

        public event DisconnectedCallback Disconnected;

        public event SentCallback Sent;

        private FlexiSocket(string ip, int port, IProtocol protocol)
        {
            IPAddress addres;
            if (!IPAddress.TryParse(ip, out addres)) //not ipv4/ipv6
            {
                try
                {
                    var addresses = Dns.GetHostAddresses(ip);
                    addres = addresses[0]; //TODO
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("ip", ex);
                }
            }
           
            Address = addres;
            Port = port;
            _protocol = protocol;
            _socket = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }


        void ISocketClient.Connect()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += ConnectCallback;
            args.UserToken = _socket;
            args.RemoteEndPoint = new IPEndPoint(Address, Port);
            try
            {
                _socket.ConnectAsync(args);
            }
            catch (Exception ex)
            {
                OnConnected(false, ex);
            }
        }

        private void ConnectCallback(object sender, SocketAsyncEventArgs args)
        {
            var socket = (Socket) args.UserToken;
            if (args.SocketError != SocketError.Success)
                OnConnected(false, null);
            else
            {
                StartReceive(null, new StateObject(socket, _protocol));
                OnConnected(true, null);
            }
        }

        AsyncConnect ISocketClient.ConnectAsync()
        {
            var @async = new AsyncConnect(_socket, new IPEndPoint(Address, Port));
            @async.Completed += OnConnected;
            return @async;
        }

        AsyncReceive ISocketClient.ReceiveAsync()
        {
            var @async = new AsyncReceive(_socket, _protocol);
            @async.Completed += OnReceived;
            return @async;
        }

        private void StartReceive(SocketAsyncEventArgs args, StateObject state)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += ReceiveCallback;
                args.UserToken = state;
                args.SetBuffer(state.buffer, 0, state.buffer.Length);
            }
            try
            {
                if (!state.handler.ReceiveAsync(args))
                    ReceiveCallback(null, args);
            }
            catch (Exception ex)
            {
                state.Dispose();
                OnReceived(false, ex, args.SocketError, null);
            }
        }

        private void ReceiveCallback(object sender, SocketAsyncEventArgs args)
        {
            var state = (StateObject) args.UserToken;
            if (args.SocketError != SocketError.Success)
            {
                state.Dispose();
                OnReceived(false, null, args.SocketError, null);
            }
            else if (args.BytesTransferred <= 0)
            {
                state.Dispose();
                Close();
            }
            else
            {
                state.stream.Write(state.buffer, 0, args.BytesTransferred);
                if (!state.protocol.IsComplete(state.stream)) //incompleted
                    StartReceive(args, state);
                else //completed
                {
                    var data = state.protocol.Decode(state.stream);
                    state.Dispose();
                    OnReceived(true, null, args.SocketError, data);
                    StartReceive(null, new StateObject(state.handler, state.protocol));
                }
            }
        }

        public void Send(string message)
        {
            Send(_protocol.Encoding.GetBytes(message));
        }

        public void Send(byte[] message)
        {
            StartSend(new StateObject(_socket, _protocol, message), null);
        }

        private void StartSend(StateObject state, SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.SetBuffer(state.stream.GetBuffer(), 0, (int) state.stream.Length);
                args.Completed += SentCallback;
                args.UserToken = state;
            }

            try
            {
                _socket.SendAsync(args);
            }
            catch (Exception ex)
            {
                state.Dispose();
                OnSent(false, ex, args.SocketError);
            }
        }

        AsyncSend ISocketClient.SendAsync(byte[] message)
        {
            var @async = new AsyncSend(_socket, _protocol, message);
            @async.Completed += OnSent;
            return @async;
        }

        AsyncSend ISocketClient.SendAsync(string message)
        {
            return ((ISocketClient) this).SendAsync(_protocol.Encoding.GetBytes(message));
        }

        private void SentCallback(object sender, SocketAsyncEventArgs args)
        {
            var state = (StateObject) args.UserToken;
            if (args.SocketError != SocketError.Success)
            {
                state.Dispose();
                OnSent(false, null, args.SocketError);
            }
            else
            {
                if (args.BytesTransferred <= 0)
                {
                    state.Dispose();
                    Close();
                }
                else
                {
                    state.stream.Position += args.BytesTransferred;
                    if (state.stream.Position < state.stream.Length) //not finished yet
                        StartSend(state, args);
                    else
                    {
                        state.Dispose();
                        OnSent(true, null, args.SocketError);
                    }
                }
            }
        }

        void ISocketClient.Disconnect()
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += DisconnectCallback;
            try
            {
                _socket.DisconnectAsync(args);
            }
            catch (Exception ex)
            {
                OnDisconnected(false, ex);
            }
        }

        AsyncDisconnect ISocketClient.DisconnectAsync()
        {
            var @async = new AsyncDisconnect(_socket);
            @async.Completed += OnDisconnected;
            return @async;
        }

        private void DisconnectCallback(object sender, SocketAsyncEventArgs args)
        {
            OnDisconnected(true, null);
        }

        IEnumerator ISocketClient.ReceiveLoop()
        {
            while (IsConnected)
            {
                using (var receive = ((ISocketClient) this).ReceiveAsync())
                {
                    yield return receive;
                    if (!receive.IsSuccessful)
                        break;
                }
            }
        }

        #endregion

        #region Token

        public int ID
        {
            get { return _socket.GetHashCode(); }
        }

        #endregion

        #region Server

        private readonly List<ISocketClientToken> _clients = new List<ISocketClientToken>();

        public int Backlog { get; private set; }

        public bool IsListening { get; private set; }

        public bool IPv6 { get; private set; }

        ReadOnlyCollection<ISocketClientToken> ISocketServer.Clients
        {
            get
            {
                lock (_clients)
                    return new ReadOnlyCollection<ISocketClientToken>(_clients);
            }
        }

        public event ClientConnectedCallback ClientConnected;

        public event ReceivedFromClientCallback ReceivedFromClient;

        public event ReceivedStringFromClientCallback ReceivedFromClientAsString;

        public event ClientDisconnectedCallback ClientDisconnected;

        public event SentToClientCallback SentToClient;

        private FlexiSocket(int port, IProtocol protocol, bool ipv6)
        {
            if (ipv6)
            {
                if (!Socket.OSSupportsIPv6)
                    throw new NotSupportedException("IPv6 is not supported on this OS");
            }
            else
            {
                if (!Socket.SupportsIPv4)
                    throw new NotSupportedException("IPv6 is not supported on this OS");
            }
            Port = port;
            _protocol = protocol;
            IPv6 = ipv6;
            _socket = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
        }

        private FlexiSocket(Socket socket, IProtocol protocol)
        {
            _socket = socket;
            _protocol = protocol;
        }

        void ISocketServer.StartListen(int backlog)
        {
            Backlog = backlog;
            IsListening = true;
            _socket.Bind(new IPEndPoint(IPv6 ? IPAddress.IPv6Any : IPAddress.Any, Port));
            _socket.Listen(backlog);
            StartAccept(null);
        }

        void ISocketServer.SendToAll(byte[] message)
        {
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    client.Send(message);
                }
            }
        }

        void ISocketServer.SendToAll(string message)
        {
            ((ISocketServer) this).SendToAll(_protocol.Encoding.GetBytes(message));
        }

        private void StartAccept(SocketAsyncEventArgs args)
        {
            if (args == null)
            {
                args = new SocketAsyncEventArgs();
                args.Completed += AcceptCallback;
            }
            else
            {
                args.AcceptSocket = null;
            }
            _socket.AcceptAsync(args);
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.AcceptSocket != null)
            {
                var client = new FlexiSocket(args.AcceptSocket, _protocol);
                lock (_clients)
                    _clients.Add(client);
                if (ReceivedFromClient != null)
                    client.Received += delegate(bool success, Exception exception, SocketError error, byte[] message)
                    {
                        if (success)
                            ReceivedFromClient(client, message);
                    };
                if (ReceivedFromClientAsString != null)
                    client.ReceivedAsString +=
                        delegate(bool success, Exception exception, SocketError error, string message)
                        {
                            if (success)
                                ReceivedFromClientAsString(client, message);
                        };
                if (ClientDisconnected != null)
                {
                    client.Closed += delegate
                    {
                        lock (_clients)
                            _clients.Remove(client);
                        ClientDisconnected(client);
                    };
                }
                if (SentToClient != null)
                    client.Sent +=
                        delegate(bool success, Exception exception, SocketError error)
                        {
                            SentToClient(success, client);
                        };
                if (ClientConnected != null)
                    ClientConnected(client);
                client.StartReceive(null, new StateObject(client._socket, _protocol));
                StartAccept(args);
            }
        }

        #endregion

        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                lock (_clients)
                {
                    _clients.ForEach(client => client.Close());
                    _clients.Clear();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                OnClosed();
            }
        }

        /// <summary>
        /// Create a client
        /// </summary>
        /// <param name="ip">Server address</param>
        /// <param name="port">Server listening port</param>
        /// <param name="protocol">Protocol</param>
        /// <returns>Created client</returns>
        public static ISocketClient Create(string ip, int port, IProtocol protocol)
        {
            return new FlexiSocket(ip, port, protocol);
        }


        /// <summary>
        /// Create a server
        /// </summary>
        /// <param name="port">Listening port</param>
        /// <param name="protocol">Protocol</param>
        /// <param name="ipv6">Use ipv6</param>
        /// <returns>Created server</returns>
        public static ISocketServer Create(int port, IProtocol protocol, bool ipv6)
        {
            return new FlexiSocket(port, protocol, ipv6);
        }

        private void OnClosed()
        {
            IsConnected = false;
            IsListening = false;
            var handler = Closed;
            if (handler != null) handler();
        }

        private void OnConnected(bool success, Exception exception)
        {
            IsConnected = true;
            var handler = Connected;
            if (handler != null) handler(success, exception);
        }

        private void OnReceived(bool success, Exception exception, SocketError error, byte[] message)
        {
            if (message == null)
                Close();
            else
            {
                var handler = Received;
                if (handler != null) handler(success, exception, error, message);
                var strHandler = ReceivedAsString;
                if (strHandler != null) strHandler(success, exception, error, _protocol.Encoding.GetString(message));
            }
        }

        private void OnDisconnected(bool success, Exception exception)
        {
            IsConnected = false;
            var handler = Disconnected;
            if (handler != null) handler(success, exception);
        }

        private void OnSent(bool success, Exception exception, SocketError error)
        {
            var handler = Sent;
            if (handler != null) handler(success, exception, error);
        }

        private class StateObject : IDisposable
        {
            public readonly Socket handler;
            public readonly MemoryStream stream;
            public readonly IProtocol protocol;
            public readonly byte[] buffer;

            private StateObject(Socket handler, IProtocol protocol, MemoryStream stream, byte[] buffer)
            {
                this.handler = handler;
                this.protocol = protocol;
                this.stream = stream;
                this.buffer = buffer;
            }

            /// <summary>
            /// Receive state
            /// </summary>
            /// <param name="handler"></param>
            /// <param name="protocol"></param>
            public StateObject(Socket handler, IProtocol protocol)
                : this(handler, protocol, new MemoryStream(), new byte[8192])
            {
            }

            /// <summary>
            /// Send state
            /// </summary>
            /// <param name="handler"></param>
            /// <param name="protocol"></param>
            /// <param name="buffer"></param>
            public StateObject(Socket handler, IProtocol protocol, byte[] buffer)
                : this(handler, protocol, null, null)
            {
                var data = protocol.Encode(buffer);
                stream = new MemoryStream(data, 0, data.Length, false, true);
            }

            #region Implementation of IDisposable

            public void Dispose()
            {
                stream.Dispose();
            }

            #endregion
        }
    }
}