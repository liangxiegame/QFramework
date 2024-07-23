namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// A common format for store subsystems to use to
    /// describe available In App Purchases to UnityPurchasing,
    /// including purchase state via Receipt and Transaction
    /// Identifiers.
    /// </summary>
    public class ProductDescription
    {

        /// <summary>
        /// Parametrized Constructor.
        /// With transaction data.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="metadata"> The metadata of the product. </param>
        /// <param name="receipt"> The receipt of the purchase of the product. </param>
        /// <param name="transactionId"> The transaction id of the purchase of the product. </param>
        public ProductDescription(string id, ProductMetadata metadata,
                                  string receipt, string transactionId)
        {
            storeSpecificId = id;
            this.metadata = metadata;
            this.receipt = receipt;
            this.transactionId = transactionId;
        }


        /// <summary>
        /// Parametrized Constructor.
        /// With the transaction data and type.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="metadata"> The metadata of the product. </param>
        /// <param name="receipt"> The receipt of the purchase of the product. </param>
        /// <param name="transactionId"> The transaction id of the purchase of the product. </param>
        /// <param name="type"> The type of the product. </param>
        public ProductDescription(string id, ProductMetadata metadata,
                                  string receipt, string transactionId, ProductType type)
            : this(id, metadata, receipt, transactionId)
        {
            this.type = type;
        }

        /// <summary>
        /// Parametrized Constructor.
        /// Without transaction data.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="metadata"> The metadata of the product. </param>
        public ProductDescription(string id, ProductMetadata metadata) : this(id, metadata, null, null)
        {
        }

        /// <summary>
        /// The store-specific id of this product.
        /// </summary>
        public string storeSpecificId { get; private set; }

        /// <summary>
        /// The type of the product, with respect to the store.
        ///
        /// If this ProductDescription was explicitly queried by Unity IAP
        /// then it is not necessary to specify a type since it is already
        /// known from the product definition.
        ///
        /// Otherwise, if this ProductDescription is unknown, type must
        /// be correctly so the product can be handled correctly.
        /// </summary>
        public ProductType type;

        /// <summary>
        /// The Metadate of the product. Contains store interface information.
        /// </summary>
        public ProductMetadata metadata { get; private set; }

        /// <summary>
        /// The receipt provided on product purchase.
        /// </summary>
        public string receipt { get; private set; }

        /// <summary>
        /// The transaction id of the purchase of this product.
        /// </summary>
        public string transactionId { get; set; }
    }
}
