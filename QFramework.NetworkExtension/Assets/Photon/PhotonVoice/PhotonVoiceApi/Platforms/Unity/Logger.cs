using UnityEngine;

namespace Photon.Voice.Unity
{
    public class Logger : ILogger
    {
        public void LogError(string fmt, params object[] args) { Debug.LogErrorFormat(fmt, args); }
        public void LogWarning(string fmt, params object[] args) { Debug.LogWarningFormat(fmt, args); }
        public void LogInfo(string fmt, params object[] args) { Debug.LogFormat(fmt, args); }
        public void LogDebug(string fmt, params object[] args) { Debug.LogFormat(fmt, args); }
    }
}
