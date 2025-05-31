using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ConsoleKit
    {
        private static LogModule mDefaultLogModule = new LogModule();
        private static FrameworkModule mDefaultFrameworkModule = new FrameworkModule();

        private static List<ConsoleModule> mModules = new List<ConsoleModule>()
        {
            mDefaultLogModule,
            mDefaultFrameworkModule
        };

        public static IReadOnlyList<ConsoleModule> Modules => mModules;

        public static void AddModule(ConsoleModule module)
        {
            mModules.Add(module);
        }

        public static void RemoveAllModules()
        {
            mModules.RemoveAll(m => m != mDefaultLogModule && m != mDefaultFrameworkModule);
        }

        public static void CreateWindow()
        {
            new GameObject("ConsoleKit")
                .AddComponent<ConsoleWindow>();
        }

        public static int GetDefaultIndex()
        {
            var index = mModules.FindIndex(m => m.Default);

            if (index == -1)
            {
                index = 0;
            }

            return index;
        }
    }
}