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
    [CreateAssetMenu(fileName = "soGameObject", menuName = "soVariables/soGameObject", order = 1)]
    public class SOGameObject : ScriptableVariable, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public GameObject value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private GameObject startingValue = null;

        /// <summary>
        /// Set sGameObject value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(GameObject _value)
        {
            value = _value;
        }

        /// <summary>
        /// Set value to another sGameObject value
        /// </summary>
        /// <param name="_value"></param>
        public void SetValue(SOGameObject _value)
        {
            value = _value.value;
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
