/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using MoonSharp.Interpreter;
using UnityEngine;

namespace QFramework
{
    public class FluentGUIStyle
    {
        public static FluentGUIStyle Label()
        {
            var fluentGUIStyle = new FluentGUIStyle();
            fluentGUIStyle.mStyleFactory = () => new GUIStyle("label");
            return fluentGUIStyle;
        }

        public static FluentGUIStyle Button()
        {
            var fluentGUIStyle = new FluentGUIStyle();
            fluentGUIStyle.mStyleFactory = () => new GUIStyle("button");
            return fluentGUIStyle;
        }

        public FluentGUIStyle FontBold()
        {
            Set(style => style.fontStyle = FontStyle.Bold);
            return this;
        }

        public FluentGUIStyle FontNormal()
        {
            Set(style => style.fontStyle = FontStyle.Normal);
            return this;
        }

        public FluentGUIStyle FontSize(int size)
        {
            Set(style => style.fontSize = size);
            return this;
        }

        private Func<GUIStyle> mStyleFactory = () => new GUIStyle();

        private Action<GUIStyle> mOperations = style => { };

        public FluentGUIStyle Set(Action<GUIStyle> operation)
        {
            mOperations += operation;
            return this;
        }

        [MoonSharpHidden]
        public FluentGUIStyle()
        {
        }

        [MoonSharpHidden]
        public FluentGUIStyle(Func<GUIStyle> styleFactory)
        {
            mStyleFactory = styleFactory;
        }

        private GUIStyle mValue = null;

        public GUIStyle Value
        {
            get
            {
                if (mValue != null) return mValue;
                mValue = mStyleFactory.Invoke();
                mOperations(mValue);

                return mValue;
            }
            set => mValue = value;
        }

        public static implicit operator GUIStyle(FluentGUIStyle style) => style.Value;
    }
}