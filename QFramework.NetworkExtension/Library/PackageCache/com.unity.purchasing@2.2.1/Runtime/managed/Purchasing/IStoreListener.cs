namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Implemented by Application developers using Unity Purchasing.
    /// </summary>
    public interface IStoreListener
    {
        /// <summary>
        /// Purchasing failed to initialise for a non recoverable reason.
        /// </summary>
        /// <param name="error"> The failure reason. </param>
        void OnInitializeFailed(InitializationFailureReason error);

        /// <summary>
        /// A purchase succeeded.
        /// </summary>
        /// <param name="purchaseEvent"> The <c>PurchaseEventArgs</c> for the purchase event. </param>
        /// <returns> The result of the succesful purchase </returns>
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent);

        /// <summary>
        /// A purchase failed with specified reason.
        /// </summary>
        /// <param name="product"> The product that was attempted to be purchased. </param>
        /// <param name="failureReason"> The failure reason. </param>
        void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason);

        /// <summary>
        /// Purchasing initialized successfully.
        ///
        /// The <c>IStoreController</c> and <c>IExtensionProvider</c> are
        /// available for accessing purchasing functionality.
        /// </summary>
        /// <param name="controller"> The <c>IStoreController</c> created during initilaization. </param>
        /// <param name="extensions"> The <c>IExtensionProvider</c> created during initilaization. </param>
        void OnInitialized(IStoreController controller, IExtensionProvider extensions);
    }
}
