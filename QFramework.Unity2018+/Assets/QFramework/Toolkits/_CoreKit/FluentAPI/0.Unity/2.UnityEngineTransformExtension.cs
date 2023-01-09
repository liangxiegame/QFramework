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
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Transform", 2)]
    [APIDescriptionCN("针对 UnityEngine.GameObject 提供的链式扩展")]
    [APIDescriptionEN("The chain extension provided by UnityEngine.Object.")]
    [APIExampleCode(@"
var selfScript = new GameObject().AddComponent<MonoBehaviour>();
var transform = selfScript.transform;

transform
    .Parent(null)
    .LocalIdentity()
    .LocalPositionIdentity()
    .LocalRotationIdentity()
    .LocalScaleIdentity()
    .LocalPosition(Vector3.zero)
    .LocalPosition(0, 0, 0)
    .LocalPosition(0, 0)
    .LocalPositionX(0)
    .LocalPositionY(0)
    .LocalPositionZ(0)
    .LocalRotation(Quaternion.identity)
    .LocalScale(Vector3.one)
    .LocalScaleX(1.0f)
    .LocalScaleY(1.0f)
    .Identity()
    .PositionIdentity()
    .RotationIdentity()
    .Position(Vector3.zero)
    .PositionX(0)
    .PositionY(0)
    .PositionZ(0)
    .Rotation(Quaternion.identity)
    .DestroyChildren()
    .AsLastSibling()
    .AsFirstSibling()
    .SiblingIndex(0);

selfScript
    .Parent(null)
    .LocalIdentity()
    .LocalPositionIdentity()
    .LocalRotationIdentity()
    .LocalScaleIdentity()
    .LocalPosition(Vector3.zero)
    .LocalPosition(0, 0, 0)
    .LocalPosition(0, 0)
    .LocalPositionX(0)
    .LocalPositionY(0)
    .LocalPositionZ(0)
    .LocalRotation(Quaternion.identity)
    .LocalScale(Vector3.one)
    .LocalScaleX(1.0f)
    .LocalScaleY(1.0f)
    .Identity()
    .PositionIdentity()
    .RotationIdentity()
    .Position(Vector3.zero)
    .PositionX(0)
    .PositionY(0)
    .PositionZ(0)
    .Rotation(Quaternion.identity)
    .DestroyChildren()
    .AsLastSibling()
    .AsFirstSibling()
    .SiblingIndex(0);
")]
#endif
    public static class UnityEngineTransformExtension
    {
#if UNITY_EDITOR
        // v1 No.65
        [MethodAPI]
        [APIDescriptionCN("component.transform.SetParent(parent)")]
        [APIDescriptionEN("component.transform.SetParent(parent)")]
        [APIExampleCode(@"
myScript.Parent(rootGameObj);
")]
#endif
        public static T Parent<T>(this T self, Component parentComponent) where T : Component
        {
            self.transform.SetParent(parentComponent == null ? null : parentComponent.transform);
            return self;
        }

#if UNITY_EDITOR
        // v1 No.66
        [MethodAPI]
        [APIDescriptionCN("gameObject.transform.SetParent(parent)")]
        [APIDescriptionEN("gameObject.transform.SetParent(parent)")]
        [APIExampleCode(@"
gameObj.SetParent(null);
")]
#endif
        public static GameObject Parent(this GameObject self, Component parentComponent)
        {
            self.transform.SetParent(parentComponent == null ? null : parentComponent.transform);
            return self;
        }

#if UNITY_EDITOR
        // v1 No.67
        [MethodAPI]
        [APIDescriptionCN("component.transform.SetParent(null)")]
        [APIDescriptionEN("component.transform.SetParent(null)")]
        [APIExampleCode(@"
component.AsRootTransform();
")]
#endif
        public static T AsRootTransform<T>(this T self) where T : Component
        {
            self.transform.SetParent(null);
            return self;
        }

#if UNITY_EDITOR
        // v1 No.68
        [MethodAPI]
        [APIDescriptionCN("gameObject.transform.SetParent(null)")]
        [APIDescriptionEN("gameObject.transform.SetParent(null)")]
        [APIExampleCode(@"
gameObject.AsRootGameObject();
")]
#endif
        public static GameObject AsRootGameObject<T>(this GameObject self)
        {
            self.transform.SetParent(null);
            return self;
        }


#if UNITY_EDITOR
        // v1 No.69
        [MethodAPI]
        [APIDescriptionCN("设置本地位置为 0、本地角度为 0、本地缩放为 1")]
        [APIDescriptionEN("set local pos:0 local angle:0 local scale:1")]
        [APIExampleCode(@"
myScript.LocalIdentity();
")]
#endif
        public static T LocalIdentity<T>(this T self) where T : Component
        {
            self.transform.localPosition = Vector3.zero;
            self.transform.localRotation = Quaternion.identity;
            self.transform.localScale = Vector3.one;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.70
        [MethodAPI]
        [APIDescriptionCN("设置 gameObject 的本地位置为 0、本地角度为 0、本地缩放为 1")]
        [APIDescriptionEN("set gameObject's local pos:0  local angle:0  local scale:1")]
        [APIExampleCode(@"
myScript.LocalIdentity();
")]
#endif
        public static GameObject LocalIdentity(this GameObject self)
        {
            self.transform.localPosition = Vector3.zero;
            self.transform.localRotation = Quaternion.identity;
            self.transform.localScale = Vector3.one;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.71
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition = localPosition")]
        [APIDescriptionEN("component.transform.localPosition = localPosition")]
        [APIExampleCode(@"
spriteRenderer.LocalPosition(new Vector3(0,100,0));
")]
#endif
        public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
        {
            selfComponent.transform.localPosition = localPos;
            return selfComponent;
        }
#if UNITY_EDITOR
        // v1 No.72
        [MethodAPI]
        [APIDescriptionCN("gameObject.transform.localPosition = localPosition")]
        [APIDescriptionEN("gameObject.transform.localPosition = localPosition")]
        [APIExampleCode(@"
new GameObject().LocalPosition(new Vector3(0,100,0));
")]
#endif
        public static GameObject LocalPosition(this GameObject self, Vector3 localPos)
        {
            self.transform.localPosition = localPos;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.73
        [MethodAPI]
        [APIDescriptionCN("return component.transform.localPosition")]
        [APIDescriptionEN("return component.transform.localPosition")]
        [APIExampleCode(@"
var localPosition = spriteRenderer.LocalPosition();
")]
#endif
        public static Vector3 LocalPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localPosition;
        }

#if UNITY_EDITOR
        // v1 No.74
        [MethodAPI]
        [APIDescriptionCN("return gameObject.transform.localPosition")]
        [APIDescriptionEN("return gameObject.transform.localPosition")]
        [APIExampleCode(@"
Debug.Log(new GameObject().LocalPosition());
")]
#endif
        public static Vector3 LocalPosition(this GameObject self)
        {
            return self.transform.localPosition;
        }


#if UNITY_EDITOR
        // v1 No.75
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition = new Vector3(x,y,z)")]
        [APIDescriptionEN("component.transform.localPosition = new Vector3(x,y,z)")]
        [APIExampleCode(@"
myScript.LocalPosition(0,0,-10);
")]
#endif
        public static T LocalPosition<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.localPosition = new Vector3(x, y, z);
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.76
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localPosition = new Vector3(x,y,z)")]
        [APIDescriptionEN("gameObj.transform.localPosition = new Vector3(x,y,z)")]
        [APIExampleCode(@"
new GameObject().LocalPosition(0,0,-10);
")]
#endif
        public static GameObject LocalPosition(this GameObject self, float x, float y, float z)
        {
            self.transform.localPosition = new Vector3(x, y, z);
            return self;
        }


#if UNITY_EDITOR
        // v1 No.77
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition = new Vector3(x,y,component.transform.localPosition.z)")]
        [APIDescriptionEN("component.transform.localPosition = new Vector3(x,y,component.transform.localPosition.z)")]
        [APIExampleCode(@"
myScript.LocalPosition(0,0);
")]
#endif
        public static T LocalPosition<T>(this T selfComponent, float x, float y) where T : Component
        {
            var localPosition = selfComponent.transform.localPosition;
            localPosition.x = x;
            localPosition.y = y;
            selfComponent.transform.localPosition = localPosition;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.78
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localPosition = new Vector3(x,y,gameObj.transform.localPosition.z)")]
        [APIDescriptionEN("gameObj.transform.localPosition = new Vector3(x,y,gameObj.transform.localPosition.z)")]
        [APIExampleCode(@"
new GameObject().LocalPosition(0,0);
")]
#endif
        public static GameObject LocalPosition(this GameObject self, float x, float y)
        {
            var localPosition = self.transform.localPosition;
            localPosition.x = x;
            localPosition.y = y;
            self.transform.localPosition = localPosition;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.79
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition.x = x")]
        [APIDescriptionEN("component.transform.localPosition.x = x")]
        [APIExampleCode(@"
component.LocalPositionX(10);
")]
#endif
        public static T LocalPositionX<T>(this T selfComponent, float x) where T : Component
        {
            var localPosition = selfComponent.transform.localPosition;
            localPosition.x = x;
            selfComponent.transform.localPosition = localPosition;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.80
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localPosition.x = x")]
        [APIDescriptionEN("gameObj.transform.localPosition.x = x")]
        [APIExampleCode(@"
gameObj.LocalPositionX(10);
")]
#endif
        public static GameObject LocalPositionX(this GameObject self, float x)
        {
            var localPosition = self.transform.localPosition;
            localPosition.x = x;
            self.transform.localPosition = localPosition;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.81
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition.y = y")]
        [APIDescriptionEN("component.transform.localPosition.y = y")]
        [APIExampleCode(@"
component.LocalPositionY(10);
")]
#endif
        public static T LocalPositionY<T>(this T selfComponent, float y) where T : Component
        {
            var localPosition = selfComponent.transform.localPosition;
            localPosition.y = y;
            selfComponent.transform.localPosition = localPosition;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.82
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localPosition.y = y")]
        [APIDescriptionEN("gameObj.transform.localPosition.y = y")]
        [APIExampleCode(@"
gameObj.LocalPositionY(10);
")]
#endif

        public static GameObject LocalPositionY(this GameObject selfComponent, float y)
        {
            var localPosition = selfComponent.transform.localPosition;
            localPosition.y = y;
            selfComponent.transform.localPosition = localPosition;
            return selfComponent;
        }


#if UNITY_EDITOR
        // v1 No.83
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition.z = z")]
        [APIDescriptionEN("component.transform.localPosition.z = z")]
        [APIExampleCode(@"
component.LocalPositionZ(10);
")]
#endif
        public static T LocalPositionZ<T>(this T selfComponent, float z) where T : Component
        {
            var localPosition = selfComponent.transform.localPosition;
            localPosition.z = z;
            selfComponent.transform.localPosition = localPosition;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.84
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localPosition.z = z")]
        [APIDescriptionEN("gameObj.transform.localPosition.z = z")]
        [APIExampleCode(@"
gameObj.LocalPositionZ(10);
")]
#endif
        public static GameObject LocalPositionZ(this GameObject self, float z)
        {
            var localPosition = self.transform.localPosition;
            localPosition.z = z;
            self.transform.localPosition = localPosition;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.85
        [MethodAPI]
        [APIDescriptionCN("component.transform.localPosition = Vector3.zero")]
        [APIDescriptionEN("component.transform.localPosition = Vector3.zero")]
        [APIExampleCode(@"
component.LocalPositionIdentity();
")]
#endif
        public static T LocalPositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.86
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localPosition = Vector3.zero")]
        [APIDescriptionEN("gameObj.transform.localPosition = Vector3.zero")]
        [APIExampleCode(@"
gameObj.LocalPositionIdentity();
")]
#endif
        public static GameObject LocalPositionIdentity(this GameObject self)
        {
            self.transform.localPosition = Vector3.zero;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.87
        [MethodAPI]
        [APIDescriptionCN("return component.transform.localRotation")]
        [APIDescriptionEN("return component.transform.localRotation")]
        [APIExampleCode(@"
var localRotation = myScript.LocalRotation();
")]
#endif
        public static Quaternion LocalRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localRotation;
        }

#if UNITY_EDITOR
        // v1 No.88
        [MethodAPI]
        [APIDescriptionCN("return gameObj.transform.localRotation")]
        [APIDescriptionEN("return gameObj.transform.localRotation")]
        [APIExampleCode(@"
var localRotation = gameObj.LocalRotation();
")]
#endif
        public static Quaternion LocalRotation(this GameObject self)
        {
            return self.transform.localRotation;
        }

#if UNITY_EDITOR
        // v1 No.89
        [MethodAPI]
        [APIDescriptionCN("component.transform.localRotation = localRotation")]
        [APIDescriptionEN("component.transform.localRotation = localRotation")]
        [APIExampleCode(@"
myScript.LocalRotation(Quaternion.identity);
")]
#endif
        public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
        {
            selfComponent.transform.localRotation = localRotation;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.90
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localRotation = localRotation")]
        [APIDescriptionEN("gameObj.transform.localRotation = localRotation")]
        [APIExampleCode(@"
gameObj.LocalRotation(Quaternion.identity);
")]
#endif
        public static GameObject LocalRotation(this GameObject selfComponent, Quaternion localRotation)
        {
            selfComponent.transform.localRotation = localRotation;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.91
        [MethodAPI]
        [APIDescriptionCN("component.transform.localRotation = Quaternion.identity")]
        [APIDescriptionEN("component.transform.localRotation = Quaternion.identity")]
        [APIExampleCode(@"
component.LocalRotationIdentity();
")]
#endif
        public static T LocalRotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localRotation = Quaternion.identity;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.92
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localRotation = Quaternion.identity")]
        [APIDescriptionEN("gameObj.transform.localRotation = Quaternion.identity")]
        [APIExampleCode(@"
gameObj.LocalRotationIdentity();
")]
#endif
        public static GameObject LocalRotationIdentity(this GameObject selfComponent)
        {
            selfComponent.transform.localRotation = Quaternion.identity;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.93
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale = scale")]
        [APIDescriptionEN("component.transform.localScale = scale")]
        [APIExampleCode(@"
component.LocalScale(Vector3.one);
")]
#endif
        public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
        {
            selfComponent.transform.localScale = scale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.94
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale = scale")]
        [APIDescriptionEN("gameObj.transform.localScale = scale")]
        [APIExampleCode(@"
gameObj.LocalScale(Vector3.one);
")]
#endif
        public static GameObject LocalScale(this GameObject self, Vector3 scale)
        {
            self.transform.localScale = scale;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.95
        [MethodAPI]
        [APIDescriptionCN("return component.transform.localScale")]
        [APIDescriptionEN("return component.transform.localScale")]
        [APIExampleCode(@"
var localScale = myScript.LocalScale();
")]
#endif
        public static Vector3 LocalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localScale;
        }

#if UNITY_EDITOR
        // v1 No.96
        [MethodAPI]
        [APIDescriptionCN("return gameObj.transform.localScale")]
        [APIDescriptionEN("return gameObj.transform.localScale")]
        [APIExampleCode(@"
var localScale = gameObj.LocalScale();
")]
#endif
        public static Vector3 LocalScale(this GameObject self)
        {
            return self.transform.localScale;
        }


#if UNITY_EDITOR
        // v1 No.97
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale = new Vector3(xyz,xyz,xyz)")]
        [APIDescriptionEN("component.transform.localScale = new Vector3(xyz,xyz,xyz)")]
        [APIExampleCode(@"
myScript.LocalScale(1);
")]
#endif
        public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one * xyz;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.98
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale = new Vector3(scale,scale,scale)")]
        [APIDescriptionEN("gameObj.transform.localScale = new Vector3(scale,scale,scale)")]
        [APIExampleCode(@"
gameObj.LocalScale(1);
")]
#endif
        public static GameObject LocalScale(this GameObject self, float xyz)
        {
            self.transform.localScale = Vector3.one * xyz;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.99
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale = new Vector3(x,y,z)")]
        [APIDescriptionEN("component.transform.localScale = new Vector3(x,y,z)")]
        [APIExampleCode(@"
myScript.LocalScale(2,2,2);
")]
#endif
        public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            var localScale = selfComponent.transform.localScale;
            localScale.x = x;
            localScale.y = y;
            localScale.z = z;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.100
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale = new Vector3(x,y,z)")]
        [APIDescriptionEN("gameObj.transform.localScale = new Vector3(x,y,z)")]
        [APIExampleCode(@"
gameObj.LocalScale(2,2,2);
")]
#endif
        public static GameObject LocalScale(this GameObject selfComponent, float x, float y, float z)
        {
            var localScale = selfComponent.transform.localScale;
            localScale.x = x;
            localScale.y = y;
            localScale.z = z;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.101
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale = new Vector3(x,y,component.transform.localScale.z)")]
        [APIDescriptionEN("component.transform.localScale = new Vector3(x,y,component.transform.localScale.z)")]
        [APIExampleCode(@"
component.LocalScale(2,2);
")]
#endif
        public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
        {
            var localScale = selfComponent.transform.localScale;
            localScale.x = x;
            localScale.y = y;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.102
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale = new Vector3(x,y,gameObj.transform.localScale.z)")]
        [APIDescriptionEN("gameObj.transform.localScale = new Vector3(x,y,gameObj.transform.localScale.z)")]
        [APIExampleCode(@"
gameObj.LocalScale(2,2);
")]
#endif
        public static GameObject LocalScale(this GameObject selfComponent, float x, float y)
        {
            var localScale = selfComponent.transform.localScale;
            localScale.x = x;
            localScale.y = y;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.103
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale.x = x")]
        [APIDescriptionEN("component.transform.localScale.x = x)")]
        [APIExampleCode(@"
component.LocalScaleX(10);
")]
#endif
        public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
        {
            var localScale = selfComponent.transform.localScale;
            localScale.x = x;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }


#if UNITY_EDITOR
        // v1 No.104
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale.x = x")]
        [APIDescriptionEN("gameObj.transform.localScale.x = x)")]
        [APIExampleCode(@"
gameObj.LocalScaleX(10);
")]
#endif
        public static GameObject LocalScaleX(this GameObject self, float x)
        {
            var localScale = self.transform.localScale;
            localScale.x = x;
            self.transform.localScale = localScale;
            return self;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale.x")]
        [APIDescriptionEN("gameObj.transform.localScale.x)")]
        [APIExampleCode(@"
var scaleX = gameObj.LocalScaleX();
")]
#endif
        public static float LocalScaleX(this GameObject self)
        {
            return self.transform.localScale.x;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale.x")]
        [APIDescriptionEN("component.transform.localScale.x)")]
        [APIExampleCode(@"
var scaleX = component.LocalScaleX();
")]
#endif
        public static float LocalScaleX<T>(this T self) where T : Component
        {
            return self.transform.localScale.x;
        }


#if UNITY_EDITOR
        // v1 No.105
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale.y = y")]
        [APIDescriptionEN("component.transform.localScale.y = y)")]
        [APIExampleCode(@"
component.LocalScaleY(10);
")]
#endif
        public static T LocalScaleY<T>(this T self, float y) where T : Component
        {
            var localScale = self.transform.localScale;
            localScale.y = y;
            self.transform.localScale = localScale;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.106
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale.y = y")]
        [APIDescriptionEN("gameObj.transform.localScale.y = y)")]
        [APIExampleCode(@"
gameObj.LocalScaleY(10);
")]
#endif
        public static GameObject LocalScaleY(this GameObject self, float y)
        {
            var localScale = self.transform.localScale;
            localScale.y = y;
            self.transform.localScale = localScale;
            return self;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale.y")]
        [APIDescriptionEN("component.transform.localScale.y)")]
        [APIExampleCode(@"
var scaleY = component.LocalScaleY(10);
")]
#endif
        public static float LocalScaleY<T>(this T self) where T : Component
        {
            return self.transform.localScale.y;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale.y")]
        [APIDescriptionEN("gameObj.transform.localScale.y)")]
        [APIExampleCode(@"
var scaleY = gameObj.LocalScaleY();
")]
#endif
        public static float LocalScaleY(this GameObject self)
        {
            return self.transform.localScale.y;
        }


#if UNITY_EDITOR
        // v1 No.107
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale.z = z")]
        [APIDescriptionEN("component.transform.localScale.z = z)")]
        [APIExampleCode(@"
component.LocalScaleZ(10);
")]
#endif
        public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
        {
            var localScale = selfComponent.transform.localScale;
            localScale.z = z;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.108
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale.z = z")]
        [APIDescriptionEN("gameObj.transform.localScale.z = z)")]
        [APIExampleCode(@"
gameObj.LocalScaleZ(10);
")]
#endif
        public static GameObject LocalScaleZ(this GameObject selfComponent, float z)
        {
            var localScale = selfComponent.transform.localScale;
            localScale.z = z;
            selfComponent.transform.localScale = localScale;
            return selfComponent;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale.z")]
        [APIDescriptionEN("component.transform.localScale.z)")]
        [APIExampleCode(@"
var scaleZ = component.LocalScaleZ();
")]
#endif
        public static float LocalScaleZ<T>(this T self) where T : Component
        {
            return self.transform.localScale.z;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.localScale.z")]
        [APIDescriptionEN("gameObj.transform.localScale.z)")]
        [APIExampleCode(@"
var scaleZ = gameObj.LocalScaleZ();
")]
#endif
        public static float LocalScaleZ(this GameObject self)
        {
            return self.transform.localScale.z;
        }


#if UNITY_EDITOR
        // v1 No.109
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale = Vector3.one")]
        [APIDescriptionEN("component.transform.localScale = Vector3.one)")]
        [APIExampleCode(@"
component.LocalScaleIdentity();
")]
#endif
        public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }
#if UNITY_EDITOR
        // v1 No.110
        [MethodAPI]
        [APIDescriptionCN("component.transform.localScale = Vector3.one")]
        [APIDescriptionEN("component.transform.localScale = Vector3.one)")]
        [APIExampleCode(@"
component.LocalScaleIdentity();
")]
#endif
        public static GameObject LocalScaleIdentity(this GameObject selfComponent)
        {
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.111
        [MethodAPI]
        [APIDescriptionCN("设置世界位置:0 角度:0 缩放:1")]
        [APIDescriptionEN("set world pos:0 rotation:0 scale:1")]
        [APIExampleCode(@"
component.Identity();
")]
#endif
        public static T Identity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            selfComponent.transform.rotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.112
        [MethodAPI]
        [APIDescriptionCN("设置世界位置:0 角度:0 缩放:1")]
        [APIDescriptionEN("set world pos:0 rotation:0 scale:1")]
        [APIExampleCode(@"
component.Identity();
")]
#endif
        public static GameObject Identity(this GameObject self)
        {
            self.transform.position = Vector3.zero;
            self.transform.rotation = Quaternion.identity;
            self.transform.localScale = Vector3.one;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.113
        [MethodAPI]
        [APIDescriptionCN("component.transform.position = position")]
        [APIDescriptionEN("component.transform.position = position")]
        [APIExampleCode(@"
component.Position(Vector3.zero);
")]
#endif
        public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
        {
            selfComponent.transform.position = position;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.114
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.position = position")]
        [APIDescriptionEN("gameObj.transform.position = position")]
        [APIExampleCode(@"
gameObj.Position(Vector3.zero);
")]
#endif
        public static GameObject Position(this GameObject self, Vector3 position)
        {
            self.transform.position = position;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.115
        [MethodAPI]
        [APIDescriptionCN("return component.transform.position")]
        [APIDescriptionEN("return component.transform.position")]
        [APIExampleCode(@"
var pos = myScript.Position();
")]
#endif
        public static Vector3 Position<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.position;
        }

#if UNITY_EDITOR
        // v1 No.116
        [MethodAPI]
        [APIDescriptionCN("return gameObj.transform.position")]
        [APIDescriptionEN("return gameObj.transform.position")]
        [APIExampleCode(@"
var pos = gameObj.Position();
")]
#endif
        public static Vector3 Position(this GameObject self)
        {
            return self.transform.position;
        }

#if UNITY_EDITOR
        // v1 No.117
        [MethodAPI]
        [APIDescriptionCN("component.transform.position = new Vector3(x,y,z)")]
        [APIDescriptionEN("component.transform.position = new Vector3(x,y,z)")]
        [APIExampleCode(@"
myScript.Position(0,0,-10);
")]
#endif
        public static T Position<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.position = new Vector3(x, y, z);
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.118
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.position = new Vector3(x,y,z)")]
        [APIDescriptionEN("gameObj.transform.position = new Vector3(x,y,z)")]
        [APIExampleCode(@"
gameObj.Position(0,0,-10);
")]
#endif
        public static GameObject Position(this GameObject self, float x, float y, float z)
        {
            self.transform.position = new Vector3(x, y, z);
            return self;
        }

#if UNITY_EDITOR
        // v1 No.119
        [MethodAPI]
        [APIDescriptionCN("component.transform.position = new Vector3(x,y,原来的 z)")]
        [APIDescriptionEN("component.transform.position = new Vector3(x,y,origin z)")]
        [APIExampleCode(@"
component.Position(0,0);
")]
#endif
        public static T Position<T>(this T selfComponent, float x, float y) where T : Component
        {
            var position = selfComponent.transform.position;
            position.x = x;
            position.y = y;
            selfComponent.transform.position = position;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.120
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.position = new Vector3(x,y,原来的 z)")]
        [APIDescriptionEN("gameObj.transform.position = new Vector3(x,y,origin z)")]
        [APIExampleCode(@"
gameObj.Position(0,0,-10);
")]
#endif
        public static GameObject Position(this GameObject selfComponent, float x, float y)
        {
            var position = selfComponent.transform.position;
            position.x = x;
            position.y = y;
            selfComponent.transform.position = position;
            return selfComponent;
        }


#if UNITY_EDITOR
        // v1 No.121
        [MethodAPI]
        [APIDescriptionCN("component.transform.position = Vector3.zero")]
        [APIDescriptionEN("component.transform.position = Vector3.zero")]
        [APIExampleCode(@"
component.PositionIdentity();
")]
#endif
        public static T PositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.122
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.position = Vector3.zero")]
        [APIDescriptionEN("gameObj.transform.position = Vector3.zero")]
        [APIExampleCode(@"
gameObj.PositionIdentity();
")]
#endif
        public static GameObject PositionIdentity(this GameObject selfComponent)
        {
            selfComponent.transform.position = Vector3.zero;
            return selfComponent;
        }


#if UNITY_EDITOR
        // v1 No.121
        [MethodAPI]
        [APIDescriptionCN("component.transform.position.x = x")]
        [APIDescriptionEN("component.transform.position.x = x")]
        [APIExampleCode(@"
component.PositionX(x);
")]
#endif
        public static T PositionX<T>(this T selfComponent, float x) where T : Component
        {
            var position = selfComponent.transform.position;
            position.x = x;
            selfComponent.transform.position = position;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.122
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.position.x = x")]
        [APIDescriptionEN("gameObj.transform.position.x = x")]
        [APIExampleCode(@"
gameObj.PositionX(x);
")]
#endif
        public static GameObject PositionX(this GameObject self, float x)
        {
            var position = self.transform.position;
            position.x = x;
            self.transform.position = position;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.123
        [MethodAPI]
        [APIDescriptionCN("将 positionX 的计算结果设置给 position.x")]
        [APIDescriptionEN("Sets the positionX calculation to position.x")]
        [APIExampleCode(@"
component.PositionX(x=>x * 5);
")]
#endif
        public static T PositionX<T>(this T selfComponent, Func<float, float> xSetter) where T : Component
        {
            var position = selfComponent.transform.position;
            position.x = xSetter(position.x);
            selfComponent.transform.position = position;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.124
        [MethodAPI]
        [APIDescriptionCN("将 positionX 的计算结果设置给 position.x")]
        [APIDescriptionEN("Sets the positionX calculation to position.x")]
        [APIExampleCode(@"
gameObj.PositionX(x=>x * 5);
")]
#endif
        public static GameObject PositionX(this GameObject self, Func<float, float> xSetter)
        {
            var position = self.transform.position;
            position.x = xSetter(position.x);
            self.transform.position = position;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.125
        [MethodAPI]
        [APIDescriptionCN("component.transform.position.y = y")]
        [APIDescriptionEN("component.transform.position.y = y")]
        [APIExampleCode(@"
myScript.PositionY(10);
")]
#endif
        public static T PositionY<T>(this T selfComponent, float y) where T : Component
        {
            var position = selfComponent.transform.position;
            position.y = y;
            selfComponent.transform.position = position;
            return selfComponent;
        }
#if UNITY_EDITOR
        // v1 No.126
        [MethodAPI]
        [APIDescriptionCN("component.transform.position.y = y")]
        [APIDescriptionEN("component.transform.position.y = y")]
        [APIExampleCode(@"
myScript.PositionY(10);
")]
#endif
        public static GameObject PositionY(this GameObject self, float y)
        {
            var position = self.transform.position;
            position.y = y;
            self.transform.position = position;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.127
        [MethodAPI]
        [APIDescriptionCN("将 positionY 的计算结果设置给 position.y")]
        [APIDescriptionEN("Sets the positionY calculation to position.y")]
        [APIExampleCode(@"
component.PositionY(y=>y * 5);
")]
#endif
        public static T PositionY<T>(this T selfComponent, Func<float, float> ySetter) where T : Component
        {
            var position = selfComponent.transform.position;
            position.y = ySetter(position.y);
            selfComponent.transform.position = position;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.128
        [MethodAPI]
        [APIDescriptionCN("将 positionY 的计算结果设置给 position.y")]
        [APIDescriptionEN("Sets the positionY calculation to position.y")]
        [APIExampleCode(@"
gameObj.PositionY(y=>y * 5);
")]
#endif
        public static GameObject PositionY(this GameObject self, Func<float, float> ySetter)
        {
            var position = self.transform.position;
            position.y = ySetter(position.y);
            self.transform.position = position;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.129
        [MethodAPI]
        [APIDescriptionCN("component.transform.position.z = z")]
        [APIDescriptionEN("component.transform.position.z = z")]
        [APIExampleCode(@"
component.PositionZ(10);
")]
#endif
        public static T PositionZ<T>(this T selfComponent, float z) where T : Component
        {
            var position = selfComponent.transform.position;
            position.z = z;
            selfComponent.transform.position = position;
            return selfComponent;
        }
#if UNITY_EDITOR
        // v1 No.130
        [MethodAPI]
        [APIDescriptionCN("component.transform.position.z = z")]
        [APIDescriptionEN("component.transform.position.z = z")]
        [APIExampleCode(@"
component.PositionZ(10);
")]
#endif
        public static GameObject PositionZ(this GameObject self, float z)
        {
            var position = self.transform.position;
            position.z = z;
            self.transform.position = position;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.131
        [MethodAPI]
        [APIDescriptionCN("将 positionZ 的计算结果设置给 position.z")]
        [APIDescriptionEN("Sets the positionZ calculation to position.z")]
        [APIExampleCode(@"
component.PositionZ(z=>z * 5);
")]
#endif
        public static T PositionZ<T>(this T self, Func<float, float> zSetter) where T : Component
        {
            var position = self.transform.position;
            position.z = zSetter(position.z);
            self.transform.position = position;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.132
        [MethodAPI]
        [APIDescriptionCN("将 positionZ 的计算结果设置给 position.z")]
        [APIDescriptionEN("Sets the positionZ calculation to position.z")]
        [APIExampleCode(@"
component.PositionZ(z=>z * 5);
")]
#endif
        public static GameObject PositionZ(this GameObject self, Func<float, float> zSetter)
        {
            var position = self.transform.position;
            position.z = zSetter(position.z);
            self.transform.position = position;
            return self;
        }


#if UNITY_EDITOR
        // v1 No.133
        [MethodAPI]
        [APIDescriptionCN("component.transform.rotation = Quaternion.identity")]
        [APIDescriptionEN("component.transform.rotation = Quaternion.identity")]
        [APIExampleCode(@"
component.RotationIdentity();
")]
#endif
        public static T RotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.rotation = Quaternion.identity;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.134
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.rotation = Quaternion.identity")]
        [APIDescriptionEN("gameObj.transform.rotation = Quaternion.identity")]
        [APIExampleCode(@"
gameObj.RotationIdentity();
")]
#endif
        public static GameObject RotationIdentity(this GameObject selfComponent)
        {
            selfComponent.transform.rotation = Quaternion.identity;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.135
        [MethodAPI]
        [APIDescriptionCN("component.transform.rotation = rotation")]
        [APIDescriptionEN("component.transform.rotation = rotation")]
        [APIExampleCode(@"
component.Rotation(Quaternion.identity);
")]
#endif
        public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
        {
            selfComponent.transform.rotation = rotation;
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.136
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.rotation = rotation")]
        [APIDescriptionEN("gameObj.transform.rotation = rotation")]
        [APIExampleCode(@"
gameObj.Rotation(Quaternion.identity);
")]
#endif
        public static GameObject Rotation(this GameObject self, Quaternion rotation)
        {
            self.transform.rotation = rotation;
            return self;
        }

#if UNITY_EDITOR
        // v1 No.137
        [MethodAPI]
        [APIDescriptionCN("return component.transform.rotation")]
        [APIDescriptionEN("return component.transform.rotation")]
        [APIExampleCode(@"
var rotation = myScript.Rotation();
")]
#endif
        public static Quaternion Rotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.rotation;
        }

#if UNITY_EDITOR
        // v1 No.138
        [MethodAPI]
        [APIDescriptionCN("return gameObj.transform.rotation")]
        [APIDescriptionEN("return gameObj.transform.rotation")]
        [APIExampleCode(@"
var rotation = gameObj.Rotation();
")]
#endif
        public static Quaternion Rotation(this GameObject self)
        {
            return self.transform.rotation;
        }


#if UNITY_EDITOR
        // v1 No.139
        [MethodAPI]
        [APIDescriptionCN("return component.transform.lossyScale")]
        [APIDescriptionEN("return component.transform.lossyScale")]
        [APIExampleCode(@"
var scale = component.Scale();
")]
#endif
        public static Vector3 Scale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

#if UNITY_EDITOR
        // v1 No.140
        [MethodAPI]
        [APIDescriptionCN("return gameObj.transform.lossyScale")]
        [APIDescriptionEN("return gameObj.transform.lossyScale")]
        [APIExampleCode(@"
var scale = gameObj.Scale();
")]
#endif
        public static Vector3 Scale(this GameObject selfComponent)
        {
            return selfComponent.transform.lossyScale;
        }


#if UNITY_EDITOR
        // v1 No.141
        [MethodAPI]
        [APIDescriptionCN("Destroy 掉所有的子 GameObject")]
        [APIDescriptionEN("destroy all child gameObjects")]
        [APIExampleCode(@"
rootTransform.DestroyChildren();
")]
#endif
        public static T DestroyChildren<T>(this T selfComponent) where T : Component
        {
            var childCount = selfComponent.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.141.1
        [MethodAPI]
        [APIDescriptionCN("根据条件 Destroy 掉所有的子 GameObject ")]
        [APIDescriptionEN("destroy all child gameObjects if condition matched")]
        [APIExampleCode(@"
rootTransform.DestroyChildrenWithCondition(child=>child != other);
")]
#endif
        public static T DestroyChildrenWithCondition<T>(this T selfComponent, Func<Transform, bool> condition)
            where T : Component
        {
            var childCount = selfComponent.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = selfComponent.transform.GetChild(i);
                if (condition(child))
                {
                    child.DestroyGameObjGracefully();
                }
            }

            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.142
        [MethodAPI]
        [APIDescriptionCN("Destroy 掉所有的子 GameObject")]
        [APIDescriptionEN("destroy all child gameObjects")]
        [APIExampleCode(@"
rootGameObj.DestroyChildren();
")]
#endif
        public static GameObject DestroyChildren(this GameObject selfGameObj)
        {
            var childCount = selfGameObj.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfGameObj.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfGameObj;
        }


#if UNITY_EDITOR
        // v1 No.143
        [MethodAPI]
        [APIDescriptionCN("component.transform.SetAsLastSibling()")]
        [APIDescriptionEN("component.transform.SetAsLastSibling()")]
        [APIExampleCode(@"
myScript.AsLastSibling();
")]
#endif
        public static T AsLastSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsLastSibling();
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.144
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.SetAsLastSibling()")]
        [APIDescriptionEN("gameObj.transform.SetAsLastSibling()")]
        [APIExampleCode(@"
gameObj.AsLastSibling();
")]
#endif
        public static GameObject AsLastSibling(this GameObject self)
        {
            self.transform.SetAsLastSibling();
            return self;
        }

#if UNITY_EDITOR
        // v1 No.145
        [MethodAPI]
        [APIDescriptionCN("component.transform.SetAsFirstSibling()")]
        [APIDescriptionEN("component.transform.SetAsFirstSibling()")]
        [APIExampleCode(@"
component.AsFirstSibling();
")]
#endif
        public static T AsFirstSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsFirstSibling();
            return selfComponent;
        }
#if UNITY_EDITOR
        // v1 No.146
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.SetAsFirstSibling()")]
        [APIDescriptionEN("gameObj.transform.SetAsFirstSibling()")]
        [APIExampleCode(@"
gameObj.AsFirstSibling();
")]
#endif
        public static GameObject AsFirstSibling(this GameObject selfComponent)
        {
            selfComponent.transform.SetAsFirstSibling();
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.147
        [MethodAPI]
        [APIDescriptionCN("component.transform.SetSiblingIndex(index)")]
        [APIDescriptionEN("component.transform.SetSiblingIndex(index)")]
        [APIExampleCode(@"
myScript.SiblingIndex(10);
")]
#endif
        public static T SiblingIndex<T>(this T selfComponent, int index) where T : Component
        {
            selfComponent.transform.SetSiblingIndex(index);
            return selfComponent;
        }

#if UNITY_EDITOR
        // v1 No.148
        [MethodAPI]
        [APIDescriptionCN("gameObj.transform.SetSiblingIndex(index)")]
        [APIDescriptionEN("gameObj.transform.SetSiblingIndex(index)")]
        [APIExampleCode(@"
gameObj.SiblingIndex(10);
")]
#endif
        public static GameObject SiblingIndex(this GameObject selfComponent, int index)
        {
            selfComponent.transform.SetSiblingIndex(index);
            return selfComponent;
        }
    }
}