#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject.Internal;

namespace Zenject
{
    public class SceneDecoratorContext : Context
    {
        [SerializeField]
        List<MonoInstaller> _lateInstallers = new List<MonoInstaller>();

        [SerializeField]
        List<MonoInstaller> _lateInstallerPrefabs = new List<MonoInstaller>();

        [SerializeField]
        List<ScriptableObjectInstaller> _lateScriptableObjectInstallers = new List<ScriptableObjectInstaller>();

        public IEnumerable<MonoInstaller> LateInstallers
        {
            get { return _lateInstallers; }
            set
            {
                _lateInstallers.Clear();
                _lateInstallers.AddRange(value);
            }
        }

        public IEnumerable<MonoInstaller> LateInstallerPrefabs
        {
            get { return _lateInstallerPrefabs; }
            set
            {
                _lateInstallerPrefabs.Clear();
                _lateInstallerPrefabs.AddRange(value);
            }
        }

        public IEnumerable<ScriptableObjectInstaller> LateScriptableObjectInstallers
        {
            get { return _lateScriptableObjectInstallers; }
            set
            {
                _lateScriptableObjectInstallers.Clear();
                _lateScriptableObjectInstallers.AddRange(value);
            }
        }

        [FormerlySerializedAs("SceneName")]
        [SerializeField]
        string _decoratedContractName = null;

        DiContainer _container;
        readonly List<MonoBehaviour> _injectableMonoBehaviours = new List<MonoBehaviour>();

        public string DecoratedContractName
        {
            get { return _decoratedContractName; }
        }

        public override DiContainer Container
        {
            get
            {
                Assert.IsNotNull(_container);
                return _container;
            }
        }

        public override IEnumerable<GameObject> GetRootGameObjects()
        {
            // This method should never be called because SceneDecoratorContext's are not bound
            // to the container
            throw Assert.CreateException();
        }

        public void Initialize(DiContainer container)
        {
            Assert.IsNull(_container);
            Assert.That(_injectableMonoBehaviours.IsEmpty());

            _container = container;

            GetInjectableMonoBehaviours(_injectableMonoBehaviours);

            foreach (var instance in _injectableMonoBehaviours)
            {
                container.QueueForInject(instance);
            }
        }

        public void InstallDecoratorSceneBindings()
        {
            _container.Bind<SceneDecoratorContext>().FromInstance(this);
            InstallSceneBindings(_injectableMonoBehaviours);
        }

        public void InstallDecoratorInstallers()
        {
            InstallInstallers();
        }

        protected override void GetInjectableMonoBehaviours(List<MonoBehaviour> monoBehaviours)
        {
            var scene = gameObject.scene;

            ZenUtilInternal.AddStateMachineBehaviourAutoInjectersInScene(scene);
            ZenUtilInternal.GetInjectableMonoBehavioursInScene(scene, monoBehaviours);
        }

        public void InstallLateDecoratorInstallers()
        {
            InstallInstallers(new List<InstallerBase>(), new List<Type>(), _lateScriptableObjectInstallers, _lateInstallers, _lateInstallerPrefabs);
        }
    }
}

#endif
