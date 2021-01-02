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

using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace QFramework
{
    public interface ISpace : IMGUIView,IXMLToObjectConverter
    {
        ISpace Pixel(int pixel);
    }
    
    internal class Space : View,ISpace
    {
        private int mPixel = 10;
        
        protected override void OnGUI()
        {
            GUILayout.Space(mPixel);
        }

        public ISpace Pixel(int pixel)
        {
            mPixel = pixel;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var space = EasyIMGUI.Space();
            
            foreach (XmlAttribute nodeAttribute in node.Attributes)
            {
                if (nodeAttribute.Name == "Id")
                {
                    space.Id = nodeAttribute.Value;
                } 
                else if (nodeAttribute.Name == "Pixel")
                {
                    space.Pixel(int.Parse(nodeAttribute.Value));
                }
            }

            return space as T;
        }
    }
}