﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class EditorActionKit
    {
        public static void ExecuteNode(NodeAction nodeAction)
        {
            new NodeActionEditorWrapper(nodeAction);
        }
    }

    public class NodeActionEditorWrapper
    {
        private NodeAction mNodeAction;

        public NodeActionEditorWrapper(NodeAction action)
        {
            mNodeAction = action;
            EditorApplication.update += Update;
            mNodeAction.OnEndedCallback += () =>
            {
                EditorApplication.update -= Update; 
            };
        }

        void Update()
        {
            if (!mNodeAction.Finished && mNodeAction.Execute(Time.deltaTime))
            {
                mNodeAction.Dispose();
                mNodeAction = null;
            }
        }
    }
}