using System;
using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Product definition used by Apps declaring products for sale.
    /// </summary>
    public class ProductDefinition
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        private ProductDefinition()
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="id"> The product id. </param>
        /// <param name="storeSpecificId"> The product's id for a specific store. </param>
        /// <param name="type"> The product type. </param>
        public ProductDefinition(string id, string storeSpecificId, ProductType type) : this(id, storeSpecificId, type, true)
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="id"> The product id. </param>
        /// <param name="storeSpecificId"> The product's id for a specific store. </param>
        /// <param name="type"> The product type. </param>
        /// <param name="enabled"> Whether the product is enabled for purchase or not. </param>
        public ProductDefinition(string id, string storeSpecificId, ProductType type, bool enabled) : this(id, storeSpecificId, type, enabled, (IEnumerable<PayoutDefinition>)null)
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="id"> The product id. </param>
        /// <param name="storeSpecificId"> The product's id for a specific store. </param>
        /// <param name="type"> The product type. </param>
        /// <param name="enabled"> Whether the product is enabled for purchase or not. </param>
        /// <param name="payout"> The payout definition for the product once purchased. </param>
        public ProductDefinition(string id, string storeSpecificId, ProductType type, bool enabled, PayoutDefinition payout) : this(id, storeSpecificId, type, enabled, new List<PayoutDefinition> { payout })
        {
        }

        /// <summary>
        /// Parametrized constructor
        /// </summary>
        /// <param name="id"> The product id. </param>
        /// <param name="storeSpecificId"> The product's id for a specific store. </param>
        /// <param name="type"> The product type. </param>
        /// <param name="enabled"> Whether the product is enabled for purchase or not. </param>
        /// <param name="payouts"> The payout definitions for the product once purchased. </param>
        public ProductDefinition(string id, string storeSpecificId, ProductType type, bool enabled, IEnumerable<PayoutDefinition> payouts)
        {
            this.id = id;
            this.storeSpecificId = storeSpecificId;
            this.type = type;
            this.enabled = enabled;
            SetPayouts(payouts);
        }

        /// <summary>
        /// Parametrized constructor, creating a ProductDefinition where the id is the same as the store specific ID.
        /// </summary>
        /// <param name="id"> The product id as well as its store-specific id. </param>
        /// <param name="type"> The product type. </param>
        public ProductDefinition(string id, ProductType type) : this(id, id, type)
        {
        }

        /// <summary>
        /// Store independent ID.
        /// </summary>
        public string id { get; private set; }

        /// <summary>
        /// The ID this product has on a specific store.
        /// </summary>
        public string storeSpecificId { get; private set; }

        /// <summary>
        /// The type of the product.
        /// </summary>
        public ProductType type { get; private set; }

        /// <summary>
        /// Whether or not the product is enabled for purchase.
        /// </summary>
        public bool enabled { get; private set; }

        /// <summary>
        /// Check if this product definition is equal to another.
        /// </summary>
        /// <param name="obj"> The product definition to compare with this object. </param>
        /// <returns> True if the definitions are equal </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ProductDefinition p = obj as ProductDefinition;
            if (p == null)
                return false;

            return (id == p.id);
        }

        /// <summary>
        /// Get the unique Hash representing the product definition.
        /// </summary>
        /// <returns> The hash code as integer </returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        private List<PayoutDefinition> m_Payouts = new List<PayoutDefinition>();

        /// <summary>
        /// Gets all payouts attached to this product.
        /// </summary>
        /// <value>The payouts.</value>
        public IEnumerable<PayoutDefinition> payouts {
            get {
                return m_Payouts;
            }
        }

        /// <summary>
        /// Gets the first attached payout. This is a shortcut for the case where only one payout is attached to the product.
        /// </summary>
        /// <value>The payout.</value>
        public PayoutDefinition payout {
            get {
                return m_Payouts.Count > 0 ? m_Payouts[0] : null;
            }
        }

        /// <summary>
        /// Update this product's payouts
        /// </summary>
        /// <param name="newPayouts">A set of payouts to replace the current payouts on this product definition</param>
        internal void SetPayouts(IEnumerable<PayoutDefinition> newPayouts)
        {
            if (newPayouts == null)
                return;

            m_Payouts.Clear();
            m_Payouts.AddRange(newPayouts);
        }
    }
}
