/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public interface ICustomAPI<TData>
    {
        TData Data { get; set; }

        ICustomAPI<TData> OnStart(Action onStart);
        ICustomAPI<TData> OnExecute(Action<float> onExecute);
        ICustomAPI<TData> OnFinish(Action onFinish);

        void Finish();
    }

    internal class Custom<TData> : IAction, ICustomAPI<TData>
    {
        public TData Data { get; set; }

        protected Action mOnStart;
        protected Action<float> mOnExecute;
        protected Action mOnFinish;

        private static SimpleObjectPool<Custom<TData>> mSimpleObjectPool =
            new SimpleObjectPool<Custom<TData>>(() => new Custom<TData>(), null, 10);

        protected Custom()
        {
        }

        public static Custom<TData> Allocate()
        {
            var custom = mSimpleObjectPool.Allocate();
            custom.ActionID = ActionKit.ID_GENERATOR++;
            custom.Deinited = false;
            custom.Reset();
            return custom;
        }

        public bool Paused { get; set; }

        public virtual void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mOnStart = null;
                mOnExecute = null;
                mOnFinish = null;

                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Custom<TData>>(mSimpleObjectPool,this));
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }

        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            mOnStart?.Invoke();
        }

        public void OnExecute(float dt)
        {
            mOnExecute?.Invoke(dt);
        }

        public void OnFinish()
        {
            mOnFinish?.Invoke();
        }

        public ICustomAPI<TData> OnStart(Action onStart)
        {
            mOnStart = onStart;
            return this;
        }

        public ICustomAPI<TData> OnExecute(Action<float> onExecute)
        {
            mOnExecute = onExecute;
            return this;
        }

        public ICustomAPI<TData> OnFinish(Action onFinish)
        {
            mOnFinish = onFinish;
            return this;
        }

        public void Finish()
        {
            Status = ActionStatus.Finished;
        }
    }

    internal class Custom : Custom<object>
    {
        private static SimpleObjectPool<Custom> mSimpleObjectPool =
            new SimpleObjectPool<Custom>(() => new Custom(), null, 10);

        protected Custom()
        {
        }

        public new static Custom Allocate()
        {
            var custom = mSimpleObjectPool.Allocate();
            custom.Deinited = false;
            custom.Reset();
            return custom;
        }

        public override void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;

                mOnStart = null;
                mOnExecute = null;
                mOnFinish = null;

                mSimpleObjectPool.Recycle(this);
            }
        }
    }

    public static class CustomExtension
    {
        public static ISequence Custom(this ISequence self, Action<ICustomAPI<object>> onCustomSetting)
        {
            var custom = ActionKit.Custom(onCustomSetting);
            return self.Append(custom);
        }


        public static ISequence Custom<TData>(this ISequence self, Action<ICustomAPI<TData>> onCustomSetting)
        {
            var custom = ActionKit.Custom(onCustomSetting);
            return self.Append(custom);
        }
    }
}