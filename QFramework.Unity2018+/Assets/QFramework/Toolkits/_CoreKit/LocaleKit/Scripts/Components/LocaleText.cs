/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class LocaleText : AbstractLocaleText
    {
        private Text mText;
        private TextMesh mTextMesh;

        protected override void SetText(string text)
        {
            if (!mText && !mTextMesh)
            {
                mText = GetComponent<Text>();
                mTextMesh = GetComponent<TextMesh>();
            }

            if (mText)
            {
                mText.text = text;
            }

            if (mTextMesh)
            {
                mTextMesh.text = text;
            }
        }
    }
}