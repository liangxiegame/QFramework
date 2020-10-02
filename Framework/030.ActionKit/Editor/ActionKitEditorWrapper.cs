using UnityEngine;

namespace QFramework
{
    public class NodeActionEditorWrapper
    {
        private NodeAction mNodeAction;

        public NodeActionEditorWrapper(NodeAction action)
        {
            mNodeAction = action;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
            mNodeAction.OnEndedCallback += () => { UnityEditor.EditorApplication.update -= Update; };
#endif
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