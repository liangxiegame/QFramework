/****************************************************************************
 * Copyright (c) 2018 ~ 2021.1 liangxie
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
    public interface ITextArea : IMGUIView, IHasText<ITextArea>, IXMLToObjectConverter
    {
        Property<string> Content { get; }
    }

    internal class TextArea : View, ITextArea
    {
        public TextArea()
        {
            Content = new Property<string>(string.Empty);
            mStyleProperty = new GUIStyleProperty(() => GUI.skin.textArea);
        }

        public Property<string> Content { get; private set; }

        protected override void OnGUI()
        {
            Content.Value = CrossPlatformGUILayout.TextArea(Content.Value, mStyleProperty.Value, LayoutStyles);
        }

        public ITextArea Text(string labelText)
        {
            Content.Value = labelText;
            return this;
        }



        public T Convert<T>(XmlNode node) where T : class
        {
            var textArea = EasyIMGUI.TextArea();

            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    textArea.Id = nodeAttribute.Value;
                }
                else if (nodeAttribute.Name == "Text")
                {
                    textArea.Text(nodeAttribute.Value);
                }
            }

            return textArea as T;
        }
    }
}