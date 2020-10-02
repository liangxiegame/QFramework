using System;
using UnityEngine;

namespace QFramework
{
    public class GUIStyleProperty
    {
        private readonly Func<GUIStyle> mCreator;


        private Action<GUIStyle> mOperations = (style) => { };

        public GUIStyleProperty Set(Action<GUIStyle> operation)
        {
            mOperations += operation;
            return this;
        }

        public GUIStyleProperty(Func<GUIStyle> creator)
        {
            mCreator = creator;
        }

        private GUIStyle mValue = null;

        public GUIStyle Value
        {
            get
            {
                if (mValue != null) return mValue;
                mValue = mCreator.Invoke();
                mOperations(mValue);

                return mValue;
            }
            set { mValue = value; }
        }
    }
}