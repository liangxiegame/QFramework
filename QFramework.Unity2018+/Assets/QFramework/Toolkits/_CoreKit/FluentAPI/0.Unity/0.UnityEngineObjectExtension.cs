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
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Object", 0)]
    [APIDescriptionCN("针对 UnityEngine.Object 提供的链式扩展")]
    [APIDescriptionEN("The chain extension provided by UnityEngine.Object")]
    [APIExampleCode(@"
var gameObject = new GameObject();
//
gameObject.Instantiate()
        .Name(""ExtensionExample"")
        .DestroySelf();
//
gameObject.Instantiate()
        .DestroySelfGracefully();
//
gameObject.Instantiate()
        .DestroySelfAfterDelay(1.0f);
//
gameObject.Instantiate()
        .DestroySelfAfterDelayGracefully(1.0f);
//
gameObject
        .Self(selfObj => Debug.Log(selfObj.name))
        .Name(""TestObj"")
        .Self(selfObj => Debug.Log(selfObj.name))
        .Name(""ExtensionExample"")
        .DontDestroyOnLoad();
")]
#endif
    public static class UnityEngineObjectExtension
    {
#if UNITY_EDITOR
        // v1 No.37
        [MethodAPI]
        [APIDescriptionCN("Object.Instantiate(Object) 的简单链式封装")]
        [APIDescriptionEN("Object.Instantiate(Object) extension")]
        [APIExampleCode(@"
prefab.Instantiate();
")]
#endif
        public static T Instantiate<T>(this T selfObj) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj);
        }
#if UNITY_EDITOR
        // v1 No.38
        [MethodAPI]
        [APIDescriptionCN("Object.Instantiate(Object,Vector3,Quaternion) 的简单链式封装")]
        [APIDescriptionEN("Object.Instantiate(Object,Vector3,Quaternion) extension")]
        [APIExampleCode(@"
prefab.Instantiate(Vector3.zero,Quaternion.identity);
")]
#endif
        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation);
        }

#if UNITY_EDITOR
        // v1 No.39
        [MethodAPI]
        [APIDescriptionCN("Object.Instantiate(Object,Vector3,Quaternion,Transform parent) 的简单链式封装")]
        [APIDescriptionEN("Object.Instantiate(Object,Vector3,Quaternion,Transform parent) extension")]
        [APIExampleCode(@"
prefab.Instantiate(Vector3.zero,Quaternion.identity,transformRoot);
")]
#endif
        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation, parent);
        }

#if UNITY_EDITOR
        // v1 No.40
        [MethodAPI]
        [APIDescriptionCN("Object.Instantiate(Transform parent,bool worldPositionStays) 的简单链式封装")]
        [APIDescriptionEN("Object.Instantiate(Transform parent,bool worldPositionStays) extension")]
        [APIExampleCode(@"
prefab.Instantiate(transformRoot,true);
")]
#endif
        public static T InstantiateWithParent<T>(this T selfObj, Transform parent, bool worldPositionStays)
            where T : UnityEngine.Object
        {
            return (T)UnityEngine.Object.Instantiate((UnityEngine.Object)selfObj, parent, worldPositionStays);
        }
        
        public static T InstantiateWithParent<T>(this T selfObj, Component parent, bool worldPositionStays)
            where T : UnityEngine.Object
        {
            return (T)UnityEngine.Object.Instantiate((UnityEngine.Object)selfObj, parent.transform, worldPositionStays);
        }
#if UNITY_EDITOR
        // v1 No.41
        [MethodAPI]
        [APIDescriptionCN("Object.Instantiate(Transform parent) 的简单链式封装")]
        [APIDescriptionEN("Object.Instantiate(Transform parent) extension")]
        [APIExampleCode(@"
prefab.Instantiate(transformRoot);
")]
#endif
        public static T InstantiateWithParent<T>(this T selfObj, Transform parent) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, parent, false);
        }
        
        public static T InstantiateWithParent<T>(this T selfObj, Component parent) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, parent.transform, false);
        }


#if UNITY_EDITOR
        // v1 No.42
        [MethodAPI]
        [APIDescriptionCN("设置名字")]
        [APIDescriptionEN("set Object's name")]
        [APIExampleCode(@"
scriptableObject.Name(""LevelData"");
Debug.Log(scriptableObject.name);
// LevelData
")]
#endif
        public static T Name<T>(this T selfObj, string name) where T : UnityEngine.Object
        {
            selfObj.name = name;
            return selfObj;
        }


#if UNITY_EDITOR
        // v1 No.43
        [MethodAPI]
        [APIDescriptionCN("Object.Destroy(Object) 简单链式封装")]
        [APIDescriptionEN("Object.Destroy(Object) extension")]
        [APIExampleCode(@"
new GameObject().DestroySelf()
")]
#endif
        public static void DestroySelf<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj);
        }

#if UNITY_EDITOR
        // v1 No.44
        [MethodAPI]
        [APIDescriptionCN("Object.Destroy(Object) 简单链式封装")]
        [APIDescriptionEN("Object.Destroy(Object) extension")]
        [APIExampleCode(@"
GameObject gameObj = null;
gameObj.DestroySelfGracefully();
// not throw null exception
// 这样写不会报异常(但是不好调试)
")]
#endif
        public static T DestroySelfGracefully<T>(this T selfObj) where T : UnityEngine.Object
        {
            if (selfObj)
            {
                UnityEngine.Object.Destroy(selfObj);
            }

            return selfObj;
        }


#if UNITY_EDITOR
        // v1 No.45
        [MethodAPI]
        [APIDescriptionCN("Object.Destroy(Object,float) 简单链式封装")]
        [APIDescriptionEN("Object.Destroy(Object,float) extension")]
        [APIExampleCode(@"
new GameObject().DestroySelfAfterDelay(5);
")]
#endif
        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }

#if UNITY_EDITOR
        // v1 No.46
        [MethodAPI]
        [APIDescriptionCN("Object.Destroy(Object,float) 简单链式封装")]
        [APIDescriptionEN("Object.Destroy(Object,float) extension")]
        [APIExampleCode(@"
GameObject gameObj = null;
gameObj.DestroySelfAfterDelayGracefully(5);
// not throw exception
// 不会报异常
")]
#endif
        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : UnityEngine.Object
        {
            if (selfObj)
            {
                UnityEngine.Object.Destroy(selfObj, delay);
            }

            return selfObj;
        }

#if UNITY_EDITOR
        // v1 No.47
        [MethodAPI]
        [APIDescriptionCN("Object.DontDestroyOnLoad 简单链式封装")]
        [APIDescriptionEN("Object.DontDestroyOnLoad extension")]
        [APIExampleCode(@"
new GameObject().DontDestroyOnLoad();
")]
#endif
        public static T DontDestroyOnLoad<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }
    }
}