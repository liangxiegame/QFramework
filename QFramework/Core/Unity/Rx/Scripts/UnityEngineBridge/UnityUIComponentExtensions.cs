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

// for uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

namespace QFramework
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    
    public static partial class UnityUIComponentExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, Text text, Func<T, string> selector)
        {
            return source.SubscribeWithState2(text, selector, (x, t, s) => t.text = s(x));
        }

        public static IDisposable SubscribeToInteractable(this IObservable<bool> source, Selectable selectable)
        {
            return source.SubscribeWithState(selectable, (x, s) => s.interactable = x);
        }

        /// <summary>Observe onClick event.</summary>
        public static IObservable<Unit> OnClickAsObservable(this Button button)
        {
            return button.onClick.AsObservable();
        }

        /// <summary>Observe onValueChanged with current `isOn` value on subscribe.</summary>
        public static IObservable<bool> OnValueChangedAsObservable(this Toggle toggle)
        {
            // Optimized Defer + StartWith
            return Observable.CreateWithState<bool, Toggle>(toggle, (t, observer) =>
            {
                observer.OnNext(t.isOn);
                return t.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Scrollbar scrollbar)
        {
            return Observable.CreateWithState<float, Scrollbar>(scrollbar, (s, observer) =>
            {
                observer.OnNext(s.value);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `normalizedPosition` value on subscribe.</summary>
        public static IObservable<Vector2> OnValueChangedAsObservable(this ScrollRect scrollRect)
        {
            return Observable.CreateWithState<Vector2, ScrollRect>(scrollRect, (s, observer) =>
            {
                observer.OnNext(s.normalizedPosition);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onValueChanged with current `value` on subscribe.</summary>
        public static IObservable<float> OnValueChangedAsObservable(this Slider slider)
        {
            return Observable.CreateWithState<float, Slider>(slider, (s, observer) =>
            {
                observer.OnNext(s.value);
                return s.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        /// <summary>Observe onEndEdit(Submit) event.</summary>
        public static IObservable<string> OnEndEditAsObservable(this InputField inputField)
        {
            return inputField.onEndEdit.AsObservable();
        }

        /// <summary>Observe onValueChange with current `text` value on subscribe.</summary>
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        [Obsolete("onValueChange has been renamed to onValueChanged")]
#endif
        public static IObservable<string> OnValueChangeAsObservable(this InputField inputField)
        {
            return Observable.CreateWithState<string, InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
#if (UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                return i.onValueChange.AsObservable().Subscribe(observer);
#else
                return i.onValueChanged.AsObservable().Subscribe(observer);
#endif
            });
        }

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        /// <summary>Observe onValueChanged with current `text` value on subscribe.</summary>
        public static IObservable<string> OnValueChangedAsObservable(this InputField inputField)
        {
            return Observable.CreateWithState<string, InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
#endif
    }
}

#endif