using System;

namespace QFramework
{
    /// <summary>
    /// 某一个脚本类型
    /// </summary>
    public interface IScript : IDisposable
    {
        void CallStaticMethod(string typeOrFileName, string methodName,params object[] args);
        void LoadScript(Action action);
    }
}