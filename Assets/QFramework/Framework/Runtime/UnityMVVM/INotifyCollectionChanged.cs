namespace QF.MVVM
{
    #if !NETFX_CORE
    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
    #endif
}