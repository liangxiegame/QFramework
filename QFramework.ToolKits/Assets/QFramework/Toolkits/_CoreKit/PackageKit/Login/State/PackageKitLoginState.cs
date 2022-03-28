/****************************************************************************
 * Copyright (c) 2020.10 liangxie
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

#if UNITY_EDITOR
using System;

namespace QFramework
{
    internal class PackageKitLoginState
    {
        public static BindableProperty<bool> InLoginView = new BindableProperty<bool>(false);

        public static CustomProperty<bool> Logined =
            new CustomProperty<bool>(() => User.Logined, value =>
            {
                if (!value)
                {
                    User.Clear();
                }
            });

        public static CustomProperty<bool> LoginViewVisible = new CustomProperty<bool>(
            () => !User.Logined && mInLoginView, (value) => mInLoginView = value
        );

        private static bool mInLoginView = true;

        public static BindableProperty<bool> RegisterViewVisible = new CustomProperty<bool>(
            () => !User.Logined && !mInLoginView,
            (value) => mInLoginView = !value);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CustomProperty<T> : BindableProperty<T>
    {
        private Func<T> mValueGetter = null;

        private Action<T> mValueSetter = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueGetter"></param>
        /// <param name="valueSetter"></param>
        public CustomProperty(Func<T> valueGetter, Action<T> valueSetter = null)
        {
            mValueGetter = valueGetter;
            mValueSetter = valueSetter;
        }

        public new T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected new T GetValue()
        {
            mValue = mValueGetter.Invoke();
            return mValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected new void SetValue(T value)
        {
            base.Value = value;
            if (mValueSetter != null) mValueSetter(value);
        }
    }
}
#endif