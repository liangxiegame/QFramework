/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    using System;
    using UnityEngine;

    public class LogKit
    {
        public enum LogLevel
        {
            None = 0,
            Exception = 1,
            Error = 2,
            Warning = 3,
            Normal = 4,
            Max = 5,
        }

        private static LogLevel mLogLevel = LogLevel.Normal;

        public static LogLevel Level
        {
            get { return mLogLevel; }
            set { mLogLevel = value; }
        }

        public static void I(object msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Normal)
            {
                return;
            }

            if (args == null || args.Length == 0)
            {
                Debug.Log(msg);
            }
            else
            {
                Debug.LogFormat(msg.ToString(), args);
            }
        }

        public static void E(Exception e)
        {
            if (mLogLevel < LogLevel.Exception)
            {
                return;
            }

            Debug.LogException(e);
        }

        public static void E(object msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Error)
            {
                return;
            }

            if (args == null || args.Length == 0)
            {
                Debug.LogError(msg);
            }
            else
            {
                Debug.LogError(string.Format(msg.ToString(), args));
            }
        }

        public static void W(object msg)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }

            Debug.LogWarning(msg);
        }

        public static void W(string msg, params object[] args)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }

            Debug.LogWarning(string.Format(msg, args));
        }
    }

    public static class LogKitExtension
    {
        public static void LogInfo<T>(this T selfMsg)
        {
            LogKit.I(selfMsg);
        }

        public static void LogWarning<T>(this T selfMsg)
        {
            LogKit.W(selfMsg);
        }

        public static void LogError<T>(this T selfMsg)
        {
            LogKit.E(selfMsg);
        }

        public static void LogException(this Exception selfExp)
        {
            LogKit.E(selfExp);
        }
    }
}