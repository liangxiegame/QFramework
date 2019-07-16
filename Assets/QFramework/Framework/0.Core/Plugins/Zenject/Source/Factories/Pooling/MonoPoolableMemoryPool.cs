using UnityEngine;

namespace Zenject
{
    // Zero parameters
    public class MonoPoolableMemoryPool<TValue> : MemoryPool<TValue>
        where TValue : Component, IPoolable
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned();
        }
    }

    // One parameters
    public class MonoPoolableMemoryPool<TParam1, TValue>
        : MemoryPool<TParam1, TValue>
        where TValue : Component, IPoolable<TParam1>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1);
        }
    }

    // Two parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TValue>
        : MemoryPool<TParam1, TParam2, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2);
        }
    }

    // Three parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2, TParam3>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2, p3);
        }
    }

    // Four parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2, TParam3, TParam4>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2, p3, p4);
        }
    }

    // Five parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2, p3, p4, p5);
        }
    }

    // Six parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2, p3, p4, p5, p6);
        }
    }

    // Seven parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2, p3, p4, p5, p6, p7);
        }
    }

    // Eight parameters
    public class MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue>
        : MemoryPool<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TValue>
        where TValue : Component, IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>
    {
        Transform _originalParent;

        [Inject]
        public MonoPoolableMemoryPool()
        {
        }

        protected override void OnCreated(TValue item)
        {
            item.gameObject.SetActive(false);
            _originalParent = item.transform.parent;
        }

        protected override void OnDestroyed(TValue item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnDespawned(TValue item)
        {
            item.OnDespawned();
            item.gameObject.SetActive(false);

            if (item.transform.parent != _originalParent)
            {
                item.transform.SetParent(_originalParent, false);
            }
        }

        protected override void Reinitialize(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8, TValue item)
        {
            item.gameObject.SetActive(true);
            item.OnSpawned(p1, p2, p3, p4, p5, p6, p7, p8);
        }
    }
}
