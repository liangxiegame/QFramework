using System;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class EventManager<T> : IEventManager where T : class
    {
        private List<T> _listeners;

        public List<T> Listeners
        {
            get { return _listeners ?? (_listeners = new List<T>()); }
            set { _listeners = value; }
        }

        public void Signal(Action<object> obj)
        {
            foreach (var item in Listeners)
            {
                var item1 = item;
                obj(item1);
            }
        }
        public void Signal(Action<T> action)
        {
            foreach (var item in Listeners)
            {
                //InvertApplication.Log(typeof(T).Name + " was signaled on " + item.GetType().Name);
                var item1 = item;
                action(item1);
            }
        }

        public System.Action Subscribe(T listener)
        {
            if (!Listeners.Contains(listener))
                Listeners.Add(listener);

            return () => { Unsubscribe(listener); };
        }

        private void Unsubscribe(T listener)
        {
            Listeners.Remove(listener);
        }

        public System.Action AddListener(object listener)
        {
            return Subscribe(listener as T);
        }
    }
}