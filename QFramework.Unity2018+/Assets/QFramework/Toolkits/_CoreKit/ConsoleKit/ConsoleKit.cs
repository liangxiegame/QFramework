using System;
using System.Collections.Generic;

namespace QFramework
{
    public class ConsoleKit
    {
        private static LogModule mDefaultLogModule = new LogModule();
        private static List<ConsoleModule> mModules = new List<ConsoleModule>()
        {
            mDefaultLogModule
        };

        public static IReadOnlyList<ConsoleModule> Modules => mModules;

        public static void InitModules()
        {
            Modules.ForEach(m => m.OnInit());
        }

        public static void AddModule(ConsoleModule module)
        {
            mModules.Add(module);
        }

        public static void DestroyModules()
        {
            Modules.ForEach(m => m.OnDestroy());
            mModules.RemoveAll(m => m != mDefaultLogModule);
        }
    }
    

}