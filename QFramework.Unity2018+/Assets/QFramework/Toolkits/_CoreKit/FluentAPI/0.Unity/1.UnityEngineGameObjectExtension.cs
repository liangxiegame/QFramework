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
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.GameObject", 1)]
    [APIDescriptionCN("针对 UnityEngine.GameObject 提供的链式扩展")]
    [APIDescriptionEN("The chain extension provided by UnityEngine.Object.")]
    [APIExampleCode(@"
var gameObject = new GameObject();
var transform = gameObject.transform;
var selfScript = gameObject.AddComponent<MonoBehaviour>();
var boxCollider = gameObject.AddComponent<BoxCollider>();
//
gameObject.Show(); // gameObject.SetActive(true)
selfScript.Show(); // this.gameObject.SetActive(true)
boxCollider.Show(); // boxCollider.gameObject.SetActive(true)
gameObject.transform.Show(); // transform.gameObject.SetActive(true)
//
gameObject.Hide(); // gameObject.SetActive(false)
selfScript.Hide(); // this.gameObject.SetActive(false)
boxCollider.Hide(); // boxCollider.gameObject.SetActive(false)
transform.Hide(); // transform.gameObject.SetActive(false)
//
selfScript.DestroyGameObj();
boxCollider.DestroyGameObj();
]transform.DestroyGameObj();
//
selfScript.DestroyGameObjGracefully();
boxCollider.DestroyGameObjGracefully();
transform.DestroyGameObjGracefully();
//
selfScript.DestroyGameObjAfterDelay(1.0f);
boxCollider.DestroyGameObjAfterDelay(1.0f);
transform.DestroyGameObjAfterDelay(1.0f);
//
selfScript.DestroyGameObjAfterDelayGracefully(1.0f);
boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
transform.DestroyGameObjAfterDelayGracefully(1.0f);
//
gameObject.Layer(0);
selfScript.Layer(0);
boxCollider.Layer(0);
transform.Layer(0);
//
gameObject.Layer(""Default"");
selfScript.Layer(""Default"");
boxCollider.Layer(""Default"");
transform.Layer(""Default"");
")]
#endif
    public static class UnityEngineGameObjectExtension
    {
#if UNITY_EDITOR
        // v1 No.48
        [MethodAPI]
        [APIDescriptionCN("gameObject.SetActive(true)")]
        [APIDescriptionEN("gameObject.SetActive(true)")]
        [APIExampleCode(@"
new GameObject().Show();
")]
#endif
        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }
#if UNITY_EDITOR
        // v1 No.49
        [MethodAPI]
        [APIDescriptionCN("script.gameObject.SetActive(true)")]
        [APIDescriptionEN("script.gameObject.SetActive(true)")]
        [APIExampleCode(@"
GetComponent<MyScript>().Show();
")]
#endif
        public static T Show<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Show();
            return selfComponent;
        }


#if UNITY_EDITOR
        // v1 No.50
        [MethodAPI]
        [APIDescriptionCN("gameObject.SetActive(false)")]
        [APIDescriptionEN("gameObject.SetActive(false)")]
        [APIExampleCode(@"
gameObject.Hide();
")]
#endif
        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

#if UNITY_EDITOR
        // v1 No.51
        [MethodAPI]
        [APIDescriptionCN("myScript.gameObject.SetActive(false)")]
        [APIDescriptionEN("myScript.gameObject.SetActive(false)")]
        [APIExampleCode(@"
GetComponent<MyScript>().Hide();
")]
#endif
        public static T Hide<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Hide();
            return selfComponent;
        }


#if UNITY_EDITOR
        // v1 No.52
        [MethodAPI]
        [APIDescriptionCN("Destroy(myScript.gameObject)")]
        [APIDescriptionEN("Destroy(myScript.gameObject)")]
        [APIExampleCode(@"
myScript.DestroyGameObj();
")]
#endif
        public static void DestroyGameObj<T>(this T selfBehaviour) where T : Component
        {
            selfBehaviour.gameObject.DestroySelf();
        }


#if UNITY_EDITOR
        // v1 No.53
        [MethodAPI]
        [APIDescriptionCN("if (myScript) Destroy(myScript.gameObject)")]
        [APIDescriptionEN("if (myScript) Destroy(myScript.gameObject)")]
        [APIExampleCode(@"
myScript.DestroyGameObjGracefully();
")]
#endif
        public static void DestroyGameObjGracefully<T>(this T selfBehaviour) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfGracefully();
            }
        }

#if UNITY_EDITOR
        // v1 No.54
        [MethodAPI]
        [APIDescriptionCN("Object.Destroy(myScript.gameObject,delaySeconds)")]
        [APIDescriptionEN("Object.Destroy(myScript.gameObject,delaySeconds)")]
        [APIExampleCode(@"
myScript.DestroyGameObjAfterDelay(5);
")]
#endif
        public static T DestroyGameObjAfterDelay<T>(this T selfBehaviour, float delay) where T : Component
        {
            selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            return selfBehaviour;
        }

#if UNITY_EDITOR
        // v1 No.55
        [MethodAPI]
        [APIDescriptionCN("if (myScript && myScript.gameObject) Object.Destroy(myScript.gameObject,delaySeconds)")]
        [APIDescriptionEN("if (myScript && myScript.gameObject) Object.Destroy(myScript.gameObject,delaySeconds)")]
        [APIExampleCode(@"
myScript.DestroyGameObjAfterDelayGracefully(5);
")]
#endif
        public static T DestroyGameObjAfterDelayGracefully<T>(this T selfBehaviour, float delay) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            }

            return selfBehaviour;
        }


#if UNITY_EDITOR
        // v1 No.56
        [MethodAPI]
        [APIDescriptionCN("gameObject.layer = layer")]
        [APIDescriptionEN("gameObject.layer = layer")]
        [APIExampleCode(@"
new GameObject().Layer(0);
")]
#endif
        public static GameObject Layer(this GameObject selfObj, int layer)
        {
            selfObj.layer = layer;
            return selfObj;
        }

#if UNITY_EDITOR
        // v1 No.57
        [MethodAPI]
        [APIDescriptionCN("component.gameObject.layer = layer")]
        [APIDescriptionEN("component.gameObject.layer = layer")]
        [APIExampleCode(@"
rigidbody2D.Layer(0);
")]
#endif
        public static T Layer<T>(this T selfComponent, int layer) where T : Component
        {
            selfComponent.gameObject.layer = layer;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.58
        [MethodAPI]
        [APIDescriptionCN("gameObj.layer = LayerMask.NameToLayer(layerName)")]
        [APIDescriptionEN("gameObj.layer = LayerMask.NameToLayer(layerName)")]
        [APIExampleCode(@"
new GameObject().Layer(""Default"");
")]
#endif

        public static GameObject Layer(this GameObject selfObj, string layerName)
        {
            selfObj.layer = LayerMask.NameToLayer(layerName);
            return selfObj;
        }

#if UNITY_EDITOR
        // v1 No.59
        [MethodAPI]
        [APIDescriptionCN("component.gameObject.layer = LayerMask.NameToLayer(layerName)")]
        [APIDescriptionEN("component.gameObject.layer = LayerMask.NameToLayer(layerName)")]
        [APIExampleCode(@"
spriteRenderer.Layer(""Default"");
")]
#endif
        public static T Layer<T>(this T selfComponent, string layerName) where T : Component
        {
            selfComponent.gameObject.layer = LayerMask.NameToLayer(layerName);
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.60
        [MethodAPI]
        [APIDescriptionCN("layerMask 中的层级是否包含 gameObj 所在的层级")]
        [APIDescriptionEN("Whether the layer in layerMask contains the same layer as gameObj")]
        [APIExampleCode(@"
gameObj.IsInLayerMask(layerMask);
")]
#endif
        public static bool IsInLayerMask(this GameObject selfObj, LayerMask layerMask)
        {
            return LayerMaskUtility.IsInLayerMask(selfObj, layerMask);
        }

#if UNITY_EDITOR
        // v1 No.61
        [MethodAPI]
        [APIDescriptionCN("layerMask 中的层级是否包含 component.gameObject 所在的层级")]
        [APIDescriptionEN("Whether the layer in layerMask contains the same layer as component.gameObject")]
        [APIExampleCode(@"
spriteRenderer.IsInLayerMask(layerMask);
")]
#endif
        public static bool IsInLayerMask<T>(this T selfComponent, LayerMask layerMask) where T : Component
        {
            return LayerMaskUtility.IsInLayerMask(selfComponent.gameObject, layerMask);
        }


#if UNITY_EDITOR
        // v1 No.62
        [MethodAPI]
        [APIDescriptionCN("获取组件，没有则添加再返回")]
        [APIDescriptionEN("Get component, add and return if not")]
        [APIExampleCode(@"
gameObj.GetOrAddComponent<SpriteRenderer>();
")]
#endif
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var comp = self.gameObject.GetComponent<T>();
            return comp ? comp : self.gameObject.AddComponent<T>();
        }

#if UNITY_EDITOR
        // v1 No.63
        [MethodAPI]
        [APIDescriptionCN("获取组件，没有则添加再返回")]
        [APIDescriptionEN("Get component, add and return if not")]
        [APIExampleCode(@"
component.GetOrAddComponent<SpriteRenderer>();
")]
#endif
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetOrAddComponent<T>();
        }

#if UNITY_EDITOR
        // v1 No.64
        [MethodAPI]
        [APIDescriptionCN("获取组件，没有则添加再返回")]
        [APIDescriptionEN("Get component, add and return if not")]
        [APIExampleCode(@"
gameObj.GetOrAddComponent(typeof(SpriteRenderer));
")]
#endif
        public static Component GetOrAddComponent(this GameObject self, Type type)
        {
            var component = self.gameObject.GetComponent(type);
            return component ? component : self.gameObject.AddComponent(type);
        }
    }

    public static class LayerMaskUtility
    {
        public static bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            var objLayerMask = 1 << layer;
            return (layerMask.value & objLayerMask) == objLayerMask;
        }

        public static bool IsInLayerMask(GameObject gameObj, LayerMask layerMask)
        {
            // 根据Layer数值进行移位获得用于运算的Mask值
            var objLayerMask = 1 << gameObj.layer;
            return (layerMask.value & objLayerMask) == objLayerMask;
        }
    }
}