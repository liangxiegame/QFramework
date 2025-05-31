/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 *
 ****************************************************************************/

using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
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

        public static void W(object msg, params object[] args)
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

#if UNITY_EDITOR
    // * 参考: https://zhuanlan.zhihu.com/p/92291084
    // 感谢 https://github.com/clksaaa 提供的 issue 反馈 和 解决方案
    public static class OpenAssetLogLine
    {
        private static bool m_hasForceMono = false;

        // 处理asset打开的callback函数
        [OnOpenAsset(-1)]
        static bool OnOpenAsset(int instance, int line)
        {
            if (m_hasForceMono) return false;

            // 自定义函数，用来获取log中的stacktrace，定义在后面。
            string stack_trace = GetStackTrace();
            // 通过stacktrace来定位是否是自定义的log，log中有LogKit/LogKit.cs，很好识别
            if (!string.IsNullOrEmpty(stack_trace) && stack_trace.Contains("LogKit/LogKit.cs"))
            {
                // 正则匹配at xxx，在第几行
                Match matches = Regex.Match(stack_trace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                string pathline = "";
                while (matches.Success)
                {
                    pathline = matches.Groups[1].Value;

                    // 找到不是我们自定义log文件的那行，重新整理文件路径，手动打开
                    if (!pathline.Contains("LogKit/LogKit.cs") && !string.IsNullOrEmpty(pathline))
                    {
                        int split_index = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, split_index);
                        line = Convert.ToInt32(pathline.Substring(split_index + 1));
                        m_hasForceMono = true;
                        //方式一
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(path), line);
                        m_hasForceMono = false;
                        //方式二
                        //string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        // fullpath = fullpath + path;
                        //  UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullpath.Replace('/', '\\'), line);
                        return true;
                    }

                    matches = matches.NextMatch();
                }

                return true;
            }

            return false;
        }

        static string GetStackTrace()
        {
// 找到类UnityEditor.ConsoleWindow
            var type_console_window = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
// 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
            var filedInfo =
                type_console_window.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
// 获取ms_ConsoleWindow的值
            var ConsoleWindowInstance = filedInfo.GetValue(null);
            if (ConsoleWindowInstance != null)
            {
                if ((object)EditorWindow.focusedWindow == ConsoleWindowInstance)
                {
// 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
                    filedInfo = type_console_window.GetField("m_ActiveText",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    string activeText = filedInfo.GetValue(ConsoleWindowInstance).ToString();
                    return activeText;
                }
            }

            return null;
        }
    }
#endif
}