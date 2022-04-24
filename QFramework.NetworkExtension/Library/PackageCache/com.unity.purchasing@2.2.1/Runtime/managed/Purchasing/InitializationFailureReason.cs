namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Reasons for which purchasing initialization could fail.
    /// </summary>
    public enum InitializationFailureReason
    {
        /// <summary>
        /// In App Purchases disabled in device settings.
        /// </summary>
        PurchasingUnavailable,

        /// <summary>
        /// No products available for purchase,
        /// typically indicating a configuration error.
        /// </summary>
        NoProductsAvailable,

        /// <summary>
        /// The store reported the app as unknown.
        /// Typically indicates the app has not been created
        /// on the relevant developer portal, or the wrong
        /// identifier has been configured.
        /// </summary>
        AppNotKnown
    }
}
