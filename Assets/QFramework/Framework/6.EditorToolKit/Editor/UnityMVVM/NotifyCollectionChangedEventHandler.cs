namespace QF.MVVM
{
    #if !NETFX_CORE
    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs changeArgs);
    #endif
}