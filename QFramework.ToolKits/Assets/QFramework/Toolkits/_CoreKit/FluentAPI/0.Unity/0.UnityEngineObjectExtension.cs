/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("0.FluentAPI.Unity", "System.Object", 0)]
    [APIDescriptionCN("针对 System.Object 提供的链式扩展，理论上任何对象都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.object can theoretically be used by any Object")]
    [APIExampleCode(@"
var gameObject = new GameObject();

gameObject.Instantiate()
        .Name(""ExtensionExample"")
        .DestroySelf();

gameObject.Instantiate()
        .DestroySelfGracefully();

gameObject.Instantiate()
        .DestroySelfAfterDelay(1.0f);

gameObject.Instantiate()
        .DestroySelfAfterDelayGracefully(1.0f);

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

        #region CEUO001 Instantiate

        public static T Instantiate<T>(this T selfObj) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj);
        }

        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation);
        }

        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, position, rotation, parent);
        }


        public static T InstantiateWithParent<T>(this T selfObj, Transform parent, bool worldPositionStays)
            where T : UnityEngine.Object
        {
            return (T)UnityEngine.Object.Instantiate((UnityEngine.Object)selfObj, parent, worldPositionStays);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Transform parent) where T : UnityEngine.Object
        {
            return UnityEngine.Object.Instantiate(selfObj, parent, false);
        }

        #endregion

        #region CEUO002 Name

        public static T Name<T>(this T selfObj, string name) where T : UnityEngine.Object
        {
            selfObj.name = name;
            return selfObj;
        }

        #endregion

        #region CEUO003 Destroy Self

        public static void DestroySelf<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj);
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : UnityEngine.Object
        {
            if (selfObj)
            {
                UnityEngine.Object.Destroy(selfObj);
            }

            return selfObj;
        }

        #endregion

        #region CEUO004 Destroy Self AfterDelay

        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : UnityEngine.Object
        {
            UnityEngine.Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }

        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : UnityEngine.Object
        {
            if (selfObj)
            {
                UnityEngine.Object.Destroy(selfObj, delay);
            }

            return selfObj;
        }

        #endregion
        

        #region CEUO006 DontDestroyOnLoad

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : UnityEngine.Object
        {
            UnityEngine.Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }

        #endregion

        public static T As<T>(this object selfObj) where T : class
        {
            return selfObj as T;
        }
    }
}