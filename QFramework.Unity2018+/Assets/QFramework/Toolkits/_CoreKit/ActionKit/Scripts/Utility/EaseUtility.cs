/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public enum EaseType
    {
        Linear,
        InOutBack,
        InBack,
        OutBack,
        InOutBounce,
        OutBounce,
        InBounce,
        InOutCircle,
        OutCircle,
        InCircle,
        InOutCubic,
        OutCubic,
        InCubic,
        InOutElastic,
        OutElastic,
        InElastic,
        InOutExpo,
        OutExpo,
        InExpo,
        InOutQuad,
        OutQuad,
        InQuad,
        InoutQuart,
        OutQuart,
        InQuart,
        InOutQuint,
        OutQuint,
        InQuint,
        InOutSine,
        OutSine,
        InSine,
        Count
    }

    
    public class EaseUtility
    {
        public static float Linear(float start, float end, float pos)
        {
            return Mathf.Lerp(start, end, pos);
        }

        public static float InOutBack(float start, float end, float pos, float bounciness = 1.5f)
        {
            var mid = (start + end) * 0.5f;
            return pos < 0.5f
                ? InBack(start, mid, pos * 2, bounciness)
                : OutBack(mid, end, (pos - 0.5f) * 2, bounciness);
        }

        public static float InBack(float start, float end, float pos, float bounciness = 1.5f)
        {
            var distance = end - start;
            return distance * pos * pos * ((bounciness + 1) * pos - bounciness) + start;
        }

        public static float OutBack(float start, float end, float pos, float bounciness = 1.5f)
        {
            var distance = end - start;
            pos -= 1;
            return distance * (pos * pos * ((bounciness + 1) * pos + bounciness) + 1) + start;
        }

        public static float InOutBounce(float start, float end, float pos, float bounciness = 7.5625f)
        {
            return pos < 0.5f
                ? InBounce(start, (start + end) / 2, pos * 2)
                : OutBounce((start + end) / 2, end, (pos - 0.5f) * 2);
        }

        public static float OutBounce(float start, float end, float pos, float bounciness = 7.5625f)
        {
            var distance = end - start;
            if (pos < 1f / 2.75f)
                return distance * (bounciness * pos * pos) + start;
            if (pos < 2f / 2.75f)
            {
                pos -= 1.5f / 2.75f;
                return distance * (bounciness * pos * pos + 3f / 4f) + start;
            }

            if (pos < 2.5f / 2.75f)
            {
                pos -= 2.25f / 2.75f;
                return distance * (bounciness * pos * pos + 15f / 16f) + start;
            }

            pos -= 2.625f / 2.75f;
            return distance * (bounciness * pos * pos + 63f / 64f) + start;
        }

        public static float InBounce(float start, float end, float pos, float bounciness = 7.5625f)
        {
            var distance = end - pos;
            pos = 1 - pos;
            return distance - OutBounce(start, end, pos, bounciness) + start;
        }


        public static float InOutCircle(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InCircle(start, (start + end) / 2, pos * 2)
                : OutCircle((start + end) / 2, end, (pos - 0.5f) * 2);
        }

        public static float OutCircle(float start, float end, float pos)
        {
            var distance = end - start;
            pos -= 1;
            return distance * Mathf.Sqrt(1 - pos * pos) + start;
        }

        public static float InCircle(float start, float end, float pos)
        {
            var distance = end - start;
            return -distance * (Mathf.Sqrt(1 - pos * pos) - 1) + start;
        }

        public static float InOutCubic(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InCubic(start, (start + end) * 0.5f, pos * 2)
                : OutCubic((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        public static float OutCubic(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (Mathf.Pow(pos - 1, 3) + 1) + start;
        }

        public static float InCubic(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * Mathf.Pow(pos, 3) + start;
        }

        public static float InOutElastic(float start, float end, float pos, float elasticity = 0.3f, float duration = 5.0f)
        {
            return pos < 0.5f
                ? InElastic(start, (start + end) * 0.5f, pos * 2, elasticity, duration)
                : OutElastic((start + end) * 0.5f, end, (pos - 0.5f) * 2, elasticity, duration);
        }

        public static float OutElastic(float start, float end, float pos, float elasticity = 0.3f, float duration = 5.0f)
        {
            var distance = end - start;

            if (pos == 0 || distance == 0) return start;
            if (Math.Abs(pos - 1.0f) < 0.01f) return end;

            var p = duration * elasticity;
            var s = Mathf.Sign(distance) < 0 ? p * 0.25f : p / (2 * Mathf.PI) * Mathf.Asin(1);

            return distance * Mathf.Pow(2, -10 * pos) * Mathf.Sin((pos * duration - s) * (2 * Mathf.PI) / p) +
                   distance + start;
        }

        public static float InElastic(float start, float end, float pos, float elasticity = 0.3f, float duration = 5.0f)
        {
            var distance = end - start;


            if (pos == 0 || distance == 0) return start;
            if (Math.Abs(pos - 1) < 0.01f) return end;

            var p = duration * elasticity;
            var s = Mathf.Sign(distance) < -1 ? p * 0.25f : p / (2 * Mathf.PI) * Mathf.Asin(1);

            return -(distance * Mathf.Pow(2, 10 * --pos) * Mathf.Sin((pos * duration - s) * (Mathf.PI * 2) / p)) +
                   start;
        }


        public static float InOutExpo(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InExpo(start, (start + end) * 0.5f, pos * 2)
                : OutExpo((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        public static float OutExpo(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (-Mathf.Pow(2, -10 * pos) + 1) + start;
        }

        public static float InExpo(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * Mathf.Pow(2, 10 * (pos - 1)) + start;
        }

        public static float InOutQuad(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InQuad(start, (start + end) * 0.5f, pos * 2)
                : OutQuad((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        public static float OutQuad(float start, float end, float pos)
        {
            var distance = end - start;
            return -distance * pos * (pos - 2) + start;
        }

        public static float InQuad(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * pos * pos + start;
        }

        public static float InOutQuart(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InQuart(start, (start + end) * 0.5f, pos * 2)
                : OutQuart((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        public static float OutQuart(float start, float end, float pos)
        {
            var distance = end - start;
            return -distance * ((pos - 1) * (pos - 1) * (pos - 1) * (pos - 1) - 1) + start;
        }

        public static float InQuart(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (pos * pos * pos * pos) + start;
        }

        public static float InOutQuint(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InQuint(start, (start + end) * 0.5f, pos * 2)
                : OutQuint((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        public static float OutQuint(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * ((pos - 1) * (pos - 1) * (pos - 1) * (pos - 1) * (pos - 1) + 1) + start;
        }

        public static float InQuint(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * pos * pos * pos * pos * pos + start;
        }

        public static float InOutSine(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * 0.5f * (1 - Mathf.Cos(Mathf.PI * pos)) + start;
        }

        public static float OutSine(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * Mathf.Sin(pos * 1.570796f) + start;
        }
        public static float InSine(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (1 - Mathf.Cos(pos * 1.570796f)) + start;
        }
    }
}