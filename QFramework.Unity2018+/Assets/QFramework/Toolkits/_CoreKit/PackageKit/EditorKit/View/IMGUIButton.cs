/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIButton : IMGUIView,
        IHasText<IMGUIButton>,
        IHasTextGetter<IMGUIButton>,
        ICanClick<IMGUIButton>,
        IXMLToObjectConverter
    {
    }

    internal class IMGUIButtonView : IMGUIAbstractView, IMGUIButton
    {
        private string mLabelText = string.Empty;
        private Action mOnClick = () => { };

        protected override void OnGUI()
        {
            if (GUILayout.Button(mTextGetter == null ? mLabelText : mTextGetter(), GUI.skin.button, LayoutStyles))
            {
                mOnClick.Invoke();
                // GUIUtility.ExitGUI();
            }
        }

        public IMGUIButton Text(string labelText)
        {
            mLabelText = labelText;
            return this;
        }


        public IMGUIButton OnClick(Action action)
        {
            mOnClick = action;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var button = EasyIMGUI.Button();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    button.Id = childNodeAttribute.Value;
                }
                else if (childNodeAttribute.Name == "Text")
                {
                    button.Text(childNodeAttribute.Value);
                }
                else if (childNodeAttribute.Name == "Width")
                {
                    button.Width(int.Parse(childNodeAttribute.Value));
                }
            }

            return button as T;
        }


        private Func<string> mTextGetter;

        public IMGUIButton Text(Func<string> textGetter)
        {
            mTextGetter = textGetter;
            return this;
        }
    }
}
#endif