using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Relays IAP Transaction information to Unity Analytics.
    ///
    /// Responsible for adapting Unity Purchasing's unified
    /// receipts for Unity Analytics' Transaction API.
    /// </summary>
    internal class AnalyticsReporter
    {
        private IUnityAnalytics m_Analytics;

        public AnalyticsReporter(IUnityAnalytics analytics)
        {
            m_Analytics = analytics;
        }

        public void OnPurchaseSucceeded(Product product)
        {
            if (null == product.metadata.isoCurrencyCode)
            {
                return;
            }

            m_Analytics.Transaction(product.definition.storeSpecificId,
                product.metadata.localizedPrice,
                product.metadata.isoCurrencyCode,
                product.receipt,
                null);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            var data = new Dictionary<string, object>() {
                { "productID", product.definition.storeSpecificId },
                { "reason", reason },
                { "price", product.metadata.localizedPrice },
                { "currency", product.metadata.isoCurrencyCode }
            };
            m_Analytics.CustomEvent("unity.PurchaseFailed", data);
        }
    }
}
