using System;

namespace QF.GraphDesigner
{
    public class Benchmark : IDisposable
    {
        public Benchmark(Action<TimeSpan> action)
        {
            Action = action;
        }

        public DateTime Start;
        public Action<TimeSpan> Action;
        public Benchmark()
        {
            Start = DateTime.Now;

        }

        public void Dispose()
        {
            var ts = DateTime.Now.Subtract(Start);
            Action(ts);
        }
    }
}