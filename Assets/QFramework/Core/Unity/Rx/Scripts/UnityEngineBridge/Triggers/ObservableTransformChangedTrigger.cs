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

// after uGUI(from 4.6)
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

namespace QFramework
{
    using UnityEngine;
    
    [DisallowMultipleComponent]
    public class ObservableTransformChangedTrigger : ObservableTriggerBase
    {
        Subject<Unit> onBeforeTransformParentChanged;

        // Callback sent to the graphic before a Transform parent change occurs
        void OnBeforeTransformParentChanged()
        {
            if (onBeforeTransformParentChanged != null) onBeforeTransformParentChanged.OnNext(Unit.Default);
        }

        /// <summary>Callback sent to the graphic before a Transform parent change occurs.</summary>
        public IObservable<Unit> OnBeforeTransformParentChangedAsObservable()
        {
            return onBeforeTransformParentChanged ?? (onBeforeTransformParentChanged = new Subject<Unit>());
        }

        Subject<Unit> onTransformParentChanged;

        // This function is called when the parent property of the transform of the GameObject has changed
        void OnTransformParentChanged()
        {
            if (onTransformParentChanged != null) onTransformParentChanged.OnNext(Unit.Default);
        }

        /// <summary>This function is called when the parent property of the transform of the GameObject has changed.</summary>
        public IObservable<Unit> OnTransformParentChangedAsObservable()
        {
            return onTransformParentChanged ?? (onTransformParentChanged = new Subject<Unit>());
        }

        Subject<Unit> onTransformChildrenChanged;

        // This function is called when the list of children of the transform of the GameObject has changed
        void OnTransformChildrenChanged()
        {
            if (onTransformChildrenChanged != null) onTransformChildrenChanged.OnNext(Unit.Default);
        }

        /// <summary>This function is called when the list of children of the transform of the GameObject has changed.</summary>
        public IObservable<Unit> OnTransformChildrenChangedAsObservable()
        {
            return onTransformChildrenChanged ?? (onTransformChildrenChanged = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onBeforeTransformParentChanged != null)
            {
                onBeforeTransformParentChanged.OnCompleted();
            }
            if (onTransformParentChanged != null)
            {
                onTransformParentChanged.OnCompleted();
            }
            if (onTransformChildrenChanged != null)
            {
                onTransformChildrenChanged.OnCompleted();
            }
        }
    }
}

#endif