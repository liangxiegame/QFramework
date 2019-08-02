using System;

namespace QFramework.GraphDesigner
{
    public interface IDebugLogger
    {
        void Log(string message);
        void LogException(Exception ex);
    }
}