using System;

namespace QFramework.GraphDesigner
{
    public interface IEventManager
    {
        Action AddListener(object listener);
        void Signal(Action<object> obj);
    }
}