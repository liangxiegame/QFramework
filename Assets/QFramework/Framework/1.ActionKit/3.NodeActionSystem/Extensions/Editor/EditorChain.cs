using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class EditorActionKit
    {
        private static bool mRegistered = false;
        
        private static void RegisterUpdate()
        {
            if (mRegistered) return;
            EditorApplication.update += Update;
            mRegistered = true;
        }

        private static void UnRegisterUpdate()
        {
            EditorApplication.update -= Update;
        }

        private static void Update()
        {
            if (mNodes.Count != 0)
            {
                if (!mNodes[0].Finished && mNodes[0].Execute(Time.deltaTime))
                {
                    var node = mNodes[0];
                    mNodes.RemoveAt(0);
                    node.Dispose();
                }
            }
        }
        
        public static void ExecuteNode(NodeAction nodeAction)
        {
            RegisterUpdate();
            mNodes.Add(nodeAction);
        }

        private static List<NodeAction> mNodes = new List<NodeAction>();

    }
}