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
    [CreateAssetMenu(fileName = "soBool", menuName = "soVariables/soBool", order = 1)]
    public class SOBool : ScriptableVariable, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public bool value;

        //Can the value be reset in game
        //public bool resettable;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private bool startingValue = false;

        /// <summary>
        /// Set sBool value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(bool _value)
        {
            value = _value;
        }

        /// <summary>
        /// Set value to another sBool value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(SOBool _value)
        {
            value = _value.value;
        }

        /// <summary>
        /// Swap the bool value
        /// </summary>
        public void Toggle()
        {
            value = !value;
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
