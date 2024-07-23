/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

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
        }
    }
}