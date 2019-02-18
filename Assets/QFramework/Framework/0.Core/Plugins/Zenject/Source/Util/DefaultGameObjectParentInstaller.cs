#if !NOT_UNITY3D

using System;
using UnityEngine;

namespace Zenject
{
    public class DefaultGameObjectParentInstaller : Installer<string, DefaultGameObjectParentInstaller>
    {
        readonly string _name;

        public DefaultGameObjectParentInstaller(string name)
        {
            _name = name;
        }

        public override void InstallBindings()
        {
#if !ZEN_TESTS_OUTSIDE_UNITY
            var defaultParent = new GameObject(_name);

            defaultParent.transform.SetParent(
                Container.InheritedDefaultParent, false);

            Container.DefaultParent = defaultParent.transform;

            Container.Bind<IDisposable>()
                .To<DefaultParentObjectDestroyer>().AsCached().WithArguments(defaultParent);

            // Always destroy the default parent last so that the non-monobehaviours get a chance
            // to clean it up if they want to first
            Container.BindDisposableExecutionOrder<DefaultParentObjectDestroyer>(int.MinValue);
#endif
        }

        class DefaultParentObjectDestroyer : IDisposable
        {
            readonly GameObject _gameObject;

            public DefaultParentObjectDestroyer(GameObject gameObject)
            {
                _gameObject = gameObject;
            }

            public void Dispose()
            {
                GameObject.Destroy(_gameObject);
            }
        }
    }
}

#endif
