using System;

namespace Zenject
{
    // Note that there's a reason we don't just have a generic
    // argument for signal type - because when using struct type signals it throws
    // exceptions on AOT platforms
    public class SignalCallbackWrapper : IDisposable
    {
        readonly SignalBus _signalBus;
        readonly Action<object> _action;
        readonly Type _signalType;
        readonly object _identifier;

        public SignalCallbackWrapper(
            SignalBindingBindInfo bindInfo,
            Action<object> action,
            SignalBus signalBus)
        {
            _signalType = bindInfo.SignalType;
            _identifier = bindInfo.Identifier;
            _signalBus = signalBus;
            _action = action;

            signalBus.SubscribeId(bindInfo.SignalType, _identifier, OnSignalFired);
        }

        void OnSignalFired(object signal)
        {
            _action(signal);
        }

        public void Dispose()
        {
            _signalBus.UnsubscribeId(_signalType, _identifier, OnSignalFired);
        }
    }
}
