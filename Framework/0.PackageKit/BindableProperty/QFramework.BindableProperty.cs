using System;
using UnityEngine;
using UnityEngine.Events;

namespace QFramework
{
    [Serializable]
    public class IntProperty : Property<int>
    {
        public int Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }


    [Serializable]
    public class Property<T>
    {
        public Property()
        {
        }

        private bool mSetted = false;

        public Property(T initValue)
        {
            mValue = initValue;
        }

        public T Value
        {
            get { return mValue; }
            set
            {
                if (value != null && value.Equals(mValue) && mSetted) return;

                mValue = value;

                if (mSetter != null)
                {
                    mSetter.Invoke(mValue);
                    OnValueChanged.Invoke(mValue);
                }

                mSetted = true;
            }
        }

        protected T mValue;

        public IDisposable Bind(Action<T> onValueChanged)
        {
            mSetter += onValueChanged;

            return new CustomDisposable(() => { mSetter -= onValueChanged; });
        }

        public void UnBindAll()
        {
            mSetter = null;
        }

        private event Action<T> mSetter = t => { };


        public UnityEvent<T> OnValueChanged = new OnPropertyChangedEvent<T>();
    }


    public class OnPropertyChangedEvent<T> : UnityEvent<T>
    {

    }

    public class CustomDisposable : IDisposable
    {
        private Action mOnDispose = null;

        public CustomDisposable(Action onDispose)
        {
            mOnDispose = onDispose;
        }

        public void Dispose()
        {
            mOnDispose.Invoke();
            mOnDispose = null;
        }
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
}