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
    public interface ICanClick<T>
    {
        T OnClick(Action action);
    }

    public interface IHasRect<T>
    {
        T Rect(Rect rect);
        T Position(Vector2 position);
        T Position(float x, float y);
        T Size(float width, float height);
        T Size(Vector2 size);
        T Width(float width);
        T Height(float height);
    }

    public interface IHasText<T>
    {
        T Text(string labelText);
    }

    public interface IHasTextGetter<T>
    {
        T Text(Func<string> textGetter);
    }
}