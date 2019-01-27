using System;

namespace QFramework.MVVM
{
    #if !NETFX_CORE
    public delegate void NotifyCollectionChangedEventHandler(Object sender, NotifyCollectionChangedEventArgs changeArgs);
    #endif
}