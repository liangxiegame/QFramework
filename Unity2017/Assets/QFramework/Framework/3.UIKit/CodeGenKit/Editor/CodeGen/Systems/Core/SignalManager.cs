using System;
using System.Collections.Generic;

namespace QFramework.CodeGen
{
    public class SignalManager<T> : ISignalManager where T : class
    {
        private List<T> mListeners;

        public List<T> Listeners
        {
            get { return mListeners ?? (mListeners = new List<T>()); }
            set { mListeners = value; }
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

        public IDisposable Subscribe(T listener)
        {
            if (!Listeners.Contains(listener))
                Listeners.Add(listener);

            return new CustomDisposable(() => { Unsubscribe(listener); });
        }

        private void Unsubscribe(T listener)
        {
            Listeners.Remove(listener);
        }
    }
}