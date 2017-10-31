using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace QFramework.VisualDebugging.Unity
{
    public enum AvgResetInterval
    {
        Always = 1,
        VeryFast = 30,
        Fast = 60,
        Normal = 120,
        Slow = 300,
        Never = int.MaxValue
    }

    public class DebugSystems : Systems
    {
        public static AvgResetInterval avgResetInterval = AvgResetInterval.Never;

        public int totalInitializeSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in mInitializeSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalInitializeSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalExecuteSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in mExecuteSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalExecuteSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalCleanupSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in mCleanupSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalCleanupSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalTearDownSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in mTearDownSystems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalTearDownSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _systems)
                {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalSystemsCount : 1;
                }
                return total;
            }
        }

        public int initializeSystemsCount
        {
            get { return mInitializeSystems.Count; }
        }

        public int executeSystemsCount
        {
            get { return mExecuteSystems.Count; }
        }

        public int cleanupSystemsCount
        {
            get { return mCleanupSystems.Count; }
        }

        public int tearDownSystemsCount
        {
            get { return mTearDownSystems.Count; }
        }

        public string name
        {
            get { return _name; }
        }

        public GameObject gameObject
        {
            get { return _gameObject; }
        }

        public SystemInfo systemInfo
        {
            get { return _systemInfo; }
        }

        public double executeDuration
        {
            get { return mExecuteDuration; }
        }

        public double cleanupDuration
        {
            get { return mCleanupDuration; }
        }

        public SystemInfo[] initializeSystemInfos
        {
            get { return mInitializeSystemInfos.ToArray(); }
        }

        public SystemInfo[] executeSystemInfos
        {
            get { return mExecuteSystemInfos.ToArray(); }
        }

        public SystemInfo[] cleanupSystemInfos
        {
            get { return mCleanupSystemInfos.ToArray(); }
        }

        public SystemInfo[] tearDownSystemInfos
        {
            get { return mTearDownSystemInfos.ToArray(); }
        }

        public bool paused;

        string _name;

        List<ISystem> _systems;
        GameObject _gameObject;
        SystemInfo _systemInfo;

        List<SystemInfo> mInitializeSystemInfos;
        List<SystemInfo> mExecuteSystemInfos;
        List<SystemInfo> mCleanupSystemInfos;
        List<SystemInfo> mTearDownSystemInfos;

        Stopwatch _stopwatch;

        double mExecuteDuration;
        double mCleanupDuration;

        public DebugSystems(string name)
        {
            initialize(name);
        }

        protected DebugSystems(bool noInit)
        {
        }

        protected void initialize(string name)
        {
            _name = name;
            _gameObject = new GameObject(name);
            _gameObject.AddComponent<DebugSystemsBehaviour>().Init(this);

            _systemInfo = new SystemInfo(this);

            _systems = new List<ISystem>();
            mInitializeSystemInfos = new List<SystemInfo>();
            mExecuteSystemInfos = new List<SystemInfo>();
            mCleanupSystemInfos = new List<SystemInfo>();
            mTearDownSystemInfos = new List<SystemInfo>();

            _stopwatch = new Stopwatch();
        }

        public override Systems Add(ISystem system)
        {
            _systems.Add(system);

            SystemInfo childSystemInfo;

            var debugSystems = system as DebugSystems;
            if (debugSystems != null)
            {
                childSystemInfo = debugSystems.systemInfo;
                debugSystems.gameObject.transform.SetParent(_gameObject.transform, false);
            }
            else
            {
                childSystemInfo = new SystemInfo(system);
            }

            childSystemInfo.ParentSystemInfo = _systemInfo;

            if (childSystemInfo.IsInitializeSystems)
            {
                mInitializeSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.IsExecuteSystems || childSystemInfo.IsReactiveSystems)
            {
                mExecuteSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.IsCleanupSystems)
            {
                mCleanupSystemInfos.Add(childSystemInfo);
            }
            if (childSystemInfo.IsTearDownSystems)
            {
                mTearDownSystemInfos.Add(childSystemInfo);
            }

            return base.Add(system);
        }

        public void ResetDurations()
        {
            foreach (var systemInfo in mExecuteSystemInfos)
            {
                systemInfo.ResetDurations();
            }

            foreach (var system in _systems)
            {
                var debugSystems = system as DebugSystems;
                if (debugSystems != null)
                {
                    debugSystems.ResetDurations();
                }
            }
        }

        public override void Initialize()
        {
            for (int i = 0; i < mInitializeSystems.Count; i++)
            {
                var systemInfo = mInitializeSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    mInitializeSystems[i].Initialize();
                    _stopwatch.Stop();
                    systemInfo.InitializationDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }

        public override void Execute()
        {
            if (!paused)
            {
                StepExecute();
            }
        }

        public override void Cleanup()
        {
            if (!paused)
            {
                StepCleanup();
            }
        }

        public void StepExecute()
        {
            mExecuteDuration = 0;
            if (Time.frameCount % (int) avgResetInterval == 0)
            {
                ResetDurations();
            }
            for (int i = 0; i < mExecuteSystems.Count; i++)
            {
                var systemInfo = mExecuteSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    mExecuteSystems[i].Execute();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    mExecuteDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }
        }

        public void StepCleanup()
        {
            mCleanupDuration = 0;
            for (int i = 0; i < mCleanupSystems.Count; i++)
            {
                var systemInfo = mCleanupSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    mCleanupSystems[i].Cleanup();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    mCleanupDuration += duration;
                    systemInfo.AddCleanupDuration(duration);
                }
            }
        }

        public override void TearDown()
        {
            for (int i = 0; i < mTearDownSystems.Count; i++)
            {
                var systemInfo = mTearDownSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    mTearDownSystems[i].TearDown();
                    _stopwatch.Stop();
                    systemInfo.TeardownDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }
    }
}