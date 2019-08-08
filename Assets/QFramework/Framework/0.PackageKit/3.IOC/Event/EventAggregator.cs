using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace QF
{

    public interface IEventManager
    {
        int EventId { get; set; }
        Type For { get; }
        void Publish(object evt);

    }

    public class EventManager<TEventType> : IEventManager
    {
        private Subject<TEventType> mEventType;

        public Subject<TEventType> EventSubject
        {
            get { return mEventType ?? (mEventType = new Subject<TEventType>()); }
            set { mEventType = value; }
        }

        private int mEventId;
        public int EventId
        {
            get
            {
                if (mEventId > 0)
                    return mEventId;

                var eventIdAttribute =
                    For.GetCustomAttributes(typeof(EventId), true).FirstOrDefault() as
                        EventId;
                if (eventIdAttribute != null)
                {
                    return mEventId = eventIdAttribute.Identifier;
                }
                return mEventId;
            }
            set { mEventId = value; }
        }

        public Type For { get { return typeof(TEventType); } }
        public void Publish(object evt)
        {
            if (mEventType != null)
            {
                mEventType.OnNext((TEventType)evt);
            }
        }
    }

    public class EventAggregator : IEventAggregator
    {
        private Dictionary<Type, IEventManager> mManagers;

        public Dictionary<Type, IEventManager> Managers
        {
            get { return mManagers ?? (mManagers = new Dictionary<Type, IEventManager>()); }
            set { mManagers = value; }
        }
        private Dictionary<int, IEventManager> mManagersById;

        public Dictionary<int, IEventManager> ManagersById
        {
            get { return mManagersById ?? (mManagersById = new Dictionary<int, IEventManager>()); }
            set { mManagersById = value; }
        }

        public IEventManager GetEventManager(int eventId)
        {
            if (ManagersById.ContainsKey(eventId))
                return ManagersById[eventId];
            return null;
        }

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