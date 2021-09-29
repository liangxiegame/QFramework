/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
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

using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IScrollLayout : IMGUILayout,IXMLToObjectConverter
    {
        
    }
    
    internal class ScrollLayout : Layout,IScrollLayout
    {
        Vector2 mScrollPos = Vector2.zero;

        protected override void OnGUIBegin()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, LayoutStyles);
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndScrollView();
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var scroll = EasyIMGUI.Scroll();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
            {
                if (childNodeAttribute.Name == "Id")
                {
                    scroll.Id = childNodeAttribute.Value;
                }
            }

            return scroll as T;
        }
    }
}