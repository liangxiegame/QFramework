/****************************************************************************
 * Copyright (c) 2017 ~ 2020.12 liangxie
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
using UnityEngine.Events;

namespace QFramework
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Property<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public Property()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool mSetted = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initValue"></param>
        public Property(T initValue)
        {
            mValue = initValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual T GetValue()
        {
            return mValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected virtual void SetValue(T value)
        {
            if (IsValueChanged(value))
            {
                mValue = value;

                DispatchValueChangeEvent();

                mSetted = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool IsValueChanged(T value)
        {
            return value == null || !value.Equals(mValue) || !mSetted;
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual void DispatchValueChangeEvent()
        {
            if (mSetter != null)
            {
                mSetter.Invoke(mValue);

                OnValueChanged.Invoke(mValue);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected T mValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public IDisposable Bind(Action<T> onValueChanged)
        {
            mSetter += onValueChanged;

            return new CustomDisposable(() => { mSetter -= onValueChanged; });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public IDisposable BindWithInitialValue(Action<T> onValueChanged)
        {
            onValueChanged.Invoke(GetValue());
            return Bind(onValueChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        public void UnBindAll()
        {
            mSetter = null;
        }

        private event Action<T> mSetter = t => { };

        public UnityEvent<T> OnValueChanged = new OnPropertyChangedEvent<T>();
    }
    
    /// <summary>
    /// Int 类型的 Property
    /// </summary>
    [Serializable]
    public class IntProperty : Property<int>
    {
        /// <summary>
        /// 值
        /// </summary>
        public new int Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }


    public class OnPropertyChangedEvent<T> : UnityEvent<T>
    {

    }



    public class PlayerPrefsBooleanProperty : Property<bool>
    {
        public PlayerPrefsBooleanProperty(string saveKey, bool defaultValue = false)
        {
            var initValue = PlayerPrefs.GetInt(saveKey, defaultValue ? 1 : 0) == 1;

            mValue = initValue;

            this.Bind(value => PlayerPrefs.SetInt(saveKey, value ? 1 : 0));
        }
    }

    public class PlayerPrefsFloatProperty : Property<float>
    {
        public PlayerPrefsFloatProperty(string saveKey, float defaultValue = 0.0f)
        {
            var initValue = PlayerPrefs.GetFloat(saveKey, defaultValue);

            mValue = initValue;

            this.Bind(value => PlayerPrefs.SetFloat(saveKey, value));
        }
    }

    public class PlayerPrefsIntProperty : Property<int>
    {
        public PlayerPrefsIntProperty(string saveKey, int defaultValue = 0)
        {
            var initValue = PlayerPrefs.GetInt(saveKey, defaultValue);

            mValue = initValue;

            this.Bind(value => PlayerPrefs.SetInt(saveKey, value));
        }
    }

    public class PlayerPrefsStringProperty : Property<string>
    {
        public PlayerPrefsStringProperty(string saveKey, string defaultValue = null)
        {
            var initValue = PlayerPrefs.GetString(saveKey, defaultValue);

            mValue = initValue;

            this.Bind(value => PlayerPrefs.SetString(saveKey, value));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CustomProperty<T> : Property<T>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override T GetValue()
        {
            mValue = mValueGetter.Invoke();
            return mValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected override void SetValue(T value)
        {
            if (IsValueChanged(value))
            {
                mValue = value;

                DispatchValueChangeEvent();

                mSetted = true;

                if (mValueSetter != null) mValueSetter(value);
            }
        }
    }
}