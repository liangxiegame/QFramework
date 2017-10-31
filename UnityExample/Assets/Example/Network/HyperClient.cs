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
using FlexiFramework.Networking;
using UnityEngine;
using UnityEngine.UI;

public class HyperClient : MonoBehaviour
{
    public string ip;
    public int port;
    public Text output;
    public InputField input;
    private ISocketClient client;

    private void Start()
    {
        //client = FlexiSocket.Create(ip, port, Protocol.StringTerminatedBy("\n", Encoding.UTF8));
        client = FlexiSocket.Create(ip, port, Protocols.TotalLengthPrefix);
        client.Connected += ClientOnConnected;
        client.Disconnected += ClientOnDisconnected;
        client.ReceivedAsString += ClientOnReceivedAsString;
        client.Sent += ClientOnSent;
    }

    private void ClientOnReceivedAsString(bool success, Exception exception, SocketError error, string message)
    {
        Debug.Log("Receiving result: " + success);
        if (success)
            output.text += string.Format("<color=red>Server:</color> {0}\r\n\r\n", message);
        else
        {
            if (exception != null) Debug.LogException(exception);
            Debug.LogError(error);
        }
    }

    private void ClientOnDisconnected(bool success, Exception exception)
    {
        Debug.Log("Disconnecting result: " + success);
        if (!success)
            if (exception != null) Debug.LogException(exception);
    }

    private void ClientOnSent(bool success, Exception exception, SocketError error)
    {
        Debug.Log("Sending result: " + success);
        if (success)
            output.text += string.Format("<color=green>Client:</color> {0}\r\n\r\n", input.text);
        else
        {
            if (exception != null) Debug.LogException(exception);
            Debug.LogError(error);
        }
    }

    private void ClientOnConnected(bool success, Exception exception)
    {
        Debug.Log("Connecting result: " + success);
        if (success)
            StartCoroutine(client.ReceiveLoop());
        else
        {
            if (exception != null) Debug.LogException(exception);
        }
    }

    private void OnDestroy()
    {
        client.Close();
        client = null;
    }

    public void Send()
    {
        if (!string.IsNullOrEmpty(input.text) && client.IsConnected)
        {
            StartCoroutine(client.SendAsync(input.text));
        }
    }

    public void Connect()
    {
        StartCoroutine(client.ConnectAsync());
    }

    public void Disconnect()
    {
        StartCoroutine(client.DisconnectAsync());
    }
}