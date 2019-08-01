using System.Collections.Generic;

namespace Zenject
{
    [NoReflectionBaking]
    public class SignalCopyBinder
    {
        readonly List<BindInfo> _bindInfos;

        public SignalCopyBinder()
        {
            _bindInfos = new List<BindInfo>();
        }

        public SignalCopyBinder(BindInfo bindInfo)
        {
            _bindInfos = new List<BindInfo>
            {
                bindInfo
            };
        }

        // This is used in cases where you have multiple bindings that depend on each other so should
        // be inherited together
        public void AddCopyBindInfo(BindInfo bindInfo)
        {
            _bindInfos.Add(bindInfo);
        }

        public void CopyIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyIntoAll);
        }

        // Only copy the binding into children and not grandchildren
        public void CopyIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyDirectOnly);
        }

        // Do not apply the binding on the current container
        public void MoveIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveIntoAll);
        }

        // Do not apply the binding on the current container
        public void MoveIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveDirectOnly);
        }

        void SetInheritanceMethod(BindingInheritanceMethods method)
        {
            for (int i = 0; i < _bindInfos.Count; i++)
            {
                _bindInfos[i].BindingInheritanceMethod = method;
            }
        }
    }
}
