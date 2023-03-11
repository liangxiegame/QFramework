/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * https://github.com/akbiggs/UnityTimer
****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    [MonoSingletonPath("[Tools]/Timer")]
    public class Timer : MonoBehaviour,ISingleton
    {
        public static Timer Instance => MonoSingletonProperty<Timer>.Instance;
        
        private readonly BinaryHeap<TimeItem> m_UnScaleTimeHeap = new BinaryHeap<TimeItem>(128, BinaryHeapSortMode.kMin);
        private readonly BinaryHeap<TimeItem> m_ScaleTimeHeap = new BinaryHeap<TimeItem>(128, BinaryHeapSortMode.kMin);
        private float m_CurrentUnScaleTime = -1;
        private float m_CurrentScaleTime = -1;

        public float currentScaleTime
        {
            get { return m_CurrentScaleTime; }
        }

        public float currentUnScaleTime
        {
            get { return m_CurrentUnScaleTime; }
        }

        public void OnSingletonInit()
        {
            m_UnScaleTimeHeap.Clear();
            m_ScaleTimeHeap.Clear();

            m_CurrentUnScaleTime = Time.unscaledTime;
            m_CurrentScaleTime = Time.time;
        }

        public void ResetMgr()
        {
            m_UnScaleTimeHeap.Clear();
            m_ScaleTimeHeap.Clear();
        }

        public void StartMgr()
        {
            m_CurrentUnScaleTime = Time.unscaledTime;
            m_CurrentScaleTime = Time.time;
        }

        #region 投递受缩放影响定时器

        public TimeItem Post2Scale(Action<int> callback, float delay, int repeat)
        {
            TimeItem item = TimeItem.Allocate(callback, delay, repeat);
            Post2Scale(item);
            return item;
        }

        public TimeItem Post2Scale(Action<int> callback, float delay)
        {
            TimeItem item = TimeItem.Allocate(callback, delay);
            Post2Scale(item);
            return item;
        }

        public void Post2Scale(TimeItem item)
        {
            item.SortScore = m_CurrentScaleTime + item.DelayTime();
            m_ScaleTimeHeap.Insert(item);
        }

        #endregion

        #region 投递真实时间定时器

        //投递指定时间计时器：只支持标准时间
        public TimeItem Post2Really(Action<int> callback, DateTime toTime)
        {
            float passTick = (toTime.Ticks - DateTime.Now.Ticks) / 10000000;
            if (passTick < 0)
            {
                Debug.LogWarning("Timer Set Pass Time...");
                passTick = 0;
            }
            return Post2Really(callback, passTick);
        }

        public TimeItem Post2Really(Action<int> callback, float delay, int repeat)
        {
            TimeItem item = TimeItem.Allocate(callback, delay, repeat);
            Post2Really(item);
            return item;
        }

        public TimeItem Post2Really(Action<int> callback, float delay)
        {
            TimeItem item = TimeItem.Allocate(callback, delay);
            Post2Really(item);
            return item;
        }

        public void Post2Really(TimeItem item)
        {
            item.SortScore = m_CurrentUnScaleTime + item.DelayTime();
            m_UnScaleTimeHeap.Insert(item);
        }

        #endregion

        public void Update()
        {
            UpdateMgr();
        }

        public void UpdateMgr()
        {
            TimeItem item = null;
            m_CurrentUnScaleTime = Time.unscaledTime;
            m_CurrentScaleTime = Time.time;

            #region 不受缩放影响定时器更新

            while ((item = m_UnScaleTimeHeap.Top()) != null)
            {
                if (!item.isEnable)
                {
                    m_UnScaleTimeHeap.Pop();
                    item.Recycle2Cache();
                    continue;
                }

                if (item.SortScore < m_CurrentUnScaleTime)
                {
                    m_UnScaleTimeHeap.Pop();

                    item.OnTimeTick();

                    if (item.isEnable && item.NeedRepeat())
                    {
                        Post2Really(item);
                    }
                    else
                    {
                        item.Recycle2Cache();
                    }
                }
                else
                {
                    break;
                }
            }

            #endregion

            #region 受缩放影响定时器更新

            while ((item = m_ScaleTimeHeap.Top()) != null)
            {
                if (!item.isEnable)
                {
                    m_ScaleTimeHeap.Pop();
                    item.Recycle2Cache();
                    continue;
                }

                if (item.SortScore < m_CurrentScaleTime)
                {
                    m_ScaleTimeHeap.Pop();

                    item.OnTimeTick();

                    if (item.isEnable && item.NeedRepeat())
                    {
                        Post2Scale(item);
                    }
                    else
                    {
                        item.Recycle2Cache();
                    }
                }
                else
                {
                    break;
                }
            }

            #endregion
        }

        public void Dump()
        {

        }
    }
}