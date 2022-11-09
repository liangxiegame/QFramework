/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEngine;

namespace QFramework
{
    internal class MDContext
    {
        public MDContext(GUISkin skin, MDHandlerImages images, MDHandlerNavigate navigate)
        {
            mStyleConverter = new MDStyleConverter(skin);
            mImages = images;
            mNagivate = navigate;

            Apply(MDStyle.Default);
        }

        MDStyleConverter mStyleConverter;
        GUIStyle mStyleGUI;
        MDHandlerImages mImages;
        MDHandlerNavigate mNagivate;

        public void SelectPage(string path)
        {
            mNagivate.SelectPage(path);
        }

        public Texture FetchImage(string url)
        {
            return mImages.FetchImage(url);
        }

        public float LineHeight
        {
            get { return mStyleGUI.lineHeight; }
        }

        public float MinWidth
        {
            get { return LineHeight * 2.0f; }
        }

        public float IndentSize
        {
            get { return LineHeight * 1.5f; }
        }

        public void Reset()
        {
            Apply(MDStyle.Default);
        }

        public GUIStyle Apply(MDStyle style)
        {
            mStyleGUI = mStyleConverter.Apply(style);
            mStyleGUI.richText = true;
            return mStyleGUI;
        }

        public Vector2 CalcSize(GUIContent content)
        {
            return mStyleGUI.CalcSize(content);
        }
    }
}
#endif