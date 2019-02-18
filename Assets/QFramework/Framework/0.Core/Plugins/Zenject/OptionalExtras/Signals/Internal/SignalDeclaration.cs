using System;
using System.Collections.Generic;
using ModestTree;
#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace Zenject
{
    public class SignalDeclaration : ITickable, IDisposable
    {
        readonly List<SignalSubscription> _subscriptions = new List<SignalSubscription>();
        readonly List<object> _asyncQueue = new List<object>();
        readonly BindingId _bindingId;
        readonly SignalMissingHandlerResponses _missingHandlerResponses;
        readonly bool _isAsync;
        readonly ZenjectSettings.SignalSettings _settings;

#if ZEN_SIGNALS_ADD_UNIRX
        readonly Subject<object> _stream = new Subject<object>();
#endif

        public SignalDeclaration(
            SignalDeclarationBindInfo bindInfo,
            [InjectOptional]
            ZenjectSettings zenjectSettings)
        {
            zenjectSettings = zenjectSettings ?? ZenjectSettings.Default;
            _settings = zenjectSettings.Signals ?? ZenjectSettings.SignalSettings.Default;

            _bindingId = new BindingId(bindInfo.SignalType, bindInfo.Identifier);
            _missingHandlerResponses = bindInfo.MissingHandlerResponse;
            _isAsync = bindInfo.RunAsync;
            TickPriority = bindInfo.TickPriority;
        }

#if ZEN_SIGNALS_ADD_UNIRX
        public IObservable<object> Stream
        {
            get { return _stream; }
        }
#endif

        public int TickPriority
        {
            get; private set;
        }

        public bool IsAsync
        {
            get { return _isAsync; }
        }

        public BindingId BindingId
        {
            get { return _bindingId; }
        }

        public void Dispose()
        {
            if (_settings.RequireStrictUnsubscribe)
            {
                Assert.That(_subscriptions.IsEmpty(),
                    "Found {0} signal handlers still added to declaration {1}", _subscriptions.Count, _bindingId);
            }
            else
            {
                // We can't rely entirely on the destruction order in Unity because of
                // the fact that OnDestroy is completely unpredictable.
                // So if you have a GameObjectContext at the root level in your scene, then it
                // might be destroyed AFTER the SceneContext.  So if you have some signal declarations
                // in the scene context, they might get disposed before some of the subscriptions
                // so in this case you need to disconnect from the subscription so that it doesn't
                // try to remove itself after the declaration has been destroyed
                for (int i = 0; i < _subscriptions.Count; i++)
                {
                    _subscriptions[i].OnDeclarationDespawned();
                }
            }
        }

        public void Fire(object signal)
        {
            Assert.That(signal.GetType().DerivesFromOrEqual(_bindingId.Type));

            if (_isAsync)
            {
                _asyncQueue.Add(signal);
            }
            else
            {
                // Cache the callback list to allow handlers to be added from within callbacks
                using (var block = DisposeBlock.Spawn())
                {
                    var subscriptions = block.SpawnList<SignalSubscription>();
                    subscriptions.AddRange(_subscriptions);
                    FireInternal(subscriptions, signal);
                }
            }
        }

        void FireInternal(List<SignalSubscription> subscriptions, object signal)
        {
            if (subscriptions.IsEmpty()
#if ZEN_SIGNALS_ADD_UNIRX
                && !_stream.HasObservers
#endif
                )
            {
                if (_missingHandlerResponses == SignalMissingHandlerResponses.Warn)
                {
                    Log.Warn("Fired signal '{0}' but no subscriptions found!  If this is intentional then either add OptionalSubscriber() to the binding or change the default in ZenjectSettings", signal.GetType());
                }
                else if (_missingHandlerResponses == SignalMissingHandlerResponses.Throw)
                {
                    throw Assert.CreateException(
                        "Fired signal '{0}' but no subscriptions found!  If this is intentional then either add OptionalSubscriber() to the binding or change the default in ZenjectSettings", signal.GetType());
                }
            }

            for (int i = 0; i < subscriptions.Count; i++)
            {
                var subscription = subscriptions[i];

                // This is a weird check for the very rare case where an Unsubscribe is called
                // from within the same callback (see TestSignalsAdvanced.TestSubscribeUnsubscribeInsideHandler)
                if (_subscriptions.Contains(subscription))
                {
                    subscription.Invoke(signal);
                }
            }

#if ZEN_SIGNALS_ADD_UNIRX
            _stream.OnNext(signal);
#endif
        }

        public void Tick()
        {
            Assert.That(_isAsync);

            if (!_asyncQueue.IsEmpty())
            {
                // Cache the callback list to allow handlers to be added from within callbacks
                using (var block = DisposeBlock.Spawn())
                {
                    var subscriptions = block.SpawnList<SignalSubscription>();
                    subscriptions.AddRange(_subscriptions);

                    // Cache the signals so that if the signal is fired again inside the handler that it
                    // is not executed until next frame
                    var signals = block.SpawnList<object>();
                    signals.AddRange(_asyncQueue);

                    _asyncQueue.Clear();

                    for (int i = 0; i < signals.Count; i++)
                    {
                        FireInternal(subscriptions, signals[i]);
                    }
                }
            }
        }

        public void Add(SignalSubscription subscription)
        {
            Assert.That(!_subscriptions.Contains(subscription));
            _subscriptions.Add(subscription);
        }

        public void Remove(SignalSubscription subscription)
        {
            _subscriptions.RemoveWithConfirm(subscription);
        }

        public class Factory : PlaceholderFactory<SignalDeclarationBindInfo, SignalDeclaration>
        {
        }
    }
}
