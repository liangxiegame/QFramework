using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 递归遍历查找相应条件的子物体
        /// </summary>
        /// <param name="tfParent">当前Transform</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static Transform FindChildRecursionExt(this Transform tfParent, Func<Transform, bool> predicate)
        {
            if (predicate(tfParent))
            {
                Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursionExt(predicate);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }
        
        /// <summary>
        /// 递归遍历查找指定的名字的子物体
        /// </summary>
        /// <param name="tfParent">当前Transform</param>
        /// <param name="name">目标名</param>
        /// <param name="stringComparison">字符串比较规则</param>
        /// <returns></returns>
        public static Transform FindChildRecursionExt(this Transform tfParent, string name,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (tfParent.name.Equals(name, stringComparison))
            {
                //Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursionExt(name, stringComparison);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        public static string GetPathExt(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }
    }
}