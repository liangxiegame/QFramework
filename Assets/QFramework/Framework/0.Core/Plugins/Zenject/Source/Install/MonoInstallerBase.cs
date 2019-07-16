#if !NOT_UNITY3D

using System;
using System.Diagnostics;
using UnityEngine;

namespace Zenject
{
    // We'd prefer to make this abstract but Unity 5.3.5 has a bug where references
    // can get lost during compile errors for classes that are abstract
    [DebuggerStepThrough]
    public class MonoInstallerBase : MonoBehaviour, IInstaller
    {
        [Inject]
        protected DiContainer Container
        {
            get; set;
        }

        public virtual bool IsEnabled
        {
            get { return enabled; }
        }

        public virtual void Start()
        {
            // Define this method so we expose the enabled check box
        }

        public virtual void InstallBindings()
        {
            throw new NotImplementedException();
        }
    }
}

#endif

