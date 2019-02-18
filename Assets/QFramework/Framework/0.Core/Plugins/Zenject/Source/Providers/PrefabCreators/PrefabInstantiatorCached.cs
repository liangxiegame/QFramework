#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [NoReflectionBaking]
    public class PrefabInstantiatorCached : IPrefabInstantiator
    {
        readonly IPrefabInstantiator _subInstantiator;

        GameObject _gameObject;

        public PrefabInstantiatorCached(IPrefabInstantiator subInstantiator)
        {
            _subInstantiator = subInstantiator;
        }

        public List<TypeValuePair> ExtraArguments
        {
            get { return _subInstantiator.ExtraArguments; }
        }

        public Type ArgumentTarget
        {
            get { return _subInstantiator.ArgumentTarget; }
        }

        public GameObjectCreationParameters GameObjectCreationParameters
        {
            get { return _subInstantiator.GameObjectCreationParameters; }
        }

        public UnityEngine.Object GetPrefab()
        {
            return _subInstantiator.GetPrefab();
        }

        public GameObject Instantiate(InjectContext context, List<TypeValuePair> args, out Action injectAction)
        {
            // We can't really support arguments if we are using the cached value since
            // the arguments might change when called after the first time
            Assert.IsEmpty(args);

            if (_gameObject != null)
            {
                injectAction = null;
                return _gameObject;
            }

            _gameObject = _subInstantiator.Instantiate(context, new List<TypeValuePair>(), out injectAction);
            return _gameObject;
        }
    }
}

#endif
