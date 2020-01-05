using System;
using System.Collections.Generic;
using UnityEditor;

namespace QFramework.PackageKit
{
    public class EditorLifecycle
    {
        static EditorLifecycle()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            ExecuteCommand();
        }
        
        private static Queue<Action> mPrivateCommands = new Queue<Action>();

        private static Queue<Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        public static void ExecuteCommand()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }
}