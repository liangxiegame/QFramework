using System;

namespace QF.GraphDesigner
{
    public interface IEventManager
    {
        System.Action AddListener(object listener);
        void Signal(Action<object> obj);
    }
}