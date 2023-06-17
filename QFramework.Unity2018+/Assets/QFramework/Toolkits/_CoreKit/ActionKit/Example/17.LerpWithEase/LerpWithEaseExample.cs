using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Example
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

    public class LerpWithEaseExample : MonoBehaviour
    {
        void Start()
        {
            Application.targetFrameRate = 60;
            var spriteTransform = transform.Find("Sprite");

            ActionKit.Sequence()
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(Linear(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutBack(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InBack(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutBack(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutBounce(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutBounce(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InBounce(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutCircle(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InCircle(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutCircle(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InCubic(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutCubic(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutCubic(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutElastic(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InElastic(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutElastic(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutExpo(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InExpo(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutExpo(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutQuad(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InQuad(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutQuad(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutQuart(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InQuart(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutQuart(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutQuint(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InQuint(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutQuint(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InOutSine(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(InSine(0, 5, f)); })
                .Lerp01(3.0f, f => { spriteTransform.LocalPositionX(OutSine(0, 5, f)); })
                .Start(this);
        }

        float Linear(float start, float end, float pos)
        {
            return Mathf.Lerp(start, end, pos);
        }

        float InOutBack(float start, float end, float pos, float bounciness = 1.5f)
        {
            var mid = (start + end) * 0.5f;
            return pos < 0.5f
                ? InBack(start, mid, pos * 2, bounciness)
                : OutBack(mid, end, (pos - 0.5f) * 2, bounciness);
        }

        float InBack(float start, float end, float pos, float bounciness = 1.5f)
        {
            var distance = end - start;
            return distance * pos * pos * ((bounciness + 1) * pos - bounciness) + start;
        }

        float OutBack(float start, float end, float pos, float bounciness = 1.5f)
        {
            var distance = end - start;
            pos -= 1;
            return distance * (pos * pos * ((bounciness + 1) * pos + bounciness) + 1) + start;
        }

        float InOutBounce(float start, float end, float pos, float bounciness = 7.5625f)
        {
            return pos < 0.5f
                ? InBounce(start, (start + end) / 2, pos * 2)
                : OutBounce((start + end) / 2, end, (pos - 0.5f) * 2);
        }

        float OutBounce(float start, float end, float pos, float bounciness = 7.5625f)
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

        float InBounce(float start, float end, float pos, float bounciness = 7.5625f)
        {
            var distance = end - pos;
            pos = 1 - pos;
            return distance - OutBounce(start, end, pos, bounciness) + start;
        }


        float InOutCircle(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InCircle(start, (start + end) / 2, pos * 2)
                : OutCircle((start + end) / 2, end, (pos - 0.5f) * 2);
        }

        float OutCircle(float start, float end, float pos)
        {
            var distance = end - start;
            pos -= 1;
            return distance * Mathf.Sqrt(1 - pos * pos) + start;
        }

        float InCircle(float start, float end, float pos)
        {
            var distance = end - start;
            return -distance * (Mathf.Sqrt(1 - pos * pos) - 1) + start;
        }

        float InOutCubic(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InCubic(start, (start + end) * 0.5f, pos * 2)
                : OutCubic((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        float OutCubic(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (Mathf.Pow(pos - 1, 3) + 1) + start;
        }

        float InCubic(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * Mathf.Pow(pos, 3) + start;
        }

        float InOutElastic(float start, float end, float pos, float elasticity = 0.3f, float duration = 5.0f)
        {
            return pos < 0.5f
                ? InElastic(start, (start + end) * 0.5f, pos * 2, elasticity, duration)
                : OutElastic((start + end) * 0.5f, end, (pos - 0.5f) * 2, elasticity, duration);
        }

        float OutElastic(float start, float end, float pos, float elasticity = 0.3f, float duration = 5.0f)
        {
            var distance = end - start;

            if (pos == 0 || distance == 0) return start;
            if (Math.Abs(pos - 1.0f) < 0.01f) return end;

            var p = duration * elasticity;
            var s = Mathf.Sign(distance) < 0 ? p * 0.25f : p / (2 * Mathf.PI) * Mathf.Asin(1);

            return distance * Mathf.Pow(2, -10 * pos) * Mathf.Sin((pos * duration - s) * (2 * Mathf.PI) / p) +
                   distance + start;
        }

        float InElastic(float start, float end, float pos, float elasticity = 0.3f, float duration = 5.0f)
        {
            var distance = end - start;


            if (pos == 0 || distance == 0) return start;
            if (Math.Abs(pos - 1) < 0.01f) return end;

            var p = duration * elasticity;
            var s = Mathf.Sign(distance) < -1 ? p * 0.25f : p / (2 * Mathf.PI) * Mathf.Asin(1);

            return -(distance * Mathf.Pow(2, 10 * --pos) * Mathf.Sin((pos * duration - s) * (Mathf.PI * 2) / p)) +
                   start;
        }


        float InOutExpo(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InExpo(start, (start + end) * 0.5f, pos * 2)
                : OutExpo((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        float OutExpo(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (-Mathf.Pow(2, -10 * pos) + 1) + start;
        }

        float InExpo(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * Mathf.Pow(2, 10 * (pos - 1)) + start;
        }

        float InOutQuad(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InQuad(start, (start + end) * 0.5f, pos * 2)
                : OutQuad((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        float OutQuad(float start, float end, float pos)
        {
            var distance = end - start;
            return -distance * pos * (pos - 2) + start;
        }

        float InQuad(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * pos * pos + start;
        }

        float InOutQuart(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InQuart(start, (start + end) * 0.5f, pos * 2)
                : OutQuart((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        float OutQuart(float start, float end, float pos)
        {
            var distance = end - start;
            return -distance * ((pos - 1) * (pos - 1) * (pos - 1) * (pos - 1) - 1) + start;
        }

        float InQuart(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (pos * pos * pos * pos) + start;
        }

        float InOutQuint(float start, float end, float pos)
        {
            return pos < 0.5f
                ? InQuint(start, (start + end) * 0.5f, pos * 2)
                : OutQuint((start + end) * 0.5f, end, (pos - 0.5f) * 2);
        }

        float OutQuint(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * ((pos - 1) * (pos - 1) * (pos - 1) * (pos - 1) * (pos - 1) + 1) + start;
        }

        float InQuint(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * pos * pos * pos * pos * pos + start;
        }

        float InOutSine(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * 0.5f * (1 - Mathf.Cos(Mathf.PI * pos)) + start;
        }

        float OutSine(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * Mathf.Sin(pos * 1.570796f) + start;
        }
        float InSine(float start, float end, float pos)
        {
            var distance = end - start;
            return distance * (1 - Mathf.Cos(pos * 1.570796f)) + start;
        }
    }
}


// }

//     ///@func twerp(TwerpType, start, end, pos, [looped], [option1], [option2]);
// function twerp(_type, _start, _end, _pos, _looped = false) {
//   _type = clamp(_type,0,TwerpType.count);
//   _pos = clamp(_looped ? _pos % 1 : _pos,0,1);
//   var _chng = _end-_start;
//   var _mid = (_start+_end) / 2;
