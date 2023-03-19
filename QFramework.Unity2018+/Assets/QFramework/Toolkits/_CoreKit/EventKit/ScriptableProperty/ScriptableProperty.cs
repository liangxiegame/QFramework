/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework.Experimental
{
    public abstract class ScriptableProperty : ScriptableObject
    {
        /// <summary>
        /// Reset the value to it's inital value if it's resettable
        /// </summary>
        public abstract void ResetValue();
    }
}
