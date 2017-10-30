using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace QFramework
{

    public class TimeDebugger
    {
        public static bool SHOW_LOG = true;

        public class TimeItem
        {
            public string   name;
            private long    beginTime;
            private long    endTime;

            public TimeItem(string name)
            {
                this.name = name;
                this.beginTime = DateTime.Now.Ticks;
            }

            public void RecordEndTime()
            {
                this.endTime = DateTime.Now.Ticks;
            }

            public long passTime
            {
                get
                {
                    return (endTime - beginTime) / 10000;
                }
            }

            public long passTicks
            {
                get
                {
                    return (endTime - beginTime);
                }
            }
        }

        private string          m_Name;
        private List<TimeItem>  m_BeginTimeLists;
        private List<TimeItem>  m_EndTimeLists;
        private static TimeDebugger s_TimeDebugger = new TimeDebugger("Total");

        public static TimeDebugger S
        {
            get
            {
                return s_TimeDebugger;
            }
        }

        public TimeDebugger(string name = "")
        {
            m_Name = name;
            m_BeginTimeLists = new List<TimeItem>();
            m_EndTimeLists = new List<TimeItem>();
        }

        public TimeItem Begin(string name)
        {
            TimeItem item = new TimeItem(name);
            m_BeginTimeLists.Add(item);
            return item;
        }

        public void End()
        {
            if (m_BeginTimeLists.Count > 0)
            {
                TimeItem item = m_BeginTimeLists[m_BeginTimeLists.Count - 1];
                m_BeginTimeLists.RemoveAt(m_BeginTimeLists.Count - 1);

                m_EndTimeLists.Add(item);
                item.RecordEndTime();
            }
        }

        public void Reset()
        {
            m_BeginTimeLists.Clear();
            m_EndTimeLists.Clear();
        }

        public void Dump(int min = 0)
        {
            if (!SHOW_LOG)
            {
                return;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Begin Dump Time Debugger :" + m_Name);
            builder.AppendLine();
            for (int i = 0; i < m_EndTimeLists.Count; ++i)
            {
                TimeItem item = m_EndTimeLists[i];
                if (item.passTime > min)
                {
                    builder.AppendLine(string.Format("#      {2}: [PassTime:{0}, PassTicks:{1}]", item.passTime, item.passTicks, item.name));
                }
            }
            builder.AppendLine();
            builder.AppendLine("End Dump Time Debugger :" + m_Name);

            Log.W(builder.ToString());
        }
    }

}