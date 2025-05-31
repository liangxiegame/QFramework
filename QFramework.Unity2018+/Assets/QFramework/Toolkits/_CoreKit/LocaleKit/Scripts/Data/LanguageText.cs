/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using UnityEngine;

namespace QFramework
{
    [System.Serializable]
    public class LanguageText
    {
        [SerializeField] public int LanguageIndex = 0;
        [HideInInspector] public Language Language;

        [TextArea(0,int.MaxValue)] public string Text;
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LanguageText))]
    public class LanguageTextDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
                return EditorGUIUtility.singleLineHeight;
            
            var textProperty = property.FindPropertyRelative("Text");
            var height = EditorGUI.GetPropertyHeight(textProperty);
            return height + EditorGUIUtility.singleLineHeight;
        }

        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
                return;

            var languageIndex = property.FindPropertyRelative("LanguageIndex");
            var language = property.FindPropertyRelative("Language");
            var text = property.FindPropertyRelative("Text");
            
            EditorGUI.BeginProperty(position, label, property);

            var languageRect = new Rect(position.x, position.y, position.width,
                EditorGUIUtility.singleLineHeight);
            var languages = LanguageDefineConfig.Default.LanguageDefines;
            
            languageIndex.intValue = EditorGUI.Popup(languageRect, languageIndex.intValue,
                languages.Select(l => l.Language.ToString()).ToArray());

            if (language.intValue != (int)languages[languageIndex.intValue].Language)
            {
                language.intValue = (int)languages[languageIndex.intValue].Language;
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var textHeight = EditorGUI.GetPropertyHeight(text);

            var textRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 5, position.width,
                textHeight - EditorGUIUtility.singleLineHeight - 12);
            text.stringValue = EditorGUI.TextArea(textRect, text.stringValue);


            var previewButtonRect = new Rect(position.x, position.y + 10
                + textHeight - 12,
                position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(previewButtonRect, Locale.Preview))
            {
                var localeText = property.serializedObject.targetObject as AbstractLocaleText;
                if (localeText != null) localeText.UpdateText((Language)language.intValue);
            }
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        
        public class Locale
        {
            public static string Preview => LocaleKitEditor.IsCN.Value ? "预览 " : "Preview ";
        }
    }
#endif
}