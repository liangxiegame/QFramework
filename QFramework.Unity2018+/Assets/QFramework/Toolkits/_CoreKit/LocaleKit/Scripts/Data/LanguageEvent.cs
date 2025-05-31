/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace QFramework
{
    [System.Serializable]
    public class LanguageEvent
    {
        [SerializeField] public int LanguageIndex;
        [HideInInspector] public Language Language;
        [SerializeField] public UnityEvent OnLocale;
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LanguageEvent))]
    public class LanguageEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUILayout.BeginVertical("box");
            {
                var languageIndex = property.FindPropertyRelative("LanguageIndex");

                var onLocale = property.FindPropertyRelative("OnLocale");

                var languages = LanguageDefineConfig.Default.LanguageDefines;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Language:");
                    EditorGUI.BeginChangeCheck();
                    languageIndex.intValue = EditorGUILayout.Popup(languageIndex.intValue,
                        languages.Select(l => l.Language.ToString()).ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {

                        property.FindPropertyRelative("Language").intValue =
                            (int)languages[languageIndex.intValue].Language;

                    }
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(onLocale);
            }
            GUILayout.EndVertical();
        }
    }
#endif
}