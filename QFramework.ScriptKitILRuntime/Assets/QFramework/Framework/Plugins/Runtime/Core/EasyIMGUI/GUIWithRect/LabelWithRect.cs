/****************************************************************************
 * Copyright (c) 2021.1 liangxie
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
    public interface ILabelWithRect : IMGUIView, IHasText<ILabelWithRect>, IXMLToObjectConverter,
        IHasRect<ILabelWithRect>
    {
    }

    public class LabelWithRect : View, ILabelWithRect
    {
        public LabelWithRect()
        {
            mStyleProperty = new GUIStyleProperty(() => new GUIStyle(GUI.skin.label));
        }

        private string mText = string.Empty;
        private Rect mRect = new Rect(0, 0, 200, 100);

        protected override void OnGUI()
        {
            GUI.Label(mRect, mText, mStyleProperty.Value);
        }

        public ILabelWithRect Text(string labelText)
        {
            mText = labelText;

            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            throw new System.NotImplementedException();
        }

        public ILabelWithRect Rect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public ILabelWithRect Position(Vector2 position)
        {
            mRect.position = position;
            return this;
        }

        public ILabelWithRect Position(float x, float y)
        {
            mRect.x = x;
            mRect.y = y;
            return this;
        }

        public ILabelWithRect Size(float width, float height)
        {
            mRect.width = width;
            mRect.height = height;
            return this;
        }

        public ILabelWithRect Size(Vector2 size)
        {
            mRect.size = size;
            return this;
        }

        public ILabelWithRect Width(float width)
        {
            mRect.width = width;
            return this;
        }

        public ILabelWithRect Height(float height)
        {
            mRect.height = height;
            return this;
        }
    }
}