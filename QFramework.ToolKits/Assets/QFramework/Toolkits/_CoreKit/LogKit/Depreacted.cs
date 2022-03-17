/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    [Obsolete("请使用 LogKit，use LogKit instead", true)]
    public static class Log
    {
        
        [Obsolete("请使用 LogKit.Level，use LogKit.Level instead", true)]
        public static LogKit.LogLevel Level
        {
            get => LogKit.Level;
            set => LogKit.Level = value;
        }

        [Obsolete("请使用 LogKit.I，use LogKit.I instead", true)]
        public static void I(object msg, params object[] args)
        {
            LogKit.I(msg, args);
        }

        [Obsolete("请使用 LogKit.E，use LogKit.E instead", true)]
        public static void E(Exception e)
        {
            LogKit.E(e);
        }

        [Obsolete("请使用 LogKit.E，use LogKit.E instead", true)]
        public static void E(object msg, params object[] args)
        {
            LogKit.E(msg);
        }

        [Obsolete("请使用 LogKit.W，use LogKit.W instead", true)]
        public static void W(object msg)
        {
            LogKit.W(msg);
        }

        [Obsolete("请使用 LogKit.W，use LogKit.W instead", true)]
        public static void W(string msg, params object[] args)
        {
            LogKit.W(msg, args);
        }
    }
}