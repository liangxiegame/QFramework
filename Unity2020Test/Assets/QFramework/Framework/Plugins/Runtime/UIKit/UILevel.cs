/****************************************************************************
 * Copyright (c) 2017 ~ 2020.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

namespace QFramework
{
    
    public enum UILevel
    {
        [Obsolete]
        AlwayBottom = -3, //如果不想区分太复杂那最底层的UI请使用这个
        Bg = -2, //背景层UI
        [Obsolete]
        AnimationUnderPage = -1, //动画层
        Common = 0, //普通层UI
        [Obsolete]
        AnimationOnPage = 1, // 动画层
        PopUI = 2, //弹出层UI
        [Obsolete]
        Guide = 3, //新手引导层
        [Obsolete]
        Const = 4, //持续存在层UI
        [Obsolete]
        Toast = 5, //对话框层UI
        [Obsolete]
        Forward = 6, //最高UI层用来放置UI特效和模型
        [Obsolete]
        AlwayTop = 7, //如果不想区分太复杂那最上层的UI请使用这个

        // 一个 Panel 就是一个 Canvas 的 Panel
        CanvasPanel = 100, // 
    }
}