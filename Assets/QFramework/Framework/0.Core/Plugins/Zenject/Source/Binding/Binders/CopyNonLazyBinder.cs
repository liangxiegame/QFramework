using System.Collections.Generic;

namespace Zenject
{
    [NoReflectionBaking]
    public class CopyNonLazyBinder : NonLazyBinder
    {
        List<BindInfo> _secondaryBindInfos;

        public CopyNonLazyBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        // This is used in cases where you have multiple bindings that depend on each other so should
        // be inherited together (eg. FromIFactory)
        internal void AddSecondaryCopyBindInfo(BindInfo bindInfo)
        {
            if (_secondaryBindInfos == null)
            {
                _secondaryBindInfos = new List<BindInfo>();
            }
            _secondaryBindInfos.Add(bindInfo);
        }

        public NonLazyBinder CopyIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyIntoAll);
            return this;
        }

        // Only copy the binding into children and not grandchildren
        public NonLazyBinder CopyIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.CopyDirectOnly);
            return this;
        }

        // Do not apply the binding on the current container
        public NonLazyBinder MoveIntoAllSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveIntoAll);
            return this;
        }

        // Do not apply the binding on the current container
        public NonLazyBinder MoveIntoDirectSubContainers()
        {
            SetInheritanceMethod(BindingInheritanceMethods.MoveDirectOnly);
            return this;
        }

        void SetInheritanceMethod(BindingInheritanceMethods method)
        {
            BindInfo.BindingInheritanceMethod = method;

            if (_secondaryBindInfos != null)
            {
                foreach (var secondaryBindInfo in _secondaryBindInfos)
                {
                    secondaryBindInfo.BindingInheritanceMethod = method;
                }
            }
        }
    }
}
