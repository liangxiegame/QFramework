using System;
using System.Collections.Generic;
using QFramework.ILRuntime;

namespace QFramework
{
    public class ILEventSystemNode<TArchitecture> : ILPool<ILEventSystemNode<TArchitecture>>
        where TArchitecture : ILArchitecture<TArchitecture>, new()
    {
        List<ILCanDispose> mDisposes = new List<ILCanDispose>();

        public void Register<TEvent>(Action<TEvent> onEvent)
        {
            var icanDispose = ILSingleton<TArchitecture>.Instance.RegisterEvent<TEvent>(onEvent);
            mDisposes.Add(icanDispose);
        }

        void UnRegisterAll()
        {
            foreach (var canDispose in mDisposes)
            {
                canDispose.Dispose();
            }

            mDisposes.Clear();
        }

        protected override void OnRecycle()
        {
            UnRegisterAll();
        }
    }
}