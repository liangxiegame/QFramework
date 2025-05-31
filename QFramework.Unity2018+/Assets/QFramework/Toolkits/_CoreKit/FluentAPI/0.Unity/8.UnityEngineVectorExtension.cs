/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Vector2/3", 8)]
    [APIDescriptionCN("针对 Vector2/Vector3 封装的函数")]
    [APIDescriptionEN("wrapper function for Vector2/Vector3")]
    [APIExampleCode(@"
    gameObjA.DirectionFrom(gameObjB);
    myComponentA.DirectionFrom(gameObjB);
    gameObjA.DirectionFrom(myComponentB);
    myComponentA.DirectionFrom(myComponentB);

    // also support DirectionTo/ NormalizedDirectionFrom /NormalizedDirectionTo
")]
#endif
    public static class UnityEngineVectorExtension 
    {
        #if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("to.transform.position - self.transform.position")]
        [APIDescriptionEN("to.transform.position - self.transform.position")]
        [APIExampleCode(@"
gameObj/otherComponent.DirectionTo(otherGameObj/otherComponent)
")]
#endif
        public static Vector3 DirectionTo(this Component self, Component to) =>
            to.transform.position - self.transform.position;

        public static Vector3 DirectionTo(this GameObject self, GameObject to) =>
            to.transform.position - self.transform.position;

        public static Vector3 DirectionTo(this Component self, GameObject to) =>
            to.transform.position - self.transform.position;

        public static Vector3 DirectionTo(this GameObject self, Component to) =>
            to.transform.position - self.transform.position;

#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("self.transform.position - from.transform.position")]
        [APIDescriptionEN("self.transform.position - from.transform.position")]
        [APIExampleCode(@"
gameObj/otherComponent.DirectionFrom(otherGameObj/otherComponent)
")]
#endif
        public static Vector3 DirectionFrom(this Component self, Component from) =>
            self.transform.position - from.transform.position;

        public static Vector3 DirectionFrom(this GameObject self, GameObject from) =>
            self.transform.position - from.transform.position;

        public static Vector3 DirectionFrom(this GameObject self, Component from) =>
            self.transform.position - from.transform.position;

        public static Vector3 DirectionFrom(this Component self, GameObject from) =>
            self.transform.position - from.transform.position;

        
#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("(to.transform.position - self.transform.position).normalized")]
        [APIDescriptionEN("(to.transform.position - self.transform.position).normalized")]
        [APIExampleCode(@"
gameObj/otherComponent.NormalizedDirectionTo(otherGameObj/otherComponent)
")]
#endif
        public static Vector3 NormalizedDirectionTo(this Component self, Component to) =>
            self.DirectionTo(to).normalized;

        public static Vector3 NormalizedDirectionTo(this GameObject self, GameObject to) =>
            self.DirectionTo(to).normalized;

        public static Vector3 NormalizedDirectionTo(this Component self, GameObject to) =>
            self.DirectionTo(to).normalized;

        public static Vector3 NormalizedDirectionTo(this GameObject self, Component to) =>
            self.DirectionTo(to).normalized;

#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("(self.transform.position - from.transform.position).normalized")]
        [APIDescriptionEN("(self.transform.position - from.transform.position).normalized")]
        [APIExampleCode(@"
gameObj/otherComponent.NormalizedDirectionTo(otherGameObj/otherComponent)
")]
#endif
        public static Vector3 NormalizedDirectionFrom(this Component self, Component from) =>
            self.DirectionFrom(from).normalized;

        public static Vector3 NormalizedDirectionFrom(this GameObject self, GameObject from) =>
            self.DirectionFrom(from).normalized;

        public static Vector3 NormalizedDirectionFrom(this GameObject self, Component from) =>
            self.DirectionFrom(from).normalized;

        public static Vector3 NormalizedDirectionFrom(this Component self, GameObject from) =>
            self.DirectionFrom(from).normalized;

        
#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("(Vector2)(to.transform.position - self.transform.position)")]
        [APIDescriptionEN("(Vector2)(to.transform.position - self.transform.position)")]
        [APIExampleCode(@"
gameObj/otherComponent.Direction2DTo(otherGameObj/otherComponent)
")]
#endif
        public static Vector2 Direction2DTo(this Component self, Component to) =>
            to.transform.position - self.transform.position;

        public static Vector2 Direction2DTo(this GameObject self, GameObject to) =>
            to.transform.position - self.transform.position;

        public static Vector2 Direction2DTo(this Component self, GameObject to) =>
            to.transform.position - self.transform.position;

        public static Vector2 Direction2DTo(this GameObject self, Component to) =>
            to.transform.position - self.transform.position;

#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("(Vector2)(self.transform.position - from.transform.position)")]
        [APIDescriptionEN("(Vector2)(self.transform.position - from.transform.position)")]
        [APIExampleCode(@"
gameObj/otherComponent.Direction2DFrom(otherGameObj/otherComponent)
")]
#endif
        public static Vector2 Direction2DFrom(this Component self, Component from) =>
            self.transform.position - from.transform.position;

        public static Vector2 Direction2DFrom(this GameObject self, GameObject from) =>
            self.transform.position - from.transform.position;

        public static Vector2 Direction2DFrom(this GameObject self, Component from) =>
            self.transform.position - from.transform.position;

        public static Vector2 Direction2DFrom(this Component self, GameObject from) =>
            self.transform.position - from.transform.position;


        
#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("((Vector2)(to.transform.position - self.transform.position)).normalized")]
        [APIDescriptionEN("((Vector2)(to.transform.position - self.transform.position)).normalized")]
        [APIExampleCode(@"
gameObj/otherComponent.NormalizedDirection2DTo(otherGameObj/otherComponent)
")]
#endif
        public static Vector2 NormalizedDirection2DTo(this Component self, Component to) =>
            self.Direction2DTo(to).normalized;

        public static Vector2 NormalizedDirection2DTo(this GameObject self, GameObject to) =>
            self.Direction2DTo(to).normalized;

        public static Vector2 NormalizedDirection2DTo(this Component self, GameObject to) =>
            self.Direction2DTo(to).normalized;

        public static Vector2 NormalizedDirection2DTo(this GameObject self, Component to) =>
            self.Direction2DTo(to).normalized;

        
#if UNITY_EDITOR
        // v1.0.79
        [MethodAPI]
        [APIDescriptionCN("((Vector2)(self.transform.position - from.transform.position)).normalized")]
        [APIDescriptionEN("((Vector2)(self.transform.position - from.transform.position)).normalized")]
        [APIExampleCode(@"
gameObj/otherComponent.NormalizedDirection2DFrom(otherGameObj/otherComponent)
")]
#endif
        public static Vector2 NormalizedDirection2DFrom(this Component self, Component from) =>
            self.Direction2DFrom(from).normalized;

        public static Vector2 NormalizedDirection2DFrom(this GameObject self, GameObject from) =>
            self.Direction2DFrom(from).normalized;

        public static Vector2 NormalizedDirection2DFrom(this GameObject self, Component from) =>
            self.Direction2DFrom(from).normalized;

        public static Vector2 NormalizedDirection2DFrom(this Component self, GameObject from) =>
            self.Direction2DFrom(from).normalized;


        public static Vector2 ToVector2(this Vector3 self) => new Vector2(self.x, self.y);

        public static Vector3 ToVector3(this Vector2 self, float z = 0)
        {
            return new Vector3(self.x, self.y, z);
        }
        
        public static Vector3 X(this Vector3 self,float x)
        {
            self.x = x;
            return self;
        }
        
        public static Vector3 Y(this Vector3 self,float y)
        {
            self.y = y;
            return self;
        }
        
        public static Vector3 Z(this Vector3 self,float z)
        {
            self.z = z;
            return self;
        }
        
        
        public static Vector2 X(this Vector2 self,float x)
        {
            self.x = x;
            return self;
        }
        
        public static Vector2 Y(this Vector2 self,float y)
        {
            self.y = y;
            return self;
        }

        public static float Distance(this GameObject self, GameObject other)
        {
            return Vector3.Distance(self.Position(), other.Position());
        }
        
        public static float Distance(this Component self, GameObject other)
        {
            return Vector3.Distance(self.Position(), other.Position());
        }
        
        public static float Distance(this GameObject self, Component other)
        {
            return Vector3.Distance(self.Position(), other.Position());
        }
        
        public static float Distance(this Component self, Component other)
        {
            return Vector3.Distance(self.Position(), other.Position());
        }
        
        public static float Distance2D(this GameObject self, GameObject other)
        {
            return Vector2.Distance(self.Position2D(), other.Position2D());
        }
        
        public static float Distance2D(this Component self, GameObject other)
        {
            return Vector2.Distance(self.Position2D(), other.Position2D());
        }
        
        public static float Distance2D(this GameObject self, Component other)
        {
            return Vector2.Distance(self.Position2D(), other.Position2D());
        }
        
        public static float Distance2D(this Component self, Component other)
        {
            return Vector2.Distance(self.Position2D(), other.Position2D());
        }
        
        public static float LocalDistance(this GameObject self, GameObject other)
        {
            return Vector3.Distance(self.LocalPosition(), other.LocalPosition());
        }
        
        public static float LocalDistance(this Component self, GameObject other)
        {
            return Vector3.Distance(self.LocalPosition(), other.LocalPosition());
        }
        
        public static float LocalDistance(this GameObject self, Component other)
        {
            return Vector3.Distance(self.LocalPosition(), other.LocalPosition());
        }
        
        public static float LocalDistance(this Component self, Component other)
        {
            return Vector3.Distance(self.LocalPosition(), other.LocalPosition());
        }
        
        public static float LocalDistance2D(this GameObject self, GameObject other)
        {
            return Vector2.Distance(self.LocalPosition2D(), other.LocalPosition2D());
        }
        
        public static float LocalDistance2D(this Component self, GameObject other)
        {
            return Vector2.Distance(self.LocalPosition2D(), other.LocalPosition2D());
        }
        
        public static float LocalDistance2D(this GameObject self, Component other)
        {
            return Vector2.Distance(self.LocalPosition2D(), other.LocalPosition2D());
        }
        
        public static float LocalDistance2D(this Component self, Component other)
        {
            return Vector2.Distance(self.LocalPosition2D(), other.LocalPosition2D());
        }
    }
}