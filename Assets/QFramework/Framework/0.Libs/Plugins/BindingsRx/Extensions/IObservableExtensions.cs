using System;
using System.Collections.Generic;
using BindingsRx.Filters;
using UniRx;

namespace BindingsRx.Extensions
{
    public static class IObservableExtensions
    {
        public static IObservable<T> ApplyInputFilters<T>(this IObservable<T> observable, IFilter<T>[] filters)
        {
            if (filters.Length == 0)
            { return observable; }

            var filteredObservable = observable;
            foreach (var filter in filters)
            { filteredObservable = filter.InputFilter(filteredObservable); }

            return filteredObservable;
        }

        public static IObservable<T> ApplyOutputFilters<T>(this IObservable<T> observable, IFilter<T>[] filters)
        {
            if (filters.Length == 0)
            { return observable; }

            var filteredObservable = observable;
            foreach (var filter in filters)
            { filteredObservable = filter.OutputFilter(filteredObservable); }

            return filteredObservable;
        }
    }
}