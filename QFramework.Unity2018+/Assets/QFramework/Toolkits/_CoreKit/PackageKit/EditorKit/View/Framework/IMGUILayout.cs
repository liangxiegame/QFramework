/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public interface IMGUILayout : IMGUIView
    {
        IMGUILayout AddChild(IMGUIView view);

        void RemoveChild(IMGUIView view);

        void Clear();
    }
    
    public static class LayoutExtension
    {
        public static T Parent<T>(this T view, IMGUILayout parent) where T : IMGUIView
        {
            parent.AddChild(view);
            return view;
        }
    }
    
   
}