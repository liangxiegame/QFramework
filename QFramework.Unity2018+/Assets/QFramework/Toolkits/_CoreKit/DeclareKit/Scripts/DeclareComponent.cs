/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    [DisallowMultipleComponent]
    public class DeclareComponent : MonoBehaviour
    {
        public string[] ClassNames;
        
        private void Start()
        {
            DeclareKit.AttachRules(this);
        }
    }
}
