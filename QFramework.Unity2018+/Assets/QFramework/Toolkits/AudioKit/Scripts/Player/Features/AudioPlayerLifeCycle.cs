/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;

namespace QFramework
{
    internal class AudioPlayerLifeCycle
    {
        internal Action OnStart = null;
        internal Action OnFinish = null;

        internal void RegisterOnStartOnce(Action onStart)
        {
            if (onStart == null) return;
            
            if (OnStart == null)
            {
                OnStart = onStart;
            }
            else
            {
                OnStart += onStart;
            }
        }

        internal void RegisterOnFinishOnce(Action onFinish)
        {
            if (onFinish == null) return;
            
            if (OnFinish == null)
            {
                OnFinish = onFinish;
            }
            else
            {
                OnFinish += onFinish;
            }
        }
        

        internal void Clear()
        {
            OnStart = null;
            OnFinish = null;
        }

        internal void CallOnStartOnce()
        {
            OnStart?.Invoke();
            OnStart = null;
        }

        internal void CallOnFinishOnce()
        {
            OnFinish?.Invoke();
            OnFinish = null;
        }
        
    }
}