using System;
using System.Collections.Generic;

namespace QFramework.CodeGen
{
    public class EventManager<T> : IEventManager where T : class
    {
        private List<T> _listeners;

        public List<T> Listeners
        {
            get { return _listeners ?? (_listeners = new List<T>()); }
            set { _listeners = value; }
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
    }
}