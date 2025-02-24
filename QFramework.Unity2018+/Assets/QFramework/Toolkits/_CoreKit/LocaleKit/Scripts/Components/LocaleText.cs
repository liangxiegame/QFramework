/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
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

        public Text Text => mText;
        public TextMesh TextMesh => mTextMesh;

        public static Action<LocaleText> OnUpdateText;
        
        private void Start()
        {
            mText = GetComponent<Text>();
            mTextMesh = GetComponent<TextMesh>();
            
            LocaleKit.CurrentLanguage.Register(UpdateText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            if (SetTextOnInit)
            {
                UpdateText(LocaleKit.CurrentLanguage.Value);
            }
        }

        public void UpdateText(Language language)
        {
            if (mText)
            {
                mText.text = LanguageTexts.First(lt => lt.Language == language).Text;
            }

            if (mTextMesh)
            {
                mTextMesh.text = LanguageTexts.First(lt => lt.Language == language).Text;
            }
            
            else if (mTextMesh)
            {
                mTextMesh.text = LanguageTexts.First(lt => lt.Language == language).Text;
            }
            else // Editor Support
            {
                var text = GetComponent<Text>();

                if (text)
                {
                    text.text = LanguageTexts.First(lt => lt.Language == language).Text;
                }
                else
                {
                    var textMesh = GetComponent<TextMesh>();
                    if (textMesh)
                    {
                        textMesh.text = LanguageTexts.First(lt => lt.Language == language).Text;
                    }
                }
            }

            OnUpdateText?.Invoke(this);
        }
    }
}