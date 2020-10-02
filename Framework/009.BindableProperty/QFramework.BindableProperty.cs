using System;

#if UNITY_5_6_OR_NEWER
using UnityEngine;
using UnityEngine.Events;
#endif

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
#if UNITY_5_6_OR_NEWER
                OnValueChanged.Invoke(mValue);
#endif
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

#if UNITY_5_6_OR_NEWER
        public UnityEvent<T> OnValueChanged = new OnPropertyChangedEvent<T>();
#endif
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


#if UNITY_5_6_OR_NEWER
    public class OnPropertyChangedEvent<T> : UnityEvent<T>
    {

    }
#endif

 

#if UNITY_5_6_OR_NEWER
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
#endif

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