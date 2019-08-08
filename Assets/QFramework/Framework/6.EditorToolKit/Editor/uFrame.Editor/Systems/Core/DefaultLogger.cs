using System;
using System.IO;

namespace QF.GraphDesigner
{
    public class DefaultLogger : IDebugLogger
    {
        public void Log(string message)
        {
            File.AppendAllText("uframe-log.txt", message + "\r\n\r\n");
        }

        public void LogException(Exception ex)
        {
            File.AppendAllText("uframe-log.txt", ex.Message + "\r\n\r\n" + ex.StackTrace +"\r\n\r\n");
        }
    }
}
