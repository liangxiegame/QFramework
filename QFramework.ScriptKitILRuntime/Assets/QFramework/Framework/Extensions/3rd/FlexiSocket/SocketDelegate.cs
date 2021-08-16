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
using System.Net.Sockets;

namespace QFramework
{ 
    /// <summary>
    /// Socket closed callback
    /// </summary>
    public delegate void ClosedCallback();

    /// <summary>
    /// Socket connected callback
    /// </summary>
    /// <param name="success">Connecting result</param>
    /// <param name="exception">Socket exception</param>
    public delegate void ConnectedCallback(bool success, Exception exception);

    /// <summary>
    /// Socket received message callback
    /// </summary>
    /// <param name="success">Receiving result</param>
    /// <param name="exception">Socket exception</param>
    /// <param name="error">Socket error</param>
    /// <param name="message">Received message</param>
    public delegate void ReceivedCallback(bool success, Exception exception, SocketError error, byte[] message);

    /// <summary>
    /// Socket received message callback
    /// </summary>
    /// <param name="success">Receiving result</param>
    /// <param name="exception">Socket exception</param>
    /// <param name="error">Socket error</param>
    /// <param name="message">Received message</param>
    public delegate void ReceivedStringCallback(bool success, Exception exception, SocketError error, string message);

    /// <summary>
    /// Socket disconnected callback
    /// </summary>
    /// <param name="success">Disconnecting result</param>
    /// <param name="exception">Socket exception</param>
    public delegate void DisconnectedCallback(bool success, Exception exception);

    /// <summary>
    /// Socket message sent callback
    /// </summary>
    /// <param name="success">Sending result</param>
    /// <param name="exception">Socket exception</param>
    /// <param name="error">Socket error</param>
    public delegate void SentCallback(bool success, Exception exception, SocketError error);

    /// <summary>
    /// Client connected callback
    /// </summary>
    /// <param name="client">Connected client</param>
    public delegate void ClientConnectedCallback(ISocketClientToken client);

    /// <summary>
    /// Received from client callback
    /// </summary>
    /// <param name="client">Source client</param>
    /// <param name="message">Message</param>
    public delegate void ReceivedFromClientCallback(ISocketClientToken client, byte[] message);

    /// <summary>
    /// Received from client callback
    /// </summary>
    /// <param name="client">Source client</param>
    /// <param name="message">Message</param>
    public delegate void ReceivedStringFromClientCallback(ISocketClientToken client, string message);

    /// <summary>
    /// Client disconnected callback
    /// </summary>
    /// <param name="client">Disconnected client</param>
    public delegate void ClientDisconnectedCallback(ISocketClientToken client);

    /// <summary>
    /// Message sent(to client) callback
    /// </summary>
    /// <param name="success">Sending result</param>
    /// <param name="client">Target client</param>
    public delegate void SentToClientCallback(bool success, ISocketClientToken client);

}