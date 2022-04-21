/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework
{

    /// <summary>
    /// 从这里开始一点点整理和重构 ResKit 的 API 以及内核
    /// </summary>
    public partial class ResKit
    {
        
    }
    
    public partial class ResKit
    {
        public static void Init()
        {
            ResMgr.Init();
        }
        
        public static IEnumerator InitAsync()
        {
            yield return ResMgr.InitAsync();
        }

        private static readonly Lazy<ResKit> mInstance = new Lazy<ResKit>(() => new ResKit().InternalInit());
        internal static ResKit Get => mInstance.Value;
        
        internal IOCContainer Container = new IOCContainer();

        ResKit InternalInit()
        {
            Container.Register<IZipFileHelper>(new ZipFileHelper());
            Container.Register<IBinarySerializer>(new BinarySerializer());
            return this;
        }
    }
}