using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing.Extension;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// The main controller for Applications using Unity Purchasing.
    /// </summary>
    internal class PurchasingManager : IStoreCallback, IStoreController
    {
        private IStore m_Store;
        private IInternalStoreListener m_Listener;
        private ILogger m_Logger;
        private TransactionLog m_TransactionLog;
        private string m_StoreName;
        private Action m_AdditionalProductsCallback;
        private Action<InitializationFailureReason> m_AdditionalProductsFailCallback;

        /// <summary>
        /// Stores may opt to disable Unity IAP's transaction log.
        /// </summary>
        public bool useTransactionLog { get; set; }

        internal PurchasingManager(TransactionLog tDb, ILogger logger, IStore store, string storeName)
        {
            m_TransactionLog = tDb;
            m_Store = store;
            m_Logger = logger;
            m_StoreName = storeName;
            useTransactionLog = true;
        }

        public void InitiatePurchase(Product product)
        {
            InitiatePurchase(product, string.Empty);
        }

        public void InitiatePurchase(string productId)
        {
            InitiatePurchase(productId, string.Empty);
        }

        public void InitiatePurchase(Product product, string developerPayload)
        {
            if (null == product)
            {
                m_Logger.Log("Trying to purchase null Product");
                return;
            }

            if (!product.availableToPurchase)
            {
                m_Listener.OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
                return;
            }

            m_Store.Purchase(product.definition, developerPayload);
            m_Logger.Log("purchase({0})", product.definition.id);
        }

        public void InitiatePurchase(string purchasableId, string developerPayload)
        {
            Product product = products.WithID(purchasableId);
            if (null == product)
                m_Logger.LogWarning("Unable to purchase unknown product with id: {0}", purchasableId);
            InitiatePurchase(product, developerPayload);
        }

        /// <summary>
        /// Where an Application returned ProcessingResult.Pending they can manually
        /// finish the transaction by calling this method.
        /// </summary>
        public void ConfirmPendingPurchase(Product product)
        {
            if (null == product)
            {
                m_Logger.Log("Unable to confirm purchase with null Product");
                return;
            }

            if (string.IsNullOrEmpty(product.transactionID))
            {
                m_Logger.Log("Unable to confirm purchase; Product has missing or empty transactionID");
                return;
            }

            if (useTransactionLog)
                m_TransactionLog.Record(product.transactionID);
            m_Store.FinishTransaction(product.definition, product.transactionID);
        }

        public ProductCollection products { get; private set; }

        /// <summary>
        /// Called by our IStore when a purchase succeeds.
        /// </summary>
        public void OnPurchaseSucceeded(string id, string receipt, string transactionId)
        {
            var product = products.WithStoreSpecificID(id);
            if (null == product)
            {
                // If is possible for stores to tell us about products we have not yet
                // requested details of.
                // We should still tell the App in this scenario, albeit with incomplete information.
                var definition = new ProductDefinition(id, ProductType.NonConsumable);
                product = new Product(definition, new ProductMetadata());
            }
            UpdateProductReceipt(product, receipt, transactionId);
            ProcessPurchaseIfNew(product);
        }

        void UpdateProductReceipt(Product product, string receipt, string transactionId)
        {
            var unifiedReceipt = FormatUnifiedReceipt(receipt, transactionId);
            if (product != null)
            {
                product.receipt = unifiedReceipt;
                product.transactionID = transactionId;
            }
        }

        public void OnPurchasesRetrieved(List<Product> purchasedProducts)
        {
            foreach (var product in products.all)
            {
                var purchasedProduct = purchasedProducts.FirstOrDefault(firstPurchasedProduct => firstPurchasedProduct.definition.id == product.definition.id);
                if (purchasedProduct != null)
                {
                    HandlePurchaseRetrieved(product, purchasedProduct);
                }
                else
                {
                    ClearProductReceipt(product);
                }
            }
        }

        void HandlePurchaseRetrieved(Product product, Product purchasedProduct)
        {
            UpdateProductReceipt(product, purchasedProduct.receipt, purchasedProduct.transactionID);
            ProcessPurchaseIfNew(product);
        }

        static void ClearProductReceipt(Product product)
        {
            product.receipt = null;
            product.transactionID = null;
        }

        public void OnSetupFailed(InitializationFailureReason reason)
        {
            if (initialized)
            {
                if (null != m_AdditionalProductsFailCallback)
                    m_AdditionalProductsFailCallback(reason);
            }
            else
                m_Listener.OnInitializeFailed(reason);
        }

        public void OnPurchaseFailed(PurchaseFailureDescription description)
        {
            if (description != null)
            {
                var product = products.WithStoreSpecificID(description.productId);
                if (null == product)
                {
                    m_Logger.LogError("Failed to purchase unknown product {0}", "productId:" + description.productId + " reason:" + description.reason + " message:" + description.message);
                    return;
                }

                m_Logger.Log("onPurchaseFailedEvent({0})", "productId:" + product.definition.id + " message:" + description.message);
                m_Listener.OnPurchaseFailed(product, description.reason);
            }
        }

        /// <summary>
        /// Called back by our IStore when it has fetched the latest product data.
        /// </summary>
        public void OnProductsRetrieved(List<ProductDescription> products)
        {
            var unknownProducts = new HashSet<Product>();
            foreach (var product in products)
            {
                var matchedProduct = this.products.WithStoreSpecificID(product.storeSpecificId);
                if (null == matchedProduct)
                {
                    var definition = new ProductDefinition(product.storeSpecificId,
                            product.storeSpecificId, product.type);
                    matchedProduct = new Product(definition, product.metadata);
                    unknownProducts.Add(matchedProduct);
                }
                matchedProduct.availableToPurchase = true;
                matchedProduct.metadata = product.metadata;
                matchedProduct.transactionID = product.transactionId;

                if (!string.IsNullOrEmpty(product.receipt))
                {
                    matchedProduct.receipt = FormatUnifiedReceipt(product.receipt, product.transactionId);
                }
            }

            if (unknownProducts.Count > 0)
            {
                this.products.AddProducts(unknownProducts);
            }

            // Fire our initialisation events if this is a first poll.
            CheckForInitialization();

            // Notify the application of any purchases it
            // has not yet processed.
            // Note that this must be done *after* initialisation, above.
            foreach (var product in this.products.set)
            {
                if (!string.IsNullOrEmpty(product.receipt) && !string.IsNullOrEmpty(product.transactionID))
                    ProcessPurchaseIfNew(product);
            }
        }

        public void FetchAdditionalProducts(HashSet<ProductDefinition> products, Action successCallback,
            Action<InitializationFailureReason> failCallback)
        {
            m_AdditionalProductsCallback = successCallback;
            m_AdditionalProductsFailCallback = failCallback;
            this.products.AddProducts(products.Select(x => new Product(x, new ProductMetadata())));
            m_Store.RetrieveProducts(new ReadOnlyCollection<ProductDefinition>(products.ToList()));
        }

        /// <summary>
        /// Checks the product's transaction ID for uniqueness
        /// against the transaction log and calls the Application's
        /// ProcessPurchase method if so.
        /// </summary>
        private void ProcessPurchaseIfNew(Product product)
        {
            if (useTransactionLog && m_TransactionLog.HasRecordOf(product.transactionID))
            {
                m_Logger.Log("Already recorded transaction " + product.transactionID);
                m_Store.FinishTransaction(product.definition, product.transactionID);
                return;
            }

            var p = new PurchaseEventArgs(product);
            // Applications may elect to delay confirmations of purchases,
            // such as when persisting purchase state asynchronously.
            if (PurchaseProcessingResult.Complete == m_Listener.ProcessPurchase(p))
                ConfirmPendingPurchase(product);
        }

        private bool initialized;
        private void CheckForInitialization()
        {
            if (!initialized)
            {
                // Determine if any products are purchasable.
                var available = false;
                foreach (var product in this.products.set)
                {
                    if (!product.availableToPurchase)
                        m_Logger.LogFormat(LogType.Warning, "Unavailable product {0} -{1}", product.definition.id, product.definition.storeSpecificId);
                    else
                        available = true;
                }

                // One or more products is available.
                if (available)
                    m_Listener.OnInitialized(this);
                else
                    OnSetupFailed(InitializationFailureReason.NoProductsAvailable);
                initialized = true;
            }
            else
            {
                if (null != m_AdditionalProductsCallback)
                    m_AdditionalProductsCallback();
            }
        }

        public void Initialize(IInternalStoreListener listener, HashSet<ProductDefinition> products)
        {
            m_Listener = listener;
            m_Store.Initialize(this);

            var prods = products.Select(x => new Product(x, new ProductMetadata())).ToArray();
            this.products = new ProductCollection(prods);

            var productCollection = new ReadOnlyCollection<ProductDefinition>(products.ToList());

            // Start the initialisation process by fetching product metadata.
            m_Store.RetrieveProducts(productCollection);
        }

#if UNITY_5_4_OR_NEWER
        [Serializable]
        class UnifiedReceipt
        {
            public string Store, TransactionID, Payload;
        }
#endif

        private string FormatUnifiedReceipt(string platformReceipt, string transactionId)
        {
#if UNITY_5_4_OR_NEWER

            UnifiedReceipt ur = new UnifiedReceipt()
            {
                Store = this.m_StoreName,
                TransactionID = transactionId,
                Payload = platformReceipt
            };
            return JsonUtility.ToJson(ur);
# else
            var dic = new Dictionary<string, object>();
            dic["Store"] = this.m_StoreName;
            dic["TransactionID"] = transactionId ?? string.Empty;
            dic["Payload"] = platformReceipt ?? string.Empty;

            return SimpleJson.SimpleJson.SerializeObject(dic);
#endif
        }
    }
}
