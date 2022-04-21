using System;

namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// Represents a failed purchase as described
    /// by a purchasing service.
    /// </summary>
    public class PurchaseFailureDescription
    {
        /// <summary>
        /// Parametrized Constructor.
        /// </summary>
        /// <param name="productId"> The id of the product. </param>
        /// <param name="reason"> The reason for the purchase failure </param>
        /// <param name="message"> The message containing details about the failed purchase. </param>
        public PurchaseFailureDescription(string productId, PurchaseFailureReason reason, string message)
        {
            this.productId = productId;
            this.reason = reason;
            this.message = message;
        }

        /// <summary>
        /// The store specific product ID.
        /// </summary>
        public string productId { get; private set; }

        /// <summary>
        /// The reason for the failure.
        /// </summary>
        public PurchaseFailureReason reason { get; private set; }

        /// <summary>
        /// The message containing details about the failed purchase.
        /// </summary>
        public String message { get; private set; }
    }
}
