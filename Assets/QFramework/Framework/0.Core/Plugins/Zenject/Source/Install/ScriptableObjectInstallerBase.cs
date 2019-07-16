#if !NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    // We'd prefer to make this abstract but Unity 5.3.5 has a bug where references
    // can get lost during compile errors for classes that are abstract
    public class ScriptableObjectInstallerBase : ScriptableObject, IInstaller
    {
        [Inject]
        DiContainer _container = null;

        protected DiContainer Container
        {
            get { return _container; }
        }

        bool IInstaller.IsEnabled
        {
            get { return true; }
        }

        public virtual void InstallBindings()
        {
            throw new NotImplementedException();
        }
    }
}

#endif

