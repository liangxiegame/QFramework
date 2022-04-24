namespace UnityEngine.Purchasing
{
    /// <summary>
    /// A purchase that failed including the product under purchase,
    /// the reason for the failure and any additional information.
    /// </summary>
    public class PurchaseFailedEventArgs
    {
        internal PurchaseFailedEventArgs(Product purchasedProduct, PurchaseFailureReason reason, string message)
        {
            this.purchasedProduct = purchasedProduct;
            this.reason = reason;
            this.message = message;
        }

        /// <summary>
        /// The product which failed to be purchased.
        /// </summary>
        public Product purchasedProduct { get; private set; }

        /// <summary>
        /// The reason for the failure.
        /// </summary>
        public PurchaseFailureReason reason { get; private set; }

        /// <summary>
        /// A message containing details about the failure.
        /// </summary>
        public string message { get; private set; }
    }
}
