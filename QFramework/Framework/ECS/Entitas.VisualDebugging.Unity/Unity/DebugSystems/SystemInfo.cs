
namespace Entitas.VisualDebugging.Unity
{
    using System;

    [Flags]
    public enum SystemInterfaceFlags
    {
        None = 0,
        IInitializeSystem = 1 << 1,
        IExecuteSystem = 1 << 2,
        ICleanupSystem = 1 << 3,
        ITearDownSystem = 1 << 4,
        IReactiveSystem = 1 << 5
    }

    public class SystemInfo
    {

        public ISystem System { get; protected set; }

        public string SystemName { get; protected set; }

        public bool IsInitializeSystems
        {
            get
            {
                return (mInterfaceFlags & SystemInterfaceFlags.IInitializeSystem) ==
                       SystemInterfaceFlags.IInitializeSystem;
            }
        }

        public bool IsExecuteSystems
        {
            get
            {
                return (mInterfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem;
            }
        }

        public bool IsCleanupSystems
        {
            get
            {
                return (mInterfaceFlags & SystemInterfaceFlags.ICleanupSystem) == SystemInterfaceFlags.ICleanupSystem;
            }
        }

        public bool IsTearDownSystems
        {
            get
            {
                return (mInterfaceFlags & SystemInterfaceFlags.ITearDownSystem) == SystemInterfaceFlags.ITearDownSystem;
            }
        }

        public bool IsReactiveSystems
        {
            get
            {
                return (mInterfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem;
            }
        }

        public double InitializationDuration { get; set; }

        public double AccumulatedExecutionDuration { get; private set; }

        public double MinExecutionDuration { get; private set; }

        public double MaxExecutionDuration { get; private set; }

        public double AverageExecutionDuration
        {
            get { return mExecutionDurationsCount == 0 ? 0 : AccumulatedExecutionDuration / mExecutionDurationsCount; }
        }

        public double AccumulatedCleanupDuration { get; private set; }

        public double MinCleanupDuration { get; private set; }

        public double MaxCleanupDuration { get; private set; }

        public double AverageCleanupDuration
        {
            get { return mCleanupDurationsCount == 0 ? 0 : AccumulatedCleanupDuration / mCleanupDurationsCount; }
        }

        public double AleanupDuration { get; set; }
        public double TeardownDuration { get; set; }

        public bool AreAllParentsActive
        {
            get
            {
                return ParentSystemInfo == null || (ParentSystemInfo.IsActive && ParentSystemInfo.AreAllParentsActive);
            }
        }

        public SystemInfo ParentSystemInfo;
        public bool IsActive;

        readonly SystemInterfaceFlags mInterfaceFlags;

        int mExecutionDurationsCount;
        int mCleanupDurationsCount;

        const string SYSTEM_SUFFIX = "System";

        public SystemInfo(ISystem system)
        {
            System = system;
            mInterfaceFlags = getInterfaceFlags(system);

            var debugSystem = system as DebugSystems;
            if (debugSystem != null)
            {
                SystemName = debugSystem.name;
            }
            else
            {
                var systemType = system.GetType();
                SystemName = systemType.Name.EndsWith(SYSTEM_SUFFIX, StringComparison.Ordinal)
                    ? systemType.Name.Substring(0, systemType.Name.Length - SYSTEM_SUFFIX.Length)
                    : systemType.Name;
            }

            IsActive = true;
        }

        public void AddExecutionDuration(double executionDuration)
        {
            if (executionDuration < MinExecutionDuration || MinExecutionDuration == 0)
            {
                MinExecutionDuration = executionDuration;
            }
            if (executionDuration > MaxExecutionDuration)
            {
                MaxExecutionDuration = executionDuration;
            }

            AccumulatedExecutionDuration += executionDuration;
            mExecutionDurationsCount += 1;
        }

        public void AddCleanupDuration(double cleanupDuration)
        {
            if (cleanupDuration < MinCleanupDuration || MinCleanupDuration == 0)
            {
                MinCleanupDuration = cleanupDuration;
            }
            if (cleanupDuration > MaxCleanupDuration)
            {
                MaxCleanupDuration = cleanupDuration;
            }

            AccumulatedCleanupDuration += cleanupDuration;
            mCleanupDurationsCount += 1;
        }

        public void ResetDurations()
        {
            AccumulatedExecutionDuration = 0;
            mExecutionDurationsCount = 0;

            AccumulatedCleanupDuration = 0;
            mCleanupDurationsCount = 0;
        }

        static SystemInterfaceFlags getInterfaceFlags(ISystem system)
        {
            var flags = SystemInterfaceFlags.None;
            if (system is IInitializeSystem)
            {
                flags |= SystemInterfaceFlags.IInitializeSystem;
            }
            if (system is IReactiveSystem)
            {
                flags |= SystemInterfaceFlags.IReactiveSystem;
            }
            else if (system is IExecuteSystem)
            {
                flags |= SystemInterfaceFlags.IExecuteSystem;
            }
            if (system is ICleanupSystem)
            {
                flags |= SystemInterfaceFlags.ICleanupSystem;
            }
            if (system is ITearDownSystem)
            {
                flags |= SystemInterfaceFlags.ITearDownSystem;
            }

            return flags;
        }
    }
}