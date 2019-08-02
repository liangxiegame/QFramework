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

using System.Collections;
using System.Net;

namespace QF
{
    /// <summary>
    /// Socket client
    /// </summary>
    public interface ISocketClient
    {
        /// <summary>
        /// Connected to server callback
        /// </summary>
        event ConnectedCallback Connected;

        /// <summary>
        /// Received message from server callback
        /// </summary>
        event ReceivedCallback Received;

        /// <summary>
        /// Received message from server callback
        /// </summary>
        event ReceivedStringCallback ReceivedAsString;

        /// <summary>
        /// Disconnected from server callback
        /// </summary>
        event DisconnectedCallback Disconnected;

        /// <summary>
        /// Message sent to server callback
        /// </summary>
        event SentCallback Sent;

        /// <summary>
        /// Socket closed callback
        /// </summary>
        event ClosedCallback Closed;

        /// <summary>
        /// Server address
        /// </summary>
        IPAddress Address { get; }

        /// <summary>
        /// Server listening port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// If client is connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Close the client
        /// </summary>
        /// <remarks>
        /// <see cref="Closed"/> will be invoked on main thread if you call this
        /// <para/>
        /// Both sending and receiving will be shutdown
        /// </remarks>
        void Close();

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Connected"/> will be invoked on the async thread if you call this
        /// <para/>
        /// If successful, a receiving loop will be started and <see cref="Received"/>/<see cref="ReceivedAsString"/> will be invoked when a message is received
        /// </remarks>
        void Connect();

        /// <summary>
        /// Connect to server
        /// </summary>
        /// <returns>An async operation</returns>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Connected"/> will be invoked on main thread if you call this
        /// </remarks>
        AsyncConnect ConnectAsync();

        /// <summary>
        /// Receive messages from server
        /// </summary>
        /// <returns>An async operation</returns>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Received"/> will be invoked on main thread if you call this
        /// </remarks>
        AsyncReceive ReceiveAsync();

        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="message">Raw messaged</param>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Sent"/> will be invoked on the async thread if you call this
        /// </remarks>
        void Send(byte[] message);

        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="message">String message</param>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Sent"/> will be invoked on the async thread if you call this
        /// </remarks>
        void Send(string message);

        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="message">Raw messgae</param>
        /// <returns>An async operation</returns>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Sent"/> will be invoked on main thread if you call this
        /// </remarks>
        AsyncSend SendAsync(byte[] message);

        /// <summary>
        /// Send message to server
        /// </summary>
        /// <param name="message">String message</param>
        /// <returns>An async operation</returns>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Sent"/> will be invoked on main thread if you call this
        /// </remarks>
        AsyncSend SendAsync(string message);

        /// <summary>
        /// Disconnect from server
        /// </summary>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Disconnected"/> will be invoked on the async thread if you call this
        /// </remarks>
        void Disconnect();

        /// <summary>
        /// Disconnect from server
        /// </summary>
        /// <returns>An async operation</returns>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Disconnected"/> will be invoked on main thread if you <code>yield</code> this or call <see cref="UnityEngine.MonoBehaviour.StartCoroutine(IEnumerator)"/> with the returned operation
        /// </remarks>
        AsyncDisconnect DisconnectAsync();

        /// <summary>
        /// Keep receiving untill disconnected or failed
        /// </summary>
        /// <returns>An async operation</returns>
        /// <remarks>
        /// This won't block the main thread
        /// <para/>
        /// <see cref="Received"/> and <see cref="ReceivedAsString"/> will be invoked on main thread if you <code>yield</code> this or call <see cref="UnityEngine.MonoBehaviour.StartCoroutine(IEnumerator)"/> with the returned operation
        /// </remarks>
        IEnumerator ReceiveLoop();
    }
}