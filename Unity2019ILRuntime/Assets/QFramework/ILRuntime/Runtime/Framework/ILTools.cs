using System;
using System.IO;

namespace QFramework
{
    public static class ILTools
    {
        public static void ForeachFiles(this DirectoryInfo self, Func<FileInfo,bool> breakFunc = null)
        {
            foreachFiles(self, breakFunc);
        }

        private static bool foreachFiles(DirectoryInfo dir, Func<FileInfo, bool> breakFunc = null)
        {
            foreach (var fileInfo in dir.GetFiles())
            {
                if (breakFunc != null && breakFunc(fileInfo)) return true;
            }
            foreach (var directory in dir.GetDirectories())
            {
                if (foreachFiles(directory, breakFunc))
                {
                    return true;
                }
            }
            return false;
        }
    }
}