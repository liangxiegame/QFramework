/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Text;

namespace QFramework
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    [ClassAPI("02.LogKit", "LogKit", 4)]
    [APIDescriptionCN("简单的日志工具")]
    [APIDescriptionEN("Simple Log ToolKit")]
#endif
    public class LogKit
    {
#if UNITY_EDITOR
        // v1 No.157
        [MethodAPI]
        [APIDescriptionCN("Debug.Log & Debug.LogFormat")]
        [APIDescriptionEN("Debug.Log & Debug.LogFormat")]
        [APIExampleCode(@"
LogKit.I(""Hello LogKit"");
// Hello LogKit
LogKit.I(""Hello LogKit {0}{1}"",1,2);
// Hello LogKit 12
""Hello LogKit FluentAPI"".LogInfo();
// Hello LogKit FluentAPI
")]
#endif
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
        
#if UNITY_EDITOR
        // v1 No.158
        [MethodAPI]
        [APIDescriptionCN("Debug.LogWarning & Debug.LogWaringFormat")]
        [APIDescriptionEN("Debug.LogWarning & Debug.LogWarningFormat")]
        [APIExampleCode(@"
LogKit.E(""Hello LogKit"");
// Hello LogKit
LogKit.E(""Hello LogKit {0}{1}"",1,2);
// Hello LogKit 12
""Hello LogKit FluentAPI"".LogError();
// Hello LogKit FluentAPI
")]
#endif
        
        public static void W(object msg,params object[] args)
        {
            if (mLogLevel < LogLevel.Warning)
            {
                return;
            }

            if (args == null || args.Length == 0)
            {

                Debug.LogWarning(msg);
            }
            else
            {
                Debug.LogWarningFormat(msg.ToString(), args);
            }
        }
        
        
#if UNITY_EDITOR
        // v1 No.159
        [MethodAPI]
        [APIDescriptionCN("Debug.LogError & Debug.LogErrorFormat")]
        [APIDescriptionEN("Debug.LogError & Debug.LogErrorFormat")]
        [APIExampleCode(@"
LogKit.E(""Hello LogKit"");
// Hello LogKit
LogKit.E(""Hello LogKit {0}{1}"",1,2);
// Hello LogKit 12
""Hello LogKit FluentAPI"".LogError();
// Hello LogKit FluentAPI
")]
#endif
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


        
#if UNITY_EDITOR
        // v1 No.160
        [MethodAPI]
        [APIDescriptionCN("Debug.LogException")]
        [APIDescriptionEN("Debug.LogException")]
        [APIExampleCode(@"
LogKit.E(""Hello LogKit"");
// Hello LogKit
LogKit.E(""Hello LogKit {0}{1}"",1,2);
// Hello LogKit 12
""Hello LogKit FluentAPI"".LogError();
// Hello LogKit FluentAPI
")]
#endif
        public static void E(Exception e)
        {
            if (mLogLevel < LogLevel.Exception)
            {
                return;
            }

            Debug.LogException(e);
        }
        
        
#if UNITY_EDITOR
        // v1 No.161
        [MethodAPI]
        [APIDescriptionCN("获得 StringBuilder 用来拼接日志")]
        [APIDescriptionEN("get stringBuilder for generate log string")]
        [APIExampleCode(@"
LogKit.Builder()
    .Append(""Hello"")
    .Append("" LogKit"")
    .ToString()
    .LogInfo();
// Hello LogKit
")]
#endif
        public static StringBuilder Builder()
        {
            return new StringBuilder();
        }
        

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

#if UNITY_EDITOR
        // v1 No.162
        [PropertyAPI]
        [APIDescriptionCN("日志等级设置")]
        [APIDescriptionEN("log level")]
        [APIExampleCode(@"
LogKit.Level = LogKit.LogLevel.None;
LogKit.I(""LogKit""); // no output
LogKit.Level = LogKit.LogLevel.Exception;
LogKit.Level = LogKit.LogLevel.Error;
LogKit.Level = LogKit.LogLevel.Warning;
LogKit.Level = LogKit.LogLevel.Normal;
LogKit.I(""LogKit""); 
// LogKit
LogKit.Level = LogKit.LogLevel.Max;
")]
#endif
        public static LogLevel Level
        {
            get => mLogLevel;
            set => mLogLevel = value;
        }
    }

    public static class LogKitExtension
    {
        public static StringBuilder GreenColor(this StringBuilder self, Action<StringBuilder> childContent)
        {
            self.Append("<color=green>");
            childContent?.Invoke(self);
            self.Append("</color>");
            return self;
        }
        
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