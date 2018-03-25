/****************************************************************************
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2018 liangxie
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

namespace QFramework
{
    using System;
    using UnityEngine;
    
    public static class FuncOrActionOrEventExtension
    {
        private delegate void TestDelegate();

        public static void Example()
        {
            // action
            System.Action action = () => Debug.Log("action called");
            action.InvokeGracefully(); // if (action != null) action();

            // func
            Func<int> func = () => 1;
            func.InvokeGracefully();

            /*
            public static T InvokeGracefully<T>(this Func<T> selfFunc)
            {
                return null != selfFunc ? selfFunc() : default(T);
            }
            */

            // delegate
            TestDelegate testDelegate = () => { };
            testDelegate.InvokeGracefully();
        }
        
        
        #region Func Extension

        public static T InvokeGracefully<T>(this Func<T> selfFunc)
        {
            return null != selfFunc ? selfFunc() : default(T);
        }

        #endregion

        #region Action

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully(this System.Action selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool InvokeGracefully<T>(this Action<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully<T, K>(this Action<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call delegate
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call suceed </returns>
        public static bool InvokeGracefully(this Delegate selfAction, params object[] args)
        {
            if (null != selfAction)
            {
                selfAction.DynamicInvoke(args);
                return true;
            }
            return false;
        }

        #endregion
    }
}