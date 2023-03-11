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

    [CreateAssetMenu(fileName = "soInt", menuName = "soVariables/soInt", order = 1)]
    public class SOInt : ScriptableVariable, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public int value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private int startingValue = 0;

        /// <summary>
        /// Set sInt value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(int _value)
        {
            value = _value;
        }

        /// <summary>
        /// Set value to another sInt value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(SOInt _value)
        {
            value = _value.value;
        }

        /// <summary>
        /// Add a int value to the value
        /// </summary>
        /// <param name="_value"></param>
        public void AddValue(int _value)
        {
            value += _value;
        }

        /// <summary>
        /// Add another sInt value to the value
        /// </summary>
        /// <param name="_value"></param>
        public void AddValue(SOInt _value)
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
