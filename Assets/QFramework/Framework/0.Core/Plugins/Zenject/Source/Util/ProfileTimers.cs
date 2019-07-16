#if ZEN_INTERNAL_PROFILING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ModestTree;

namespace Zenject
{
    // Similar to ProfileBlock except used for measuring speed of zenject specifically
    // And does not use unity's profiler
    public static class ProfileTimers
    {
        static Dictionary<string, TimerInfo> _timers = new Dictionary<string, TimerInfo>();

        public static void ResetAll()
        {
            foreach (var timer in _timers.Values)
            {
                timer.Reset();
            }
        }

        public static string FormatResults()
        {
            var result = new StringBuilder();

            // Uncomment if you only want to see zenject related info
            //var timers = _timers.Where(x => x.Key != "User Code");
            var timers = _timers;

            var total = timers.Select(x => x.Value.TotalMilliseconds).Sum();

            result.Append("Total time tracked: {0:0.00} ms.  Details:".Fmt(total));

            foreach (var pair in timers.OrderByDescending(x => x.Value.TotalMilliseconds))
            {
                var time = pair.Value.TotalMilliseconds;
                var percent = 100.0 * (time / total);
                var name = pair.Key;

                result.Append("\n  {0:00.0}% ({1:00000}x) ({2:0000} ms) {3}".Fmt(percent, pair.Value.CallCount, time, name));
            }

            return result.ToString();
        }

        public static double GetTimerElapsedMilliseconds(string name)
        {
            return _timers[name].TotalMilliseconds;
        }

        public static IDisposable CreateTimedBlock(string name)
        {
            TimerInfo timer;

            if (!_timers.TryGetValue(name, out timer))
            {
                timer = new TimerInfo();
                _timers.Add(name, timer);
            }

            timer.CallCount++;

            if (timer.IsRunning)
            {
                return null;
            }

            return TimedBlock.Pool.Spawn(timer);
        }

        class TimedBlock : IDisposable
        {
            public static StaticMemoryPool<TimerInfo, TimedBlock> Pool =
                new StaticMemoryPool<TimerInfo, TimedBlock>(OnSpawned, OnDespawned);

            readonly List<TimerInfo> _pausedTimers = new List<TimerInfo>();

            TimerInfo _exclusiveTimer;

            static void OnSpawned(
                TimerInfo exclusiveTimer, TimedBlock instance)
            {
                Assert.That(instance._pausedTimers.Count == 0);

                instance._exclusiveTimer = exclusiveTimer;

                foreach (var timer in _timers.Values)
                {
                    if (exclusiveTimer == timer)
                    {
                        Assert.That(!timer.IsRunning);
                        timer.Resume();
                    }
                    else if (timer.IsRunning)
                    {
                        timer.Pause();
                        instance._pausedTimers.Add(timer);
                    }
                }
            }

            static void OnDespawned(TimedBlock instance)
            {
                Assert.That(instance._exclusiveTimer.IsRunning);
                instance._exclusiveTimer.Pause();

                foreach (var timer in instance._pausedTimers)
                {
                    Assert.That(!timer.IsRunning);
                    timer.Resume();
                }

                instance._pausedTimers.Clear();
            }

            public void Dispose()
            {
                Pool.Despawn(this);
            }
        }

        public class TimerInfo
        {
            readonly Stopwatch _timer;

            public TimerInfo()
            {
                _timer = new Stopwatch();
            }

            public int CallCount
            {
                get; set;
            }

            public double TotalMilliseconds
            {
                get { return _timer.Elapsed.TotalMilliseconds; }
            }

            public bool IsRunning
            {
                get { return _timer.IsRunning; }
            }

            public void Reset()
            {
                _timer.Reset();
            }

            public void Resume()
            {
                _timer.Start();
            }

            public void Pause()
            {
                _timer.Stop();
            }
        }
    }
}

#endif
