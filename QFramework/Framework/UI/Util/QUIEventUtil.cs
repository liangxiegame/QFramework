/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using QFramework;

namespace QFramework
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public static class QUIEventUtil
    {
        private static UnityAction mOnBeforeClickEvent;

        public static void OnBeforeClickEvent(UnityAction onBeforeClickEvent)
        {
            mOnBeforeClickEvent = onBeforeClickEvent;
        }

        public static void OnClick(this Button selfBtn, UnityAction onClick, UnityAction onBeforeClickEventOnce = null,
            UnityAction onPointerUpEventOnce = null)
        {
            selfBtn.RegOnClickEvent(delegate
            {
                if (!ButtonInteractable(selfBtn)) return;
                onClick.InvokeGracefully();
            }, delegate
            {
                if (!ButtonInteractable(selfBtn)) return;
                UIEventLockManager.Instance.SendMsg(new UILockObjEventMsg(selfBtn.gameObject));
                mOnBeforeClickEvent.InvokeGracefully();
                onBeforeClickEventOnce.InvokeGracefully();
            }, delegate
            {
                if (!ButtonInteractable(selfBtn)) return;
                onPointerUpEventOnce.InvokeGracefully();
                UIEventLockManager.Instance.SendMsg(new UIUnlockObjEventMsg(selfBtn.gameObject));
            });
        }

        public static bool ButtonInteractable(Button button)
        {
            return button.IsInteractable() && Interactable(button.gameObject) && button.isActiveAndEnabled;
        }

        public static bool Interactable(GameObject obj)
        {
            if (UIEventLockManager.Instance.LockedObj == null || UIEventLockManager.Instance.LockedObj == obj)
            {
                return true;
            }

            return false;
        }

        public static void OnMouseDown<T>(this T selfComponent, UnityAction onMouseDown) where T : Component
        {
            Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(delegate(long l)
            {
                onMouseDown.InvokeGracefully();
            }).AddTo(selfComponent);
        }
    }
}