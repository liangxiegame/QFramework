using System;
using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Used by Applications to control Unity Purchasing.
    /// </summary>
    public interface IStoreController
    {
        /// <summary>
        /// Gets the collection of products in the store.
        /// </summary>
        /// <value> The product collection. </value>
        ProductCollection products { get; }

        /// <summary>
        /// Initiate a purchase from the controlled store.
        /// </summary>
        /// <param name="product"> The product to be purchased. </param>
        /// <param name="payload"> The developer payload provided for certain stores that define such a concept (ex: Google Play). </param>
        void InitiatePurchase(Product product, string payload);

        /// <summary>
        /// Initiate a purchase from the controlled store.
        /// </summary>
        /// <param name="productId"> The id of the product to be purchased. </param>
        /// <param name="payload"> The developer payload provided for certain stores that define such a concept (ex: Google Play). </param>
        void InitiatePurchase(string productId, string payload);

        /// <summary>
        /// Initiate a purchase from the controlled store.
        /// </summary>
        /// <param name="product"> The product to be purchased. </param>
        void InitiatePurchase(Product product);

        /// <summary>
        /// Initiate a purchase from the controlled store
        /// </summary>
        /// <param name="productId"> The id of the product to be purchased. </param>
        void InitiatePurchase(string productId);

        /// <summary>
        /// Fetch additional products from the controlled store.
        /// </summary>
        /// <param name="products"> The set of product definitions to be fetched. </param>
        /// <param name="successCallback"> The event triggered on a succesful fetch. </param>
        /// <param name="failCallback"> The event triggered on a failed fetch. </param>
        void FetchAdditionalProducts(HashSet<ProductDefinition> products, Action successCallback,
            Action<InitializationFailureReason> failCallback);

        /// <summary>
        /// Where an Application returned ProcessingResult.Pending
        /// from IStoreListener.ProcessPurchase(), Applications should call
        /// this method when processing completes.
        /// </summary>
        /// <param name="product"> The product for which its pending purchase it to be confirmed. </param>
        void ConfirmPendingPurchase(Product product);
    }
}
