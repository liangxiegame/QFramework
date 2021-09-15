/****************************************************************************
 * Copyright (c) 2018 ~ 2020.12 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IButton : IMGUIView,
        IHasText<IButton>,
        IHasTextGetter<IButton>,
        ICanClick<IButton>,
        IXMLToObjectConverter
    {
        
        
    }

    internal class IMGUIButton : View, IButton
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

        public IButton Text(string labelText)
        {
            mLabelText = labelText;
            return this;
        }
        

        public IButton OnClick(Action action)
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
        public IButton TextGetter(Func<string> textGetter)
        {
            mTextGetter = textGetter;
            return this;
        }
    }
}