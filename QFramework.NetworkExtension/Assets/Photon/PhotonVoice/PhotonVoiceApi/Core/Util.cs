using System;
using System.Linq;
using System.Threading;

namespace Photon.Voice
{
    // Does not work until Start() gets called
    internal class SpacingProfile
    {
        short[] buf;
        bool[] info;
        int capacity;
        int ptr = 0;
        System.Diagnostics.Stopwatch watch;
        long watchLast;
        bool flushed;

        public SpacingProfile(int capacity)
        {
            this.capacity = capacity;
        }

        public void Start()
        {
            if (watch == null)
            {
                buf = new short[capacity];
                info = new bool[capacity];
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
        }

        public void Update(bool lost, bool flush)
        {
            if (watch == null)
            {
                return;
            }

            if (flushed)
            {
                watchLast = watch.ElapsedMilliseconds;
            }
            var t = watch.ElapsedMilliseconds;
            buf[ptr] = (short)(t - watchLast);
            info[ptr] = lost;
            watchLast = t;
            ptr++;
            if (ptr == buf.Length)
            {
                ptr = 0;
            }
            flushed = flush;
        }

        public string Dump
        {
            get
            {
                if (watch == null)
                {
                    return "Error: Profiler not started.";
                }
                else
                {
                    var buf2 = buf.Select((v, i) => (info[i] ? "-" : "") + v.ToString()).ToArray();
                    return "max=" + Max + " " + string.Join(",", buf2, ptr, buf.Length - ptr) + ", " + string.Join(",", buf2, 0, ptr);
                }
            }
        }

        // do not call frequently
        public int Max { get { return buf.Select(v => Math.Abs(v)).Max(); } }
    }

    internal static class Util
    {
        static public void SetThreadName(Thread t, string name)
        {
            const int MAX = 25;
            if (name.Length > MAX)
            {
                name = name.Substring(0, MAX);
            }
            t.Name = name;
        }
    }
}