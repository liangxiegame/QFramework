/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public class TimerHelper
    {
        protected List<TimeItem>    m_TimeItemList;
        protected bool              m_IsUseAble = true;

        public void Add(TimeItem item)
        {
            if (!m_IsUseAble)
            {
                Log.E("TimeHelper Not Use Able...");
                return;
            }

            if (m_TimeItemList == null)
            {
                m_TimeItemList = new List<TimeItem>(2);
            }
            m_TimeItemList.Add(item);
        }

        public void Clear()
        {
            if (m_TimeItemList != null)
            {
                for (int i = m_TimeItemList.Count - 1; i >= 0; --i)
                {
                    m_TimeItemList[i].Cancel();
                }
                
                m_TimeItemList.Clear();
            }
        }
    }
}
