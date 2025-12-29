/****************************************************************************
 * Copyright (c) 2016 - 2025 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("07.ResKit", "ResKit", 0, "ResKit")]
    [APIDescriptionCN("资源管理方案")]
    [APIDescriptionEN("Resource Managements Solution")]
#endif
    public class ResKit
    {
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        public static void CheckAutoInit()
        {
            if (PlatformCheck.IsEditor && AssetBundlePathHelper.SimulationMode)
            {
                Init();
            }
        }
#endif

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("初始化 ResKit")]
        [APIDescriptionEN("initialise ResKit")]
        [APIExampleCode(@"
ResKit.Init();
")]
#endif
        public static void Init()
        {
            ResMgr.Init();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("异步初始化 ResKit，如果是 WebGL 平台，只支持异步初始化")]
        [APIDescriptionEN("initialise ResKit async")]
        [APIExampleCode(@"
IEnumerator Start()
{
    yield return ResKit.InitAsync();
}

// Or With ActionKit
ResKit.InitAsync().ToAction().Start(this,()=>
{

});
")]
#endif
        public static IEnumerator InitAsync()
        {
            yield return ResMgr.InitAsync();
        }
        
    }
}