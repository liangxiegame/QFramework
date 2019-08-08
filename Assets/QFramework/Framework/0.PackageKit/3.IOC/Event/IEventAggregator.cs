using UniRx;
using System; // Required for WP8 and Store APPS

namespace QF
{
    public interface IEventAggregator
    {
        IObservable<TEvent> GetEvent<TEvent>();
        void Publish<TEvent>(TEvent evt);
    }
}