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
    public interface IRepeat : ISequence
    {
    }

    public class Repeat : IRepeat
    {
        private Sequence mSequence = Sequence.Allocate();

        private int mRepeatCount = -1;
        private int mCurrentRepeatCount = 0;

        private static SimpleObjectPool<Repeat> mSimpleObjectPool =
            new SimpleObjectPool<Repeat>(() => new Repeat(), null, 5);

        private Repeat()
        {
        }

        public static Repeat Allocate(int repeatCount = -1)
        {
            var repeat = mSimpleObjectPool.Allocate();
            repeat.Deinited = false;
            repeat.Reset();
            repeat.mRepeatCount = repeatCount;
            return repeat;
        }

        public bool Paused { get; set; }
        public bool Deinited { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            mCurrentRepeatCount = 0;
        }

        public void OnExecute(float dt)
        {
            if (mRepeatCount == -1 || mRepeatCount == 0)
            {
                if (mSequence.Execute(dt))
                {
                    mSequence.Reset();
                }
            }
            else if (mCurrentRepeatCount < mRepeatCount)
            {
                if (mSequence.Execute(dt))
                {
                    mCurrentRepeatCount++;

                    if (mCurrentRepeatCount >= mRepeatCount)
                    {
                        this.Finish();
                    }
                    else
                    {
                        mSequence.Reset();
                    }
                }
            }
        }

        public void OnFinish()
        {
        }

        public ISequence Append(IAction action)
        {
            mSequence.Append(action);
            return this;
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;

                mSimpleObjectPool.Recycle(this);
            }
        }

        public void Reset()
        {
            mCurrentRepeatCount = 0;
            Status = ActionStatus.NotStart;
            mSequence.Reset();
        }
    }
    
    public static class RepeatExtension
    {
        public static ISequence Repeat(this ISequence self,Action<IRepeat> repeatSetting)
        {
            var repeat = QFramework.Repeat.Allocate();
            repeatSetting(repeat);
            return self.Append(repeat);
        }
        
        public static ISequence Repeat(this ISequence self,int repeatCount, Action<IRepeat> repeatSetting)
        {
            var repeat = QFramework.Repeat.Allocate(repeatCount);
            repeatSetting(repeat);
            return self.Append(repeat);
        }
    }
}