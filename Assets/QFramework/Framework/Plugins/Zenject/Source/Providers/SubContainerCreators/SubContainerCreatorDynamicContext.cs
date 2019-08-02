#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using UnityEngine;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public abstract class SubContainerCreatorDynamicContext : ISubContainerCreator
    {
        readonly DiContainer _container;

        public SubContainerCreatorDynamicContext(DiContainer container)
        {
            _container = container;
        }

        protected DiContainer Container
        {
            get { return _container; }
        }

        public DiContainer CreateSubContainer(
            List<TypeValuePair> args, InjectContext parentContext)
        {
            bool shouldMakeActive;
            var gameObj = CreateGameObject(out shouldMakeActive);

            var context = gameObj.AddComponent<GameObjectContext>();

            AddInstallers(args, context);

            _container.Inject(context);

            if (shouldMakeActive && !_container.IsValidating)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
                {
                    gameObj.SetActive(true);
                }
            }

            // Note: We don't need to call ResolveRoots here because GameObjectContext does this for us

            return context.Container;
        }

        protected abstract void AddInstallers(List<TypeValuePair> args, GameObjectContext context);
        protected abstract GameObject CreateGameObject(out bool shouldMakeActive);
    }
}

#endif
