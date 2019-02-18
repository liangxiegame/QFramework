using System;

namespace Zenject
{
    // Note that there's a reason we don't just have a generic
    // argument for signal type - because when using struct type signals it throws
    // exceptions on AOT platforms
    public class SignalCallbackWithLookupWrapper : IDisposable
    {
        readonly DiContainer _container;
        readonly SignalBus _signalBus;
        readonly Guid _lookupId;
        readonly Func<object, Action<object>> _methodGetter;
        readonly Type _objectType;
        readonly Type _signalType;
        readonly object _identifier;

        public SignalCallbackWithLookupWrapper(
            SignalBindingBindInfo signalBindInfo,
            Type objectType,
            Guid lookupId,
            Func<object, Action<object>> methodGetter,
            SignalBus signalBus,
            DiContainer container)
        {
            _objectType = objectType;
            _signalType = signalBindInfo.SignalType;
            _identifier = signalBindInfo.Identifier;
            _container = container;
            _methodGetter = methodGetter;
            _signalBus = signalBus;
            _lookupId = lookupId;

            signalBus.SubscribeId(signalBindInfo.SignalType, _identifier, OnSignalFired);
        }

        void OnSignalFired(object signal)
        {
            var objects = _container.ResolveIdAll(_objectType, _lookupId);

            for (int i = 0; i < objects.Count; i++)
            {
                _methodGetter(objects[i])(signal);
            }
        }

        public void Dispose()
        {
            _signalBus.UnsubscribeId(_signalType, _identifier, OnSignalFired);
        }
    }
}

