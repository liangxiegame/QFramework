using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// Represents the public interface of the underlying store system such as Google Play,
    /// or the Apple App store.
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// Initialize the instance using the specified <see cref="!:IStoreCallback" />.
        /// </summary>
        /// <param name="callback"> The callback interface for the store. </param>
        void Initialize(IStoreCallback callback);

        /// <summary>
        /// Fetch the latest product metadata, including purchase receipts,
        /// asynchronously with results returned via <c>IStoreCallback</c>.
        /// </summary>
        /// <param name="products"> The collection of products desired </param>
        void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products);

        /// <summary>
        /// Handle a purchase request from a user.
        /// Developer payload is provided for stores
        /// that define such a concept (Google Play).
        /// </summary>
        /// <param name="product"> The product to be purchased. </param>
        /// <param name="developerPayload"> The developer payload expected by the specific store implementation </param>
        void Purchase(ProductDefinition product, string developerPayload);

        /// <summary>
        /// Called by Unity Purchasing when a transaction has been recorded.
        /// Store systems should perform any housekeeping here,
        /// such as closing transactions or consuming consumables.
        /// </summary>
        /// <param name="product"> The product purchased </param>
        /// <param name="transactionId"> The id of the transaction </param>
        void FinishTransaction(ProductDefinition product, string transactionId);
    }
}
