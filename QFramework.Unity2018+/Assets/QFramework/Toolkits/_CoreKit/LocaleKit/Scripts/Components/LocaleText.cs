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
using UnityEngine.UI;

namespace QFramework
{
    public class LocaleText : MonoBehaviour
    {
        public bool SetTextOnInit = true;
        
        public List<LanguageText> LanguageTexts;

        private Text mText;
        private TextMesh mTextMesh;


        private void Start()
        {
            mText = GetComponent<Text>();
            mTextMesh = GetComponent<TextMesh>();


            LocaleKit.OnLanguageChanged.Register(() => { UpdateText(LocaleKit.CurrentLanguage); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            if (SetTextOnInit)
            {
                UpdateText(LocaleKit.CurrentLanguage);
            }
        }

        void UpdateText(Language language)
        {
            if (mText)
            {
                mText.text = LanguageTexts.First(lt => lt.Language == language).Text;
            }

            if (mTextMesh)
            {
                mTextMesh.text = LanguageTexts.First(lt => lt.Language == language).Text;
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(LocaleText))]
    internal class LocaleTextInspector : Editor
    {
        LocaleText mScript => target as LocaleText;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            if (mScript.LanguageTexts != null)
            {
                foreach (var scriptLanguageText in mScript.LanguageTexts)
                {
                    if (GUILayout.Button("Preview " + scriptLanguageText.Language))
                    {
                        Debug.Log(scriptLanguageText.Language);
                        Debug.Log((int)scriptLanguageText.Language);
                        mScript.GetComponent<Text>().text = scriptLanguageText.Text;
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}