/****************************************************************************
 * Copyright (c) 2017 ~ 2018.7 liangxie
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

#if UNITY_IPHONE
using System.Net.NetworkInformation;
using System.Net.Sockets;
#endif
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace QF
{
    /// <summary>
    /// some net work util
    /// </summary>
    public static class Network
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>IP string</returns>
        public static string GetAddressIP()
        {
            var AddressIP = "";
#if !UNITY_WEBGL
#if UNITY_3 || UNITY_4 || UNITY_5 || UNITY_2017 || UNITY_2018_0 || UNITY_2018_1
            AddressIP = UnityEngine.Network.player.ipAddress;
#else
            //获取本地的IP地址  
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
#endif
#endif
            
#if UNITY_IPHONE
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces(); ;  
            foreach (NetworkInterface adapter in adapters)  
            {  
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))  
                {  
                    UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;  
                    if (uniCast.Count > 0)  
                    {  
                        foreach (UnicastIPAddressInformation uni in uniCast)  
                        {  
                            //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4  
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                AddressIP = uni.Address.ToString();
                            }
                        }  
                    }  
                }  
            }  
#endif
            return AddressIP;
        }

        public static bool IsReachable
        {
            get { return Application.internetReachability != NetworkReachability.NotReachable; }
        }
    }
}