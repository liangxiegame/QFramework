using System;
using UnityEngine;

namespace QFramework
{
public static partial class IActionChainExtention
    {
        public static IActionChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count) {Executer = selfbehaviour};
            retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain {Executer = selfbehaviour};
            retNodeChain.AddTo(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain OnlyBegin(this IActionChain selfChain, Action<OnlyBeginAction> onBegin)
        {
            return selfChain.Append(OnlyBeginAction.Allocate(onBegin));
        }

        public static IActionChain Delay(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        public static void AddTo(this IDisposable self, Component component)
        {
            var onDestroyTrigger = component.gameObject.GetComponent<OnDestroyTrigger>();

            if (!onDestroyTrigger)
            {
                onDestroyTrigger = component.gameObject.AddComponent<OnDestroyTrigger>();
            }

            onDestroyTrigger.AddDispose(self);
        }



        /// <summary>
        /// Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IActionChain Wait(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        public static IActionChain Event(this IActionChain selfChain, params System.Action[] onEvents)
        {
            return selfChain.Append(EventAction.Allocate(onEvents));
        }


        public static IActionChain Until(this IActionChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilAction.Allocate(condition));
        }
    }
}