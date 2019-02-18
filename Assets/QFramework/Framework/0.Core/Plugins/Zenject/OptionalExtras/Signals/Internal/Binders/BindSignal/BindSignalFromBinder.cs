using System;
using ModestTree;

namespace Zenject
{
    public class BindSignalFromBinder<TObject, TSignal>
    {
        readonly BindStatement _bindStatement;
        readonly Func<TObject, Action<TSignal>> _methodGetter;
        readonly DiContainer _container;
        readonly SignalBindingBindInfo _signalBindInfo;

        public BindSignalFromBinder(
            SignalBindingBindInfo signalBindInfo, BindStatement bindStatement, Func<TObject, Action<TSignal>> methodGetter,
            DiContainer container)
        {
            _signalBindInfo = signalBindInfo;
            _bindStatement = bindStatement;
            _methodGetter = methodGetter;
            _container = container;
        }

        public SignalCopyBinder FromResolve()
        {
            return From(x => x.FromResolve().AsCached());
        }

        public SignalCopyBinder FromResolveAll()
        {
            return From(x => x.FromResolveAll().AsCached());
        }

        public SignalCopyBinder FromNew()
        {
            return From(x => x.FromNew().AsCached());
        }

        public SignalCopyBinder From(Action<ConcreteBinderGeneric<TObject>> objectBindCallback)
        {
            Assert.That(!_bindStatement.HasFinalizer);
            _bindStatement.SetFinalizer(new NullBindingFinalizer());

            var objectLookupId = Guid.NewGuid();

            // Very important here that we use NoFlush otherwise the main binding will be finalized early
            var objectBinder = _container.BindNoFlush<TObject>().WithId(objectLookupId);

            objectBindCallback(objectBinder);

            // We need to do this to make sure SignalCallbackWithLookupWrapper does not have
            // generic types to avoid AOT issues
            Func<object, Action<object>> methodGetterMapper =
                obj => s => _methodGetter((TObject)obj)((TSignal)s);

            var wrapperBinder = _container.Bind<IDisposable>()
                .To<SignalCallbackWithLookupWrapper>()
                .AsCached()
                .WithArguments(_signalBindInfo, typeof(TObject), objectLookupId, methodGetterMapper)
                .NonLazy();

            var copyBinder = new SignalCopyBinder( wrapperBinder.BindInfo);
            // Make sure if they use one of the Copy/Move methods that it applies to both bindings
            copyBinder.AddCopyBindInfo(objectBinder.BindInfo);
            return copyBinder;
        }
    }
}
