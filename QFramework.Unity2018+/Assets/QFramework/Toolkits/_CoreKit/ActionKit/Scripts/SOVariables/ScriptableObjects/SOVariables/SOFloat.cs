/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using System;

namespace QFramework.Experimental
{
    [CreateAssetMenu(fileName = "soFloat", menuName = "soVariables/soFloat", order = 1)]
    public class SOFloat : ScriptableVariable, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public float value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private float startingValue = 0;

        /// <summary>
        /// Set sFloat value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(float _value)
        {
            value = _value;
        }

        /// <summary>
        /// Set value to another sBool value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(SOFloat _value)
        {
            value = _value.value;
        }

        /// <summary>
        /// Add a float value to the value
        /// </summary>
        /// <param name="_value"></param>
        public void AddValue(float _value)
        {
            value += _value;
        }

        /// <summary>
        /// Add another sFloat value to the value
        /// </summary>
        /// <param name="_value"></param>
        public void AddValue(SOFloat _value)
        {
            value += _value.value;
        }

        /// <summary>
        /// Recieve callback after unity deseriallzes the object
        /// </summary>
        public void OnAfterDeserialize()
        {
            value = startingValue;

        }

        public void OnBeforeSerialize() { }

        /// <summary>
        /// Reset the value to it's inital value if it's resettable
        /// </summary>
        public override void ResetValue()
        {
            value = startingValue;
        }
    }
}
