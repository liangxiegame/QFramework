using System;
using System.Collections;
using Zenject.Internal;
using ModestTree;
using Assert = ModestTree.Assert;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace Zenject
{
    public abstract class ZenjectIntegrationTestFixture
    {
        SceneContext _sceneContext;

        bool _hasEndedInstall;
        bool _hasStartedInstall;

        protected DiContainer Container
        {
            get
            {
                Assert.That(_hasStartedInstall,
                    "Must call PreInstall() before accessing ZenjectIntegrationTestFixture.Container!");
                return _sceneContext.Container;
            }
        }

        protected SceneContext SceneContext
        {
            get
            {
                Assert.That(_hasStartedInstall,
                    "Must call PreInstall() before accessing ZenjectIntegrationTestFixture.SceneContext!");
                return _sceneContext;
            }
        }

        [SetUp]
        public void Setup()
        {
            Assert.That(Application.isPlaying,
                "ZenjectIntegrationTestFixture is meant to be used for play mode tests only.  Please ensure your test file '{0}' is outside of the editor folder and try again.", GetType());

            ZenjectTestUtil.DestroyEverythingExceptTestRunner(true);
            StaticContext.Clear();
        }

        protected void SkipInstall()
        {
            PreInstall();
            PostInstall();
        }

        protected void PreInstall()
        {
            Assert.That(!_hasStartedInstall, "Called PreInstall twice in test '{0}'!", TestContext.CurrentContext.Test.Name);
            _hasStartedInstall = true;

            Assert.That(!ProjectContext.HasInstance);

            var shouldValidate = CurrentTestHasAttribute<ValidateOnlyAttribute>();
            ProjectContext.ValidateOnNextRun = shouldValidate;

            Assert.That(_sceneContext == null);

            _sceneContext = SceneContext.Create();
            _sceneContext.Install();

            Assert.That(ProjectContext.HasInstance);

            Assert.IsEqual(shouldValidate, ProjectContext.Instance.Container.IsValidating);
            Assert.IsEqual(shouldValidate, _sceneContext.Container.IsValidating);
        }

        bool CurrentTestHasAttribute<T>()
            where T : Attribute
        {
            return GetType().GetMethod(TestContext.CurrentContext.Test.MethodName)
                .GetCustomAttributes(true)
                .Cast<Attribute>().OfType<T>().Any();
        }

        protected void PostInstall()
        {
            Assert.That(_hasStartedInstall,
                "Called PostInstall but did not call PreInstall in test '{0}'!", TestContext.CurrentContext.Test.Name);

            Assert.That(!_hasEndedInstall, "Called PostInstall twice in test '{0}'!", TestContext.CurrentContext.Test.Name);

            _hasEndedInstall = true;
            _sceneContext.Resolve();

            Container.Inject(this);

            if (!Container.IsValidating)
            {
                // We don't have to do this here but it's kind of convenient
                // We could also remove it and just require that users add a yield after calling
                // and it would have the same effect
                Container.Resolve<MonoKernel>().Initialize();
            }
        }

        protected IEnumerator DestroyEverything()
        {
            Assert.That(_hasStartedInstall,
                "Called DestroyAll but did not call PreInstall (or SkipInstall) in test '{0}'!", TestContext.CurrentContext.Test.Name);
            DestroyEverythingInternal(false);
            // Wait one frame for GC to really destroy everything
            yield return null;
        }

        void DestroyEverythingInternal(bool immediate)
        {
            if (_sceneContext != null)
            {
                // We need to use DestroyImmediate so that all the IDisposable's etc get processed immediately before
                // next test runs
                if (immediate)
                {
                    GameObject.DestroyImmediate(_sceneContext.gameObject);
                }
                else
                {
                    GameObject.Destroy(_sceneContext.gameObject);
                }

                _sceneContext = null;
            }

            ZenjectTestUtil.DestroyEverythingExceptTestRunner(immediate);
            StaticContext.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome == ResultState.Success)
            {
                Assert.That(_hasStartedInstall,
                    "PreInstall (or SkipInstall) was not called in test '{0}'!", TestContext.CurrentContext.Test.Name);

                Assert.That(_hasEndedInstall,
                    "PostInstall was not called in test '{0}'!", TestContext.CurrentContext.Test.Name);
            }

            DestroyEverythingInternal(true);

            _hasStartedInstall = false;
            _hasEndedInstall = false;
        }
    }
}
