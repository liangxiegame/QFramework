using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace Zenject
{
    public class SignalBus : ILateDisposable
    {
        readonly SignalSubscription.Pool _subscriptionPool;
        readonly Dictionary<BindingId, SignalDeclaration> _localDeclarationMap;
        readonly SignalBus _parentBus;
        readonly Dictionary<SignalSubscriptionId, SignalSubscription> _subscriptionMap = new Dictionary<SignalSubscriptionId, SignalSubscription>();
        readonly ZenjectSettings.SignalSettings _settings;
        readonly SignalDeclaration.Factory _signalDeclarationFactory;
        readonly DiContainer _container;

        public SignalBus(
            [Inject(Source = InjectSources.Local)]
            List<SignalDeclaration> signalDeclarations,
            [Inject(Source = InjectSources.Parent, Optional = true)]
            SignalBus parentBus,
            [InjectOptional]
            ZenjectSettings zenjectSettings,
            SignalSubscription.Pool subscriptionPool,
            SignalDeclaration.Factory signalDeclarationFactory,
            DiContainer container)
        {
            _subscriptionPool = subscriptionPool;
            zenjectSettings = zenjectSettings ?? ZenjectSettings.Default;
            _settings = zenjectSettings.Signals ?? ZenjectSettings.SignalSettings.Default;
            _signalDeclarationFactory = signalDeclarationFactory;
            _container = container;

            _localDeclarationMap = signalDeclarations.ToDictionary(x => x.BindingId, x => x);
            _parentBus = parentBus;
        }

        public SignalBus ParentBus
        {
            get { return _parentBus; }
        }

        public int NumSubscribers
        {
            get { return _subscriptionMap.Count; }
        }

        public void LateDispose()
        {
            if (_settings.RequireStrictUnsubscribe)
            {
                if (!_subscriptionMap.IsEmpty())
                {
                    throw Assert.CreateException(
                        "Found subscriptions for signals '{0}' in SignalBus.LateDispose!  Either add the explicit Unsubscribe or set SignalSettings.AutoUnsubscribeInDispose to true",
                        _subscriptionMap.Values.Select(x => x.SignalId.ToString()).Join(", "));
                }
            }
            else
            {
                foreach (var subscription in _subscriptionMap.Values)
                {
                    subscription.Dispose();
                }
            }

            foreach (var declaration in _localDeclarationMap.Values)
            {
                declaration.Dispose();
            }
        }

        public void FireId<TSignal>(object identifier, TSignal signal)
        {
            // Do this before creating the signal so that it throws if the signal was not declared
            var declaration = GetDeclaration(typeof(TSignal), identifier, true);

            declaration.Fire(signal);
        }

        public void Fire<TSignal>(TSignal signal)
        {
            FireId<TSignal>(null, signal);
        }

        public void FireId<TSignal>(object identifier)
        {
            // Do this before creating the signal so that it throws if the signal was not declared
            var declaration = GetDeclaration(typeof(TSignal), identifier, true);

            declaration.Fire(
                (TSignal)Activator.CreateInstance(typeof(TSignal)));
        }

        public void Fire<TSignal>()
        {
            FireId<TSignal>(null);
        }

        public void FireId(object identifier, object signal)
        {
            GetDeclaration(signal.GetType(), identifier, true).Fire(signal);
        }

        public void Fire(object signal)
        {
            FireId(null, signal);
        }

        public void TryFireId<TSignal>(object identifier, TSignal signal)
        {
            var declaration = GetDeclaration(typeof(TSignal), identifier, false);
            if (declaration != null)
            {
                declaration.Fire(signal);
            }
        }

        public void TryFire<TSignal>(TSignal signal)
        {
            TryFireId<TSignal>(null, signal);
        }

        public void TryFireId<TSignal>(object identifier)
        {
            var declaration = GetDeclaration(typeof(TSignal), identifier, false);
            if (declaration != null)
            {
                declaration.Fire(
                    (TSignal)Activator.CreateInstance(typeof(TSignal)));
            }
        }

        public void TryFire<TSignal>()
        {
            TryFireId<TSignal>(null);
        }

        public void TryFireId(object identifier, object signal)
        {
            var declaration = GetDeclaration(signal.GetType(), identifier, false);
            if (declaration != null)
            {
                declaration.Fire(signal);
            }
        }

        public void TryFire(object signal)
        {
            TryFireId(null, signal);
        }

#if ZEN_SIGNALS_ADD_UNIRX
        public IObservable<TSignal> GetStreamId<TSignal>(object identifier)
        {
            return GetStream(typeof(TSignal), identifier).Select(x => (TSignal)x);
        }

        public IObservable<TSignal> GetStream<TSignal>()
        {
            return GetStreamId<TSignal>(null);
        }

        public IObservable<object> GetStreamId(Type signalType, object identifier)
        {
            return GetDeclaration(signalType, identifier, true).Stream;
        }

        public IObservable<object> GetStream(Type signalType)
        {
            return GetStreamId(signalType, null);
        }
#endif

        public void SubscribeId<TSignal>(object identifier, Action callback)
        {
            Action<object> wrapperCallback = args => callback();
            SubscribeInternal(typeof(TSignal), identifier, callback, wrapperCallback);
        }

        public void Subscribe<TSignal>(Action callback)
        {
            SubscribeId<TSignal>(null, callback);
        }

        public void SubscribeId<TSignal>(object identifier, Action<TSignal> callback)
        {
            Action<object> wrapperCallback = args => callback((TSignal)args);
            SubscribeInternal(typeof(TSignal), identifier, callback, wrapperCallback);
        }

        public void Subscribe<TSignal>(Action<TSignal> callback)
        {
            SubscribeId<TSignal>(null, callback);
        }

        public void SubscribeId(Type signalType, object identifier, Action<object> callback)
        {
            SubscribeInternal(signalType, identifier, callback, callback);
        }

        public void Subscribe(Type signalType, Action<object> callback)
        {
            SubscribeId(signalType, null, callback);
        }

        public void UnsubscribeId<TSignal>(object identifier, Action callback)
        {
            UnsubscribeId(typeof(TSignal), identifier, callback);
        }

        public void Unsubscribe<TSignal>(Action callback)
        {
            UnsubscribeId<TSignal>(null, callback);
        }

        public void UnsubscribeId(Type signalType, object identifier, Action callback)
        {
            UnsubscribeInternal(signalType, identifier, callback, true);
        }

        public void Unsubscribe(Type signalType, Action callback)
        {
            UnsubscribeId(signalType, null, callback);
        }

        public void UnsubscribeId(Type signalType, object identifier, Action<object> callback)
        {
            UnsubscribeInternal(signalType, identifier, callback, true);
        }

        public void Unsubscribe(Type signalType, Action<object> callback)
        {
            UnsubscribeId(signalType, null, callback);
        }

        public void UnsubscribeId<TSignal>(object identifier, Action<TSignal> callback)
        {
            UnsubscribeInternal(typeof(TSignal), identifier, callback, true);
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback)
        {
            UnsubscribeId<TSignal>(null, callback);
        }

        public void TryUnsubscribeId<TSignal>(object identifier, Action callback)
        {
            UnsubscribeInternal(typeof(TSignal), identifier, callback, false);
        }

        public void TryUnsubscribe<TSignal>(Action callback)
        {
            TryUnsubscribeId<TSignal>(null, callback);
        }

        public void TryUnsubscribeId(Type signalType, object identifier, Action callback)
        {
            UnsubscribeInternal(signalType, identifier, callback, false);
        }

        public void TryUnsubscribe(Type signalType, Action callback)
        {
            TryUnsubscribeId(signalType, null, callback);
        }

        public void TryUnsubscribeId(Type signalType, object identifier, Action<object> callback)
        {
            UnsubscribeInternal(signalType, identifier, callback, false);
        }

        public void TryUnsubscribe(Type signalType, Action<object> callback)
        {
            TryUnsubscribeId(signalType, null, callback);
        }

        public void TryUnsubscribeId<TSignal>(object identifier, Action<TSignal> callback)
        {
            UnsubscribeInternal(typeof(TSignal), identifier, callback, false);
        }

        public void TryUnsubscribe<TSignal>(Action<TSignal> callback)
        {
            TryUnsubscribeId<TSignal>(null, callback);
        }

        void UnsubscribeInternal(Type signalType, object identifier, object token, bool throwIfMissing)
        {
            UnsubscribeInternal(new BindingId(signalType, identifier), token, throwIfMissing);
        }

        void UnsubscribeInternal(BindingId signalId, object token, bool throwIfMissing)
        {
            UnsubscribeInternal(
                new SignalSubscriptionId(signalId, token), throwIfMissing);
        }

        void UnsubscribeInternal(SignalSubscriptionId id, bool throwIfMissing)
        {
            SignalSubscription subscription;

            if (_subscriptionMap.TryGetValue(id, out subscription))
            {
                _subscriptionMap.RemoveWithConfirm(id);
                subscription.Dispose();
            }
            else
            {
                if (throwIfMissing)
                {
                    throw Assert.CreateException(
                        "Called unsubscribe for signal '{0}' but could not find corresponding subscribe.  If this is intentional, call TryUnsubscribe instead.");
                }
            }
        }

        void SubscribeInternal(Type signalType, object identifier, object token, Action<object> callback)
        {
            SubscribeInternal(new BindingId(signalType, identifier), token, callback);
        }

        void SubscribeInternal(BindingId signalId, object token, Action<object> callback)
        {
            SubscribeInternal(
                new SignalSubscriptionId(signalId, token), callback);
        }

        void SubscribeInternal(SignalSubscriptionId id, Action<object> callback)
        {
            Assert.That(!_subscriptionMap.ContainsKey(id),
                "Tried subscribing to the same signal with the same callback on Zenject.SignalBus");

            var declaration = GetDeclaration(id.SignalId, true);
            var subscription = _subscriptionPool.Spawn(callback, declaration);

            _subscriptionMap.Add(id, subscription);
        }

        public void DeclareSignal<T>(
            object identifier = null, SignalMissingHandlerResponses? missingHandlerResponse = null, bool? forceAsync = null, int? asyncTickPriority = null)
        {
            DeclareSignal(typeof(T), identifier, missingHandlerResponse, forceAsync, asyncTickPriority);
        }

        public void DeclareSignal(
            Type signalType, object identifier = null, SignalMissingHandlerResponses? missingHandlerResponse = null, bool? forceAsync = null, int? asyncTickPriority = null)
        {
            var bindInfo = SignalExtensions.CreateDefaultSignalDeclarationBindInfo(_container, signalType);

            bindInfo.Identifier = identifier;

            if (missingHandlerResponse.HasValue)
            {
                bindInfo.Identifier = missingHandlerResponse.Value;
            }

            if (forceAsync.HasValue)
            {
                bindInfo.RunAsync = forceAsync.Value;
            }

            if (asyncTickPriority.HasValue)
            {
                bindInfo.TickPriority = asyncTickPriority.Value;
            }

            var declaration = _signalDeclarationFactory.Create(bindInfo);

            _localDeclarationMap.Add(declaration.BindingId, declaration);
        }

        SignalDeclaration GetDeclaration(Type signalType, object identifier, bool requireDeclaration)
        {
            return GetDeclaration(new BindingId(signalType, identifier), requireDeclaration);
        }

        SignalDeclaration GetDeclaration(BindingId signalId, bool requireDeclaration)
        {
            SignalDeclaration handler;

            if (_localDeclarationMap.TryGetValue(signalId, out handler))
            {
                return handler;
            }

            if (_parentBus != null)
            {
                return _parentBus.GetDeclaration(signalId, requireDeclaration);
            }

            if (requireDeclaration)
            {
                throw Assert.CreateException("Fired undeclared signal '{0}'!", signalId);
            }

            return null;
        }
    }
}
