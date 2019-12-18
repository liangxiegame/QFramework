using UnityEditor;

namespace QFramework.PackageKit
{
    public class DialogUtils
    {
        public static void ShowErrorMsg(string content)
        {
            EditorUtility.DisplayDialog("error", content, "OK");
        }
    }
}