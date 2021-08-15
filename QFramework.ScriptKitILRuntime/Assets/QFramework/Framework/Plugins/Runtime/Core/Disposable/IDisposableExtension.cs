/****************************************************************************
 * Copyright (c) 2017 ~ 2021 liangxie
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
using UnityEngine;

namespace QFramework
{
    public static class IDisposableExtensions
    {
        /// <summary>
        /// 给继承IDisposable接口的对象 拓展相关Add方法
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        [Obsolete]
        public static void AddTo(this IDisposable self, IDisposableList component)
        {
            component.Add(self);
        }

        /// <summary>
        /// 给继承IDisposable接口的对象 拓展相关Add方法
        /// </summary>
        /// <param name="self"></param>
        /// <param name="disposableList"></param>
        public static void AddToDisposeList(this IDisposable self,IDisposableList disposableList)
        {
            disposableList.Add(self);
        }

        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        public static void DisposeWhenGameObjectDestroyed(this IDisposable self, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDestroyDisposeTrigger>()
                .AddDispose(self);
        }

        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void DisposeWhenGameObjectDestroyed(this IDisposable self, Component component)
        {
            component.gameObject.GetOrAddComponent<OnDestroyDisposeTrigger>()
                .AddDispose(self);
        }
        
        
        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        public static void DisposeWhenGameObjectDisabled(this IDisposable self, GameObject gameObject)
        {
            gameObject.GetOrAddComponent<OnDisableDisposeTrigger>()
                .AddDispose(self);
        }

        /// <summary>
        /// 与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void DisposeWhenGameObjectDisabled(this IDisposable self, Component component)
        {
            component.gameObject.GetOrAddComponent<OnDisableDisposeTrigger>()
                .AddDispose(self);
        }
    }
}