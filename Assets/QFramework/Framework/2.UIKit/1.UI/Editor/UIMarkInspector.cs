using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(UIMark))]
    public class UIMarkInspector : UnityEditor.Editor
    {

        class LocaleText
        {
            public static string MarkType
            {
                get { return Language.IsChinese ? "标记类型:" : "Mark Type:"; }
            }

            public static string Type
            {
                get { return Language.IsChinese ? "类型:" : "Type:"; }
            }

            public static string ClassName
            {
                get { return Language.IsChinese ? "生成类名:" : "Generate Class Name:"; }
            }

            public static string Comment
            {
                get { return Language.IsChinese ? "注释:" : "Comment:"; }
            }
        }

        public override void OnInspectorGUI()
        {
            var uiMarkScript = target as UIMark;
            
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.MarkType,GUILayout.Width(100));
            uiMarkScript.MarkType = (UIMarkType)EditorGUILayout.EnumPopup(uiMarkScript.MarkType);
            GUILayout.EndHorizontal();

            if (uiMarkScript.MarkType == UIMarkType.DefaultUnityElement)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.Type,GUILayout.Width(100));
                GUILayout.Label(uiMarkScript.ComponentName);
                GUILayout.EndHorizontal();
            }
            else if (uiMarkScript.MarkType == UIMarkType.Element)
            {
                if (string.IsNullOrEmpty(uiMarkScript.CustomComponentName.Trim()))
                {
                    uiMarkScript.CustomComponentName = uiMarkScript.name;
                }
                
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.ClassName,GUILayout.Width(100));
                uiMarkScript.CustomComponentName = GUILayout.TextField(uiMarkScript.CustomComponentName);
                GUILayout.EndHorizontal();
                
            } else if (uiMarkScript.MarkType == UIMarkType.Component)
            {
                if (string.IsNullOrEmpty(uiMarkScript.CustomComponentName.Trim()))
                {
                    uiMarkScript.CustomComponentName = uiMarkScript.name;
                }
                
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.ClassName,GUILayout.Width(100));
                uiMarkScript.CustomComponentName = GUILayout.TextField(uiMarkScript.CustomComponentName);
                GUILayout.EndHorizontal();
                
                // TODO: 支持生成组件路径
            }
            
            GUILayout.Label(LocaleText.Comment);
            uiMarkScript.CustomComment = EditorGUILayout.TextArea(uiMarkScript.Comment,GUILayout.Height(100));
            
            GUILayout.EndVertical();
            
            base.OnInspectorGUI();
        }
    }
}