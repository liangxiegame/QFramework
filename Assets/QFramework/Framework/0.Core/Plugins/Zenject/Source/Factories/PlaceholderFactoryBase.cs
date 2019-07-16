using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public interface IPlaceholderFactory : IValidatable
    {
    }

    // Placeholder factories can be used to choose a creation method in an installer, using FactoryBinder
    public abstract class PlaceholderFactoryBase<TValue> : IPlaceholderFactory
    {
        IProvider _provider;
        InjectContext _injectContext;

        [Inject]
        void Construct(IProvider provider, InjectContext injectContext)
        {
            Assert.IsNotNull(provider);
            Assert.IsNotNull(injectContext);

            _provider = provider;
            _injectContext = injectContext;
        }

        protected TValue CreateInternal(List<TypeValuePair> extraArgs)
        {
            try
            {
                var result = _provider.GetInstance(_injectContext, extraArgs);

                if (_injectContext.Container.IsValidating && result is ValidationMarker)
                {
                    return default(TValue);
                }

                Assert.That(result == null || result.GetType().DerivesFromOrEqual<TValue>());

                return (TValue) result;
            }
            catch (Exception e)
            {
                throw new ZenjectException(
                    "Error during construction of type '{0}' via {1}.Create method!".Fmt(typeof(TValue), GetType()), e);
            }
        }

        public virtual void Validate()
        {
            _provider.GetInstance(
                _injectContext, ValidationUtil.CreateDefaultArgs(ParamTypes.ToArray()));
        }

        protected abstract IEnumerable<Type> ParamTypes
        {
            get;
        }
    }
}
