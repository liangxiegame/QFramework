using System;
using System.Collections.Generic;
using QF;

namespace EGO.Framework
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

        private bool setted = false;
        
        public Property(T initValue)
        {
            mValue = initValue;
        }

        public T Value
        {
            get { return mValue; } 
            set
            {
                if (value == null || !value.Equals(mValue) || !setted)
                {
                    mValue = value;

                    if (mSetter != null)
                    {
                        mSetter.Invoke(mValue);
                    }

                    setted = true;
                }
            }
        }

        private T mValue;

        /// <summary>
        /// TODO:注销也要做下
        /// </summary>
        /// <param name="setter"></param>
        public void Bind(Action<T> setter)
        {            
            mSetter += setter;
            mBindings.Add(setter);
        }

        private List<Action<T>> mBindings = new List<Action<T>>();

        public void UnBindAll()
        {
            foreach (var binding in mBindings)
            {
                mSetter -= binding;
            }
        }

        private event Action<T> mSetter;
    }
}