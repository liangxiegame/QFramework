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

namespace QFramework
{
    using UnityEngine;
    
    public abstract class ObservableTriggerBase : MonoBehaviour
    {
        bool calledAwake = false;
        Subject<Unit> awake;

        /// <summary>Awake is called when the script instance is being loaded.</summary>
        void Awake()
        {
            calledAwake = true;
            if (awake != null) { awake.OnNext(Unit.Default); awake.OnCompleted(); }
        }

        /// <summary>Awake is called when the script instance is being loaded.</summary>
        public IObservable<Unit> AwakeAsObservable()
        {
            if (calledAwake) return Observable.Return(Unit.Default);
            return awake ?? (awake = new Subject<Unit>());
        }

        bool calledStart = false;
        Subject<Unit> start;

        /// <summary>Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.</summary>
        void Start()
        {
            calledStart = true;
            if (start != null) { start.OnNext(Unit.Default); start.OnCompleted(); }
        }

        /// <summary>Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.</summary>
        public IObservable<Unit> StartAsObservable()
        {
            if (calledStart) return Observable.Return(Unit.Default);
            return start ?? (start = new Subject<Unit>());
        }


        bool calledDestroy = false;
        Subject<Unit> onDestroy;

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        void OnDestroy()
        {
            calledDestroy = true;
            if (onDestroy != null) { onDestroy.OnNext(Unit.Default); onDestroy.OnCompleted(); }

            RaiseOnCompletedOnDestroy();
        }

        /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
        public IObservable<Unit> OnDestroyAsObservable()
        {
            if (this == null) return Observable.Return(Unit.Default);
            if (calledDestroy) return Observable.Return(Unit.Default);
            return onDestroy ?? (onDestroy = new Subject<Unit>());
        }

        protected abstract void RaiseOnCompletedOnDestroy();
    }
}