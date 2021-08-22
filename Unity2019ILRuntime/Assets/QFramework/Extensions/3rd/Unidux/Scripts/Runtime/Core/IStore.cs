using System;
using UniRx;

namespace Unidux
{
    public interface IStore<TState> where TState : DvaState
    {
        TState State { get; set; }
        Subject<TState> Subject { get; }
        
        object Dispatch(object action);
        void Update();
    }

    public interface IStoreObject
    {
        object ObjectState { get; set; }
        IObservable<object> ObjectSubject { get; }
        Type StateType { get; }
    }
}
