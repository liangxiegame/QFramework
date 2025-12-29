using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class ConsoleKit
    {
        private static readonly LogModule mDefaultLogModule = new LogModule();
        private static readonly FrameworkModule mDefaultFrameworkModule = new FrameworkModule();

        private static readonly List<ConsoleModule> mModules = new List<ConsoleModule>()
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

        public static void CreateWindow(bool showGUI = true)
        {
            if (ConsoleWindow.Default)
            {
                return;
            }

            var consoleWindow = new GameObject("ConsoleKit")
                .AddComponent<ConsoleWindow>();
            consoleWindow.ShowGUI = showGUI;
        }
        
        public static void ShowModule(string title)
        {
            if (ConsoleWindow.Default)
            {
                var index = mModules.FindIndex(m => m.Title == title);
                if (index != -1)
                {
                    ConsoleWindow.Default.ShowModule(index);
                }
            }
            else
            {
                var module = mModules.Find(m => m.Title == title);
                if (module != null)
                {
                    module.Default = true;
                }
            }
        }

        internal static int GetDefaultIndex()
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