using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class PoolExceededFixedSizeException : Exception
    {
        public PoolExceededFixedSizeException(string errorMessage)
            : base(errorMessage)
        {
        }
    }

    [Serializable]
    public class MemoryPoolSettings
    {
        public int InitialSize;
        public int MaxSize;
        public PoolExpandMethods ExpandMethod;

        public MemoryPoolSettings()
        {
            InitialSize = 0;
            MaxSize = int.MaxValue;
            ExpandMethod = PoolExpandMethods.OneAtATime;
        }

        public MemoryPoolSettings(int initialSize, int maxSize, PoolExpandMethods expandMethod)
        {
            InitialSize = initialSize;
            MaxSize = maxSize;
            ExpandMethod = expandMethod;
        }

        public static readonly MemoryPoolSettings Default = new MemoryPoolSettings();
    }

    [ZenjectAllowDuringValidation]
    public class MemoryPoolBase<TContract> : IValidatable, IMemoryPool, IDisposable
    {
        Stack<TContract> _inactiveItems;
        IFactory<TContract> _factory;
        MemoryPoolSettings _settings;
        DiContainer _container;

        int _activeCount;

        [Inject]
        void Construct(
            IFactory<TContract> factory,
            DiContainer container,
            [InjectOptional]
            MemoryPoolSettings settings)
        {
            _settings = settings ?? MemoryPoolSettings.Default;
            _factory = factory;
            _container = container;

            _inactiveItems = new Stack<TContract>(_settings.InitialSize);

            if (!container.IsValidating)
            {
                for (int i = 0; i < _settings.InitialSize; i++)
                {
                    _inactiveItems.Push(AllocNew());
                }
            }

#if UNITY_EDITOR
            StaticMemoryPoolRegistry.Add(this);
#endif
        }

        protected DiContainer Container
        {
            get { return _container; }
        }

        public IEnumerable<TContract> InactiveItems
        {
            get { return _inactiveItems; }
        }

        public int NumTotal
        {
            get { return NumInactive + NumActive; }
        }

        public int NumInactive
        {
            get { return _inactiveItems.Count; }
        }

        public int NumActive
        {
            get { return _activeCount; }
        }

        public Type ItemType
        {
            get { return typeof(TContract); }
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            StaticMemoryPoolRegistry.Remove(this);
#endif
        }

        void IMemoryPool.Despawn(object item)
        {
            Despawn((TContract)item);
        }

        public void Despawn(TContract item)
        {
            Assert.That(!_inactiveItems.Contains(item),
                "Tried to return an item to pool {0} twice", GetType());

            _activeCount--;

            _inactiveItems.Push(item);

#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
            using (ProfileBlock.Start("{0}.OnDespawned", GetType()))
#endif
            {
                OnDespawned(item);
            }

            if (_inactiveItems.Count > _settings.MaxSize)
            {
                Resize(_settings.MaxSize);
            }
        }

        TContract AllocNew()
        {
            try
            {
                var item = _factory.Create();

                if (!_container.IsValidating)
                {
                    Assert.IsNotNull(item, "Factory '{0}' returned null value when creating via {1}!", _factory.GetType(), GetType());
                    OnCreated(item);
                }

                return item;
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Error during construction of type '{0}' via {1}.Create method!".Fmt(
                        typeof(TContract), GetType()), e);
            }
        }

        void IValidatable.Validate()
        {
            try
            {
                _factory.Create();
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Validation for factory '{0}' failed".Fmt(GetType()), e);
            }
        }

        public void Clear()
        {
            Resize(0);
        }

        public void ShrinkBy(int numToRemove)
        {
            Resize(_inactiveItems.Count - numToRemove);
        }

        public void ExpandBy(int numToAdd)
        {
            Resize(_inactiveItems.Count + numToAdd);
        }

        protected TContract GetInternal()
        {
            if (_inactiveItems.Count == 0)
            {
                ExpandPool();
                Assert.That(!_inactiveItems.IsEmpty());
            }

            var item = _inactiveItems.Pop();
            _activeCount++;
            OnSpawned(item);
            return item;
        }

        public void Resize(int desiredPoolSize)
        {
            if (_inactiveItems.Count == desiredPoolSize)
            {
                return;
            }

            if (_settings.ExpandMethod == PoolExpandMethods.Disabled)
            {
                throw new PoolExceededFixedSizeException(
                    "Pool factory '{0}' attempted resize but pool set to fixed size of '{1}'!"
                    .Fmt(GetType(), _inactiveItems.Count));
            }

            Assert.That(desiredPoolSize >= 0, "Attempted to resize the pool to a negative amount");

            while (_inactiveItems.Count > desiredPoolSize)
            {
                OnDestroyed(_inactiveItems.Pop());
            }

            while (desiredPoolSize > _inactiveItems.Count)
            {
                _inactiveItems.Push(AllocNew());
            }

            Assert.IsEqual(_inactiveItems.Count, desiredPoolSize);
        }

        void ExpandPool()
        {
            switch (_settings.ExpandMethod)
            {
                case PoolExpandMethods.Disabled:
                {
                    throw new PoolExceededFixedSizeException(
                        "Pool factory '{0}' exceeded its fixed size of '{1}'!"
                        .Fmt(GetType(), _inactiveItems.Count));
                }
                case PoolExpandMethods.OneAtATime:
                {
                    ExpandBy(1);
                    break;
                }
                case PoolExpandMethods.Double:
                {
                    if (NumTotal == 0)
                    {
                        ExpandBy(1);
                    }
                    else
                    {
                        ExpandBy(NumTotal);
                    }
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }

        protected virtual void OnDespawned(TContract item)
        {
            // Optional
        }

        protected virtual void OnSpawned(TContract item)
        {
            // Optional
        }

        protected virtual void OnCreated(TContract item)
        {
            // Optional
        }

        protected virtual void OnDestroyed(TContract item)
        {
            // Optional
        }
    }
}
