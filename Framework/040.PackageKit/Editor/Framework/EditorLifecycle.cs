using System;
using System.Collections.Generic;
using UnityEditor;

namespace QFramework
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
        
        private static Queue<System.Action> mPrivateCommands = new Queue<System.Action>();

        private static Queue<System.Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(System.Action command)
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