using System;

namespace QF.GraphDesigner
{
    public interface IDebugLogger
    {
        void Log(string message);
        void LogException(Exception ex);
    }
}