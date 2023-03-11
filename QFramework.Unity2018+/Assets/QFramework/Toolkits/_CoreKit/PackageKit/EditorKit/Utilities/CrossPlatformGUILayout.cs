#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework
{
    public class CrossPlatformGUILayout
    {
        public static string PasswordField(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR
            return EditorGUILayout.PasswordField(value, style, options);
#else
            return GUILayout.PasswordField(value, '*', style, options);
#endif
        }

        public static string TextField(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR

            return EditorGUILayout.TextField(value, style, options);
#else
            return GUILayout.TextField(value, style, options);
#endif
        }

        public static string TextArea(string value, GUIStyle style, GUILayoutOption[] options)
        {
#if UNITY_EDITOR

            return EditorGUILayout.TextArea(value, style, options);
#else
            return GUILayout.TextArea(value, style, options);
#endif
        }
    }
}