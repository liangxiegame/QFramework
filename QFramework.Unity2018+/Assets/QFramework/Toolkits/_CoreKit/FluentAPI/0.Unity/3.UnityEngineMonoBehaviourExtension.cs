/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.MonoBehaviour", 3)]
    [APIDescriptionCN("MonoBehaviour 静态扩展")]
    [APIDescriptionEN("MonoBehaviour extension")]
#endif
    public static class UnityEngineMonoBehaviourExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var component = gameObject.GetComponent<MonoBehaviour>();

            component.Enable(); // component.enabled = true
            component.Disable(); // component.enabled = false
        }

#if UNITY_EDITOR
        // v1 No.149
        [MethodAPI]
        [APIDescriptionCN("monoBehaviour.enable = true")]
        [APIDescriptionEN("monoBehaviour.enable = true)")]
        [APIExampleCode(@"
myScript.Enable();
")]
#endif
        public static T Enable<T>(this T selfBehaviour, bool enable = true) where T : Behaviour
        {
            selfBehaviour.enabled = enable;
            return selfBehaviour;
        }

#if UNITY_EDITOR
        // v1 No.150
        [MethodAPI]
        [APIDescriptionCN("monoBehaviour.enable = false")]
        [APIDescriptionEN("monoBehaviour.enable = false")]
        [APIExampleCode(@"
myScript.Disable();
")]
#endif
        public static T Disable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = false;
            return selfBehaviour;
        }
    }
}