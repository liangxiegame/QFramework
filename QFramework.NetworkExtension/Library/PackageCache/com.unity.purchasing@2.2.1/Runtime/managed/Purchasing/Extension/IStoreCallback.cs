using System.Collections.Generic;

namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// Callback interface for <see cref="IStore"/>s.
    /// </summary>
    public interface IStoreCallback
    {
        /// <summary>
        /// For querying product information.
        /// </summary>
        ProductCollection products { get; }

        /// <summary>
        /// Purhasing unavailable.
        /// </summary>
        /// <param name="reason"> The reason the initialization failed. </param>
        void OnSetupFailed(InitializationFailureReason reason);

        /// <summary>
        /// Complete setup by providing a list of available products,
        /// complete with metadata and any associated purchase receipts
        /// and transaction IDs.
        ///
        /// Any previously unseen purchases will be completed by the PurchasingManager.
        /// </summary>
        /// <param name="products"> The list of product descriptions retrieved. </param>
        void OnProductsRetrieved(List<ProductDescription> products);

        /// <summary>
        /// Inform Unity Purchasing of a purchase.
        /// </summary>
        /// <param name="storeSpecificId"> The product id specific to the store it was purchased from. </param>
        /// <param name="receipt"> The receipt provided by the store detailing the purchase </param>
        /// <param name="transactionIdentifier"> The id of the transaction </param>
        void OnPurchaseSucceeded(string storeSpecificId, string receipt, string transactionIdentifier);

        /// <summary>
        /// Inform Unity Purchasing of all active purchases.
        /// </summary>
        /// <param name="purchasedProducts">all active purchased products</param>
        void OnPurchasesRetrieved(List<Product> purchasedProducts);

        /// <summary>
        /// Notify a failed purchase with associated details.
        /// </summary>
        /// <param name="desc"> The object detailing the purchase failure </param>
        void OnPurchaseFailed(PurchaseFailureDescription desc);

        /// <summary>
        /// Stores may opt to disable Unity IAP's transaction log if they offer a robust transaction
        /// system of their own (e.g. Apple).
        ///
        /// The default value is 'true'.
        /// </summary>
        bool useTransactionLog { get; set; }
    }
}
