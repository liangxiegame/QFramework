using System;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 脚本工具
    /// </summary>
    public class ScriptKit
    {
        public static IScript Script { get; set; }

        /// <summary>
        /// 调用静态方法
        /// </summary>
        public static void CallStaticMethod(string typeOrFileName, string methodName, params object[] args)
        {
            Script.CallStaticMethod(typeOrFileName, methodName, args);
        }

        public static void LoadScript(Action action)
        {
            Script.LoadScript(action);
        }

        /// <summary>
        /// 销毁实例
        /// </summary>
        public static void Dispose()
        {
            if (Script != null)
            {
                Script.Dispose();
                Script = null;
            }
            else
            {
                
            }
        }

        public const string PACKAGE_PATH_IN_ASSETS = "QFramework/Scripting/ScriptKit";
    }
}