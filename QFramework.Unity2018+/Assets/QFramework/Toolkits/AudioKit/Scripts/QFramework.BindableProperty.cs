/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

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
        public IUnRegister Bind(Action<T> onValueChanged)
        {
            mSetter += onValueChanged;

            return new CustomUnRegister(() => { mSetter -= onValueChanged; });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onValueChanged"></param>
        /// <returns></returns>
        public IUnRegister BindWithInitialValue(Action<T> onValueChanged)
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