using ModestTree;

namespace Zenject
{
    public class FactoryProviderWrapper<TContract> : IFactory<TContract>
    {
        readonly IProvider _provider;
        readonly InjectContext _injectContext;

        public FactoryProviderWrapper(
            IProvider provider, InjectContext injectContext)
        {
            Assert.That(injectContext.MemberType.DerivesFromOrEqual<TContract>());

            _provider = provider;
            _injectContext = injectContext;
        }

        public TContract Create()
        {
            var instance = _provider.GetInstance(_injectContext);

            if (_injectContext.Container.IsValidating)
            {
                // During validation it is sufficient to just call the _provider.GetInstance
                return default(TContract);
            }

            Assert.That(instance == null
                || instance.GetType().DerivesFromOrEqual(_injectContext.MemberType));

            return (TContract)instance;
        }
    }
}

