#if UNITY_EDITOR
namespace QFramework
{
    public static class EditorActionKit
    {
        public static void ExecuteNode(NodeAction nodeAction)
        {
            new NodeActionEditorWrapper(nodeAction);
        }
    }
}
#endif