/****************************************************************************
 * Copyright (c) 2021.3 liangxie
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
using UnityEngine;

namespace QFramework
{
    public interface IAreaLayout : IMGUILayout
    {
        IAreaLayout WithRect(Rect rect);
        IAreaLayout WithRectGetter(Func<Rect> rectGetter);
    }

    public class AreaLayout : Layout, IAreaLayout
    {
        private Rect mRect;
        private Func<Rect> mRectGetter;

        public IAreaLayout WithRect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public IAreaLayout WithRectGetter(Func<Rect> rectGetter)
        {
            mRectGetter = rectGetter;
            return this;
        }

        protected override void OnGUIBegin()
        {
            GUILayout.BeginArea(mRectGetter == null ? mRect : mRectGetter());
        }

        protected override void OnGUIEnd()
        {
            GUILayout.EndArea();
        }
    }
}