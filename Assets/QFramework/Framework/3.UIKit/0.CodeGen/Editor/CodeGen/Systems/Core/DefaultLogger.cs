using System.IO;

namespace QFramework.CodeGen
{
    public class DefaultLogger : IDebugLogger
    {
        public void Log(string message)
        {
            File.AppendAllText("uframe-log.txt", message + "\r\n\r\n");
        }
    }
}
