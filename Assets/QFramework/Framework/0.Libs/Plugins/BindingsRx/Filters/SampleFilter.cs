using System;
using UniRx;
using UnityEngine;

namespace BindingsRx.Filters
{
    public class SampleFilter<T> : IFilter<T>
    {
        public TimeSpan SampleRate { get; set; }

        public SampleFilter(TimeSpan sampleRate)
        { SampleRate = sampleRate; }

        public IObservable<T> InputFilter(IObservable<T> inputStream)
        { return inputStream.Sample(SampleRate); }

        public IObservable<T> OutputFilter(IObservable<T> outputStream)
        { return outputStream.Sample(SampleRate); }
    }
}