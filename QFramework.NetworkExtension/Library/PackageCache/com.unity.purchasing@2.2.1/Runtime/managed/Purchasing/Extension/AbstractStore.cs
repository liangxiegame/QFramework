using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// Extension point for purchasing plugins.
    ///
    /// An abstract class is provided so that methods can be added to the IStore
    /// without breaking binary compatibility with existing plugins.
    /// </summary>
    public abstract class AbstractStore : IStore
    {
        /// <summary>
        /// Terminal - no callback required
        /// </summary>
        /// <param name="callback"></param>
        public abstract void Initialize(IStoreCallback callback);

        /// <summary>
        /// Returns results through IStoreCallback previously passed into Initialize
        /// </summary>
        /// <param name="products"></param>
        public abstract void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products);

        /// <summary>
        /// Returns results through IStoreCallback previously passed into Initialize
        /// </summary>
        /// <param name="product"></param>
        /// <param name="developerPayload"></param>
        public abstract void Purchase(ProductDefinition product, string developerPayload);

        /// <summary>
        /// Returns results through IStoreCallback previously passed into Initialize
        /// </summary>
        /// <param name="product"></param>
        /// <param name="transactionId"></param>
        public abstract void FinishTransaction(ProductDefinition product, string transactionId);
    }
}
