/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework
{
    public class LocaleUnityEvent : MonoBehaviour
    {
        public bool CallOnInit = true;

        public List<LanguageEvent> LocaleEvents;

        private void Start()
        {
            LocaleKit.OnLanguageChanged.Register(() => { CallUnityEvent(LocaleKit.CurrentLanguage); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            if (CallOnInit)
            {
                CallUnityEvent(LocaleKit.CurrentLanguage);
            }
        }

        void CallUnityEvent(Language language)
        {
            LocaleEvents.First(le => le.Language == language)?.OnLocale?.Invoke();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LocaleUnityEvent))]
    internal class LocaleUnityEventInspector : Editor
    {
        LocaleText mScript => target as LocaleText;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // serializedObject.Update();
            //
            // if (mScript.LanguageTexts != null)
            // {
            //     foreach (var scriptLanguageText in mScript.LanguageTexts)
            //     {
            //         if (GUILayout.Button("Preview " + scriptLanguageText.Language))
            //         {
            //             // mScript.GetComponent<Text>().text = scriptLanguageText.Text;
            //         }
            //     }
            // }
            //
            // serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}