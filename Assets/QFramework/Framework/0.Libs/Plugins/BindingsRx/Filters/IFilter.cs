using System;
using UniRx;

namespace BindingsRx.Filters
{
    public interface IFilter<T>
    {
        IObservable<T> InputFilter(IObservable<T> inputStream);
        IObservable<T> OutputFilter(IObservable<T> outputStream);
    }
}