/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;

using UnityEngine.Events;

namespace QFramework
{
    public class OnPropertyChangedEvent<T> : UnityEvent<T>
    {

    }
    
    public class CustomProperty<T> 
    {
        private bool mSetted = false;
        
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }
        
        private bool IsValueChanged(T value) => value == null || !value.Equals(mValue) || !mSetted;

        
        private void DispatchValueChangeEvent()
        {
            if (mSetter != null)
            {
                mSetter.Invoke(mValue);

                OnValueChanged.Invoke(mValue);

            }
        }
        
        private T mValue;
        

        private event Action<T> mSetter = t => { };

        private readonly UnityEvent<T> OnValueChanged = new OnPropertyChangedEvent<T>();
  
        private readonly Func<T> mValueGetter = null;

        private readonly Action<T> mValueSetter = null;
        
        public CustomProperty(Func<T> valueGetter, Action<T> valueSetter = null)
        {
            mValueGetter = valueGetter;
            mValueSetter = valueSetter;
        }

        public void Bind(UnityAction<T> onValueChanged)
        {
            OnValueChanged.AddListener(onValueChanged);
        }
        
        
        private T GetValue()
        {
            mValue = mValueGetter.Invoke();
            return mValue;
        }
        
        private void SetValue(T value)
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