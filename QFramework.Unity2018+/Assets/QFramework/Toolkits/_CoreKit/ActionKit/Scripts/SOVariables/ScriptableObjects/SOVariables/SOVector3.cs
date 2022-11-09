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
    [CreateAssetMenu(fileName = "soVector3", menuName = "soVariables/soVector3", order = 1)]
    public class SOVector3 : ScriptableVariable, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public Vector3 value;

        //Can the value be reset in game
        //public bool resettable;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private Vector3 startingValue = Vector3.zero;

        /// <summary>
        /// Set sVector3 value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(Vector3 _value)
        {
            value = _value;
        }

        /// <summary>
        /// Set value to another sVector3 value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(SOVector3 _value)
        {
            value = _value.value;
        }

        /// <summary>
        /// Add a Vector3 value to the value
        /// </summary>
        /// <param name="_value"></param>
        public void AddValue(Vector3 _value)
        {
            value += _value;
        }

        /// <summary>
        /// Add another sVector3 value to the value
        /// </summary>
        /// <param name="_value"></param>
        public void AddValue(SOVector3 _value)
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
