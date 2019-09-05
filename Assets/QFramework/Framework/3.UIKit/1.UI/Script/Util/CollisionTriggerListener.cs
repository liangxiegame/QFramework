/****************************************************************************
 * Copyright (c) 2017 imagicbell
 * Copyright (c) 2018 liangxie
****************************************************************************/

namespace QFramework
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class CollisionEvent<T> : UnityEvent<T> {}

    /// <inheritdoc />
    /// <summary>
    /// 监听碰撞事件
    /// </summary>
    public class CollisionTriggerListener : MonoBehaviour
    {
        public static CollisionTriggerListener Get(GameObject go)
        {
            var listener = go.GetComponent<CollisionTriggerListener>();
            if (listener == null) listener = go.AddComponent<CollisionTriggerListener>();
            return listener;
        }

        private Dictionary<EventName, UnityEventBase> mEvents = new Dictionary<EventName, UnityEventBase>();

        private void OnDestroy()
        {
            RemoveAll();
        }

        public void RemoveAll()
        {
            foreach (var e in mEvents.Values)
            {
                e.RemoveAllListeners();
            }
            mEvents.Clear();
        }

        private void Register<T>(EventName eventName, UnityAction<T> action)
        {
            UnityEventBase e;
            if (!mEvents.TryGetValue(eventName, out e))
            {
                e = new CollisionEvent<T>();
                mEvents[eventName] = e;
            }

            (e as CollisionEvent<T>).AddListener(action);
        }

        private void UnRegister<T>(EventName eventName, UnityAction<T> action)
        {
            UnityEventBase e;
            if (mEvents.TryGetValue(eventName, out e))
                (e as CollisionEvent<T>).RemoveListener(action);
        }

        private void Dispatch<T>(EventName eventName, T value)
        {
            UnityEventBase e;
            if (mEvents.TryGetValue(eventName, out e))
                (e as CollisionEvent<T>).Invoke(value);
        }
        
        
        private enum EventName
        {
            CollisionEnter,
            CollisionExit,
            CollisionStay,
            CollisionEnter2D,
            CollisionExit2D,
            CollisionStay2D,
            TriggerEnter,
            TriggerExit,
            TriggerStay,
            TriggerEnter2D,
            TriggerExit2D,
            TriggerStay2D,
        }

        public void RegisterCollisionEnter(UnityAction<Collision> action) { Register(EventName.CollisionEnter, action); }
        public void UnRegisterCollisionEnter(UnityAction<Collision> action) { UnRegister(EventName.CollisionEnter, action); }
        private void OnCollisionEnter(Collision collision) { Dispatch(EventName.CollisionEnter, collision); }
        
        public void RegisterCollisionExit(UnityAction<Collision> action) { Register(EventName.CollisionExit, action); }
        public void UnRegisterCollisionExit(UnityAction<Collision> action) { UnRegister(EventName.CollisionExit, action); }
        private void OnCollisionExit(Collision collision) { Dispatch(EventName.CollisionExit, collision); }

        public void RegisterCollisionStay(UnityAction<Collision> action) { Register(EventName.CollisionStay, action); }
        public void UnRegisterCollisionStay(UnityAction<Collision> action) { UnRegister(EventName.CollisionStay, action); }
        private void CollisionStay(Collision collision) { Dispatch(EventName.CollisionStay, collision); }
        
        public void RegisterCollisionEnter2D(UnityAction<Collision2D> action) { Register(EventName.CollisionEnter2D, action); }
        public void UnRegisterCollisionEnter2D(UnityAction<Collision2D> action) { UnRegister(EventName.CollisionEnter2D, action); }
        private void OnCollisionEnter2D(Collision2D collision) { Dispatch(EventName.CollisionEnter2D, collision); }
        
        public void RegisterCollisionExit2D(UnityAction<Collision2D> action) { Register(EventName.CollisionExit2D, action); }
        public void UnRegisterCollisionExit2D(UnityAction<Collision2D> action) { UnRegister(EventName.CollisionExit2D, action); }
        private void OnCollisionExit2D(Collision2D collision) { Dispatch(EventName.CollisionExit2D, collision); }

        public void RegisterCollisionStay2D(UnityAction<Collision2D> action) { Register(EventName.CollisionStay2D, action); }
        public void UnRegisterCollisionStay2D(UnityAction<Collision2D> action) { UnRegister(EventName.CollisionStay2D, action); }
        private void CollisionStay2D(Collision2D collision) { Dispatch(EventName.CollisionStay2D, collision); }
        
        public void RegisterTriggerEnter(UnityAction<Collider> action) { Register(EventName.TriggerEnter, action); }
        public void UnRegisterTriggerEnter(UnityAction<Collider> action) { UnRegister(EventName.TriggerEnter, action); }
        private void OnTriggerEnter(Collider collider) { Dispatch(EventName.TriggerEnter, collider); }

        public void RegisterTriggerExit(UnityAction<Collider> action) { Register(EventName.TriggerExit, action); }
        public void UnRegisterTriggerExit(UnityAction<Collider> action) { UnRegister(EventName.TriggerExit, action); }
        private void OnTriggerExit(Collider collider) { Dispatch(EventName.TriggerExit, collider); }
        
        public void RegisterTriggerStay(UnityAction<Collider> action) { Register(EventName.TriggerStay, action); }
        public void UnRegisterTriggerStay(UnityAction<Collider> action) { UnRegister(EventName.TriggerStay, action); }
        private void OnTriggerStay(Collider collider) { Dispatch(EventName.TriggerStay, collider); }        

        public void RegisterTriggerEnter2D(UnityAction<Collider2D> action) { Register(EventName.TriggerEnter2D, action); }
        public void UnRegisterTriggerEnter2D(UnityAction<Collider2D> action) { UnRegister(EventName.TriggerEnter2D, action); }
        private void OnTriggerEnter2D(Collider2D collider) { Dispatch(EventName.TriggerEnter2D, collider); }

        public void RegisterTriggerExit2D(UnityAction<Collider2D> action) { Register(EventName.TriggerExit2D, action); }
        public void UnRegisterTriggerExit2D(UnityAction<Collider2D> action) { UnRegister(EventName.TriggerExit2D, action); }
        private void OnTriggerExit2D(Collider2D collider) { Dispatch(EventName.TriggerExit2D, collider); }
        
        public void RegisterTriggerStay2D(UnityAction<Collider2D> action) { Register(EventName.TriggerStay2D, action); }
        public void UnRegisterTriggerStay2D(UnityAction<Collider2D> action) { UnRegister(EventName.TriggerStay2D, action); }
        private void OnTriggerStay2D(Collider2D collider) { Dispatch(EventName.TriggerStay2D, collider); }
    }
}