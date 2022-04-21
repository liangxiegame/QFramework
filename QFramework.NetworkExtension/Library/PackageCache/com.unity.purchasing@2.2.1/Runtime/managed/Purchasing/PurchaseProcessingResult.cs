namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Informs Unity Purchasing as to whether an Application
    /// has finished processing a purchase.
    /// </summary>
    public enum PurchaseProcessingResult
    {
        /// <summary>
        /// The application has finished processing the purchase.
        ///
        /// Unity Purchasing should not inform the application of this
        /// transaction again.
        /// </summary>
        Complete,

        /// <summary>
        /// The application has not finished processing the purchase,
        /// eg it is pushing it to a server asynchronously.
        ///
        /// Unity Purchasing should continue to send the application notifications
        /// about this transaction when it starts.
        /// </summary>
        Pending
    }
}
