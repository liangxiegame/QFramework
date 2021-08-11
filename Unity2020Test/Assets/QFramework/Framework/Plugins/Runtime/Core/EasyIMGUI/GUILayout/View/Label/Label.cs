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
    public interface ILabel : IMGUIView, IHasText<ILabel>, IHasTextGetter<ILabel>, IXMLToObjectConverter
    {
    }

    internal class Label : View, ILabel
    {
        public string Content { get; set; }

        public Label()
        {
            mStyleProperty = new GUIStyleProperty(() => new GUIStyle(GUI.skin.label));
        }

        protected override void OnGUI()
        {
            GUILayout.Label(mTextGetter == null ? Content : mTextGetter(), Style.Value, LayoutStyles);
        }

        public ILabel Text(string labelText)
        {
            Content = labelText;
            return this;
        }

        public ILabel TextGetter(Func<string> textGetter)
        {
            mTextGetter = textGetter;
            return this;
        }

        private Func<string> mTextGetter;

        public T Convert<T>(XmlNode node) where T : class
        {
            var label = EasyIMGUI.Label();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    label.Id = childNodeAttribute.Value;
                }
                else if (childNodeAttribute.Name == "Text")
                {
                    label.Text(childNodeAttribute.Value);
                }
                else if (childNodeAttribute.Name == "FontBold")
                {
                    label.FontBold();
                }
                else if (childNodeAttribute.Name == "FontSize")
                {
                    label.FontSize(int.Parse(childNodeAttribute.Value));
                }
            }

            return label as T;
        }
    }
}