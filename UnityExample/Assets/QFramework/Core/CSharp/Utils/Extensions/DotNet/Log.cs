/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

#if UNITY_5
    using UnityEngine;
#endif

    public enum LogLevel
    {
        None = 0,
        Exception = 1,
        Error = 2,
        Warning = 3,
        Normal = 4,
        Max = 5,
    }

    public static class Log
    {

        public static void LogInfo(this object selfMsg)
        {
            I(selfMsg);
        }

        public static void LogWarning(this object selfMsg)
        {
            W(selfMsg);
        }

        public static void LogError(this object selfMsg)
        {
            E(selfMsg);
        }

        public static void LogException(this Exception selfExp)
        {
            E(selfExp);
        }

        private static LogLevel mLogLevel = LogLevel.Normal;

        public static LogLevel Level
        {
            get { return mLogLevel; }
            set { mLogLevel = value; }
        }



        public static void I(object msg)
        {
            if (mLogLevel < LogLevel.Normal)
            {
                return;
            }
#if UNITY_5
            Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        public static void I(string msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Normal)
            {
                return;
            }
#if UNITY_5
            Debug.Log(string.Format(msg, args));
#else
            Console.WriteLine(msg, args);
#endif
        }

        public static void E(object msg)
        {
            if (mLogLevel < LogLevel.Error)
            {
                return;
            }
#if UNITY_5
            Debug.LogError(msg);
#else
            Console.WriteLine("[Error] {0}", msg);
#endif
        }

        public static void E(Exception e)
        {
            if (mLogLevel < LogLevel.Exception)
            {
                return;
            }
#if UNITY_5
            Debug.LogException(e);
#else
            Console.WriteLine("[Exception] {0}", e);
#endif
        }

        public static void E(string msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Error)
            {
                return;
            }
#if UNITY_5
            Debug.LogError(string.Format(msg, args));
#else
            Console.WriteLine(string.Format("[Error] {0}", msg), args);
#endif
        }

        public static void W(object msg)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }
#if UNITY_5
            Debug.LogWarning(msg);
#else
            Console.WriteLine("[Warning] {0}", msg);
#endif
        }

        public static void W(string msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }

#if UNITY_5
            Debug.LogWarning(string.Format(msg, args));
#else
            Console.WriteLine(string.Format("[Warning] {0}", msg), args);
#endif
        }
    }
}