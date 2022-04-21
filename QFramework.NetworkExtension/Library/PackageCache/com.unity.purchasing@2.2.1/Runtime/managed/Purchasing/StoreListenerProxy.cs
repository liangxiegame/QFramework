using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Forwards transaction information to Unity Analytics.
    /// </summary>
    internal class StoreListenerProxy : IInternalStoreListener
    {
        private AnalyticsReporter m_Analytics;
        private IStoreListener m_ForwardTo;
        private IExtensionProvider m_Extensions;

        public StoreListenerProxy(IStoreListener forwardTo, AnalyticsReporter analytics, IExtensionProvider extensions)
        {
            m_ForwardTo = forwardTo;
            m_Analytics = analytics;
            m_Extensions = extensions;
        }

        public void OnInitialized(IStoreController controller)
        {
            m_ForwardTo.OnInitialized(controller, m_Extensions);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            m_ForwardTo.OnInitializeFailed(error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            m_Analytics.OnPurchaseSucceeded(e.purchasedProduct);
            return m_ForwardTo.ProcessPurchase(e);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            m_Analytics.OnPurchaseFailed(i, p);
            m_ForwardTo.OnPurchaseFailed(i, p);
        }
    }
}
