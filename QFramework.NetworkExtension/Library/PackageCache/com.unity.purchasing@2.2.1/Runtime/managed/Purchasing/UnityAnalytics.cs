using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Forward transaction information to Unity Analytics.
    /// </summary>
    internal class UnityAnalytics : IUnityAnalytics
    {
        public void Transaction(string productId, decimal price, string currency, string receipt, string signature)
        {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            Analytics.Analytics.Transaction(productId, price, currency, receipt, signature, true);
#endif
        }

        public void CustomEvent(string name, Dictionary<string, object> data)
        {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            Analytics.Analytics.CustomEvent(name, data);
#endif
        }
    }
}
