using System.Linq;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class PlaceholderFactoryBindingFinalizer<TContract> : ProviderBindingFinalizer
    {
        readonly FactoryBindInfo _factoryBindInfo;

        public PlaceholderFactoryBindingFinalizer(
            BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(bindInfo)
        {
            // Note that it doesn't derive from PlaceholderFactory<TContract>
            // when used with To<>, so we can only check IPlaceholderFactory
            Assert.That(factoryBindInfo.FactoryType.DerivesFrom<IPlaceholderFactory>());

            _factoryBindInfo = factoryBindInfo;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            var provider = _factoryBindInfo.ProviderFunc(container);

            var transientProvider = new TransientProvider(
                _factoryBindInfo.FactoryType,
                container,
                _factoryBindInfo.Arguments.Concat(
                    InjectUtil.CreateArgListExplicit(
                        provider,
                        new InjectContext(container, typeof(TContract)))).ToList(),
                BindInfo.ContextInfo, BindInfo.ConcreteIdentifier, null);

            IProvider mainProvider;

            if (BindInfo.Scope == ScopeTypes.Unset || BindInfo.Scope == ScopeTypes.Singleton)
            {
                mainProvider = BindingUtil.CreateCachedProvider(transientProvider);
            }
            else
            {
                Assert.IsEqual(BindInfo.Scope, ScopeTypes.Transient);
                mainProvider = transientProvider;
            }

            RegisterProviderForAllContracts(container, mainProvider);
        }
    }
}
