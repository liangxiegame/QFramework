/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIAreaLayout : IMGUILayout
    {
        IMGUIAreaLayout WithRect(Rect rect);
        IMGUIAreaLayout WithRectGetter(Func<Rect> rectGetter);
    }

    public class IMGUIAreaLayoutView : IMGUIAbstractLayout, IMGUIAreaLayout
    {
        private Rect mRect;
        private Func<Rect> mRectGetter;

        public IMGUIAreaLayout WithRect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public IMGUIAreaLayout WithRectGetter(Func<Rect> rectGetter)
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