using ModestTree;

namespace Zenject.Internal
{
    [NoReflectionBaking]
    public class LookupId
    {
        public IProvider Provider;
        public BindingId BindingId;

        public LookupId()
        {
        }

        public LookupId(IProvider provider, BindingId bindingId)
        {
            Assert.IsNotNull(provider);
            Assert.IsNotNull(bindingId);

            Provider = provider;
            BindingId = bindingId;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Provider.GetHashCode();
            hash = hash * 23 + BindingId.GetHashCode();
            return hash;
        }
    }
}
