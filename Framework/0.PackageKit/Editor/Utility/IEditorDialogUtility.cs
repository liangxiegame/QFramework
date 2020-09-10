using UnityEditor;

namespace QFramework.PackageKit.Utility
{
    public interface IEditorDialogUtility : IUtility
    {
        void ShowErrorMsg(string content);
    }

    public class EditorDialogUtility : IEditorDialogUtility
    {
        public void ShowErrorMsg(string content)
        {
            EditorUtility.DisplayDialog("error", content, "OK");
        }
    }
}