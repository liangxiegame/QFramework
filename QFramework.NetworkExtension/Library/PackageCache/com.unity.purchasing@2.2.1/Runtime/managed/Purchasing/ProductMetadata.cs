namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Metadata for the product, namely that which is relevant to the store subsystem
    /// </summary>
    public class ProductMetadata
    {
        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="priceString"> The price, as a string. </param>
        /// <param name="title"> The title of the product. </param>
        /// <param name="description"> The description of the product. </param>
        /// <param name="currencyCode"> The currency code of the localized price. </param>
        /// <param name="localizedPrice"> The localized price, by currency. </param>
        public ProductMetadata(string priceString, string title, string description, string currencyCode, decimal localizedPrice)
        {
            localizedPriceString = priceString;
            localizedTitle = title;
            localizedDescription = description;
            isoCurrencyCode = currencyCode;
            this.localizedPrice = localizedPrice;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductMetadata()
        {
        }

        /// <summary>
        /// Gets the localized price.
        /// This is the price formatted with currency symbol.
        /// </summary>
        /// <value>The localized price string.</value>
        public string localizedPriceString { get; internal set; }

        /// <summary>
        /// Gets the localized title, as retrieved from the store subsystem;
        /// Apple, Google etc.
        /// </summary>
        public string localizedTitle { get; internal set; }

        /// <summary>
        /// Gets the localized description, as retrieved from the store subsystem;
        /// Apple, Google etc.
        /// </summary>
        public string localizedDescription { get; internal set; }

        /// <summary>
        /// The product's currency in ISO 4217 format eg GBP, USD etc.
        /// </summary>
        public string isoCurrencyCode { get; internal set; }

        /// <summary>
        /// The product's price, denominated in the currency
        /// indicated by <c>isoCurrencySymbol</c>.
        /// </summary>
        public decimal localizedPrice { get; internal set; }
    }
}
