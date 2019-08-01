using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using QFramework;
using UniRx;

namespace uFrame.Kernel
{
    public class EventAggregator : IEventAggregator, ISubject<object>
    {
        bool isDisposed;
        readonly Subject<object> eventsSubject = new Subject<object>();

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return eventsSubject.Where(p =>
            {
                return p is TEvent;
            }).Select(delegate (object p)
            {
                return (TEvent)p;
            });
        }

        public void Publish<TEvent>(TEvent evt)
        {
            eventsSubject.OnNext(evt);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            eventsSubject.Dispose();
            isDisposed = true;
        }

        public void OnCompleted()
        {
            eventsSubject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            eventsSubject.OnError(error);
        }

        public void OnNext(object value)
        {
            eventsSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return eventsSubject.Subscribe(observer);
        }
    }

    public interface IEventManager
    {
        int EventId { get; set; }
        Type For { get; }
        void Publish(object evt);

    }

    public class EventManager<TEventType> : IEventManager
    {
        private Subject<TEventType> _eventType;

        public Subject<TEventType> EventSubject
        {
            get { return _eventType ?? (_eventType = new Subject<TEventType>()); }
            set { _eventType = value; }
        }

        private int _eventId;
        public int EventId
        {
            get
            {
                if (_eventId > 0)
                    return _eventId;

                var eventIdAttribute =
                    For.GetCustomAttributes(typeof(EventId), true).FirstOrDefault() as
                        EventId;
                if (eventIdAttribute != null)
                {
                    return _eventId = eventIdAttribute.Identifier;
                }
                return _eventId;
            }
            set { _eventId = value; }
        }

        public Type For { get { return typeof(TEventType); } }
        public void Publish(object evt)
        {
            if (_eventType != null)
            {
                _eventType.OnNext((TEventType)evt);
            }
        }
    }

    public class EcsEventAggregator : IEventAggregator
    {
        private Dictionary<Type, IEventManager> _managers;

        public Dictionary<Type, IEventManager> Managers
        {
            get { return _managers ?? (_managers = new Dictionary<Type, IEventManager>()); }
            set { _managers = value; }
        }
        private Dictionary<int, IEventManager> _managersById;

        public Dictionary<int, IEventManager> ManagersById
        {
            get { return _managersById ?? (_managersById = new Dictionary<int, IEventManager>()); }
            set { _managersById = value; }
        }

        public IEventManager GetEventManager(int eventId)
        {
            if (ManagersById.ContainsKey(eventId))
                return ManagersById[eventId];
            return null;
        }
        //public IEventManager GetEventManager(Type type)
        //{
        //    if (ManagersById.ContainsKey(eventId))
        //        return ManagersById[eventId];
        //    return null;
        //}

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            IEventManager eventManager;
            if (!Managers.TryGetValue(typeof(TEvent), out eventManager))
            {
                eventManager = new EventManager<TEvent>();
                Managers.Add(typeof(TEvent), eventManager);
                var eventId = eventManager.EventId;
                if (eventId > 0)
                {
                    ManagersById.Add(eventId, eventManager);
                }
                else
                {
                    // create warning here that eventid attribute is not set
                }
            }
            var em = eventManager as EventManager<TEvent>;
            if (em == null) return null;
            return em.EventSubject;
        }

        public void Publish<TEvent>(TEvent evt)
        {
            IEventManager eventManager;

            if (!Managers.TryGetValue(evt.GetType(), out eventManager))
            {
                // No listeners anyways
                return;
            }
            eventManager.Publish(evt);

        }

        public void PublishById(int eventId, object data)
        {
            var evt = GetEventManager(eventId);
            if (evt != null)
                evt.Publish(data);
        }

    }

}
