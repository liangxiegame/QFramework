using System;

namespace QFramework.Example
{
    public abstract class TimeTickTask
    {
        public const int TypeA = 0;
        public const int TypeB = 1;
        public const int TypeC = 2;

        // 等待中
        public const int StateWaiting = 0;
        
        // 正在倒计时
        public const int StateTicking = 1;

        // 倒计时完成
        public const int StateFinished = 2;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State = StateTicking;

        /// <summary>
        /// 剩余时间
        /// </summary>
        public int RemainSeconds
        {
            get
            {
                var remainTime = EndTime - DateTime.Now;

                if (remainTime.TotalSeconds < 0)
                {
                    return 0;
                }

                return (int) remainTime.TotalSeconds;
            }
        }

        public int Type { get; set; }

        public abstract void OnStart();
        public abstract void OnFinish();

    }

}