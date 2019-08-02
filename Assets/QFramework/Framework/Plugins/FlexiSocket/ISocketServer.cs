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

using System.Collections.ObjectModel;
using QF.MVVM;

namespace QF
{
    /// <summary>
    /// Socket server
    /// </summary>
    public interface ISocketServer
    {
        /// <summary>
        /// Client accepted callback
        /// </summary>
        event ClientConnectedCallback ClientConnected;

        /// <summary>
        /// Received message from client callback
        /// </summary>
        event ReceivedFromClientCallback ReceivedFromClient;

        /// <summary>
        /// Received message from client callback
        /// </summary>
        event ReceivedStringFromClientCallback ReceivedFromClientAsString;

        /// <summary>
        /// Client disconnected callback
        /// </summary>
        event ClientDisconnectedCallback ClientDisconnected;

        /// <summary>
        /// Sent to client
        /// </summary>
        event SentToClientCallback SentToClient;

        /// <summary>
        /// Socket closed callback
        /// </summary>
        event ClosedCallback Closed;

        /// <summary>
        /// Max connection
        /// </summary>
        int Backlog { get; }

        /// <summary>
        /// Listening port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// If server is listenning for connections
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// If ipv6 is enabled
        /// </summary>
        bool IPv6 { get; }

        /// <summary>
        /// Connected clients
        /// </summary>
        ReadOnlyCollection<ISocketClientToken> Clients { get; }

        /// <summary>
        /// Close the server
        /// </summary>
        /// <remarks>
        /// <see cref="Closed"/> will be invoked on main thread if you call this
        /// <para/>
        /// Both sending and receiving will be shutdown
        /// </remarks>
        void Close();

        /// <summary>
        /// Start listening
        /// </summary>
        /// <param name="backlog">Max connection</param>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="ClientConnected"/> will be invoked on main thread when a client is accepted
        /// <para/>
        /// <see cref="ReceivedFromClient"/> and <see cref="ReceivedFromClientAsString"/>  will be invoked on main thread when a message from a client is received
        /// <para/>
        /// <see cref="ClientDisconnected"/> will be invoked on main thread when a client is disconnected
        /// <para/>
        /// </remarks>
        void StartListen(int backlog);

        /// <summary>
        /// Send message to all connected clients
        /// </summary>
        /// <param name="message">Message</param>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="SentToClient"/> will not be invoked when you call this
        /// </remarks>
        void SendToAll(byte[] message);

        /// <summary>
        /// Send message to all connected clients
        /// </summary>
        /// <param name="message">Message</param>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="SentToClient"/> will not be invoked when you call this
        /// </remarks>
        void SendToAll(string message);
    }
}