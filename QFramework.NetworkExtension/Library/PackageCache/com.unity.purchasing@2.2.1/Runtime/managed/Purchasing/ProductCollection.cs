using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Provides helper methods to retrieve products by
    /// store independent/store specific id.
    /// </summary>
    public class ProductCollection
    {
        private Dictionary<string, Product> m_IdToProduct;
        private Dictionary<string, Product> m_StoreSpecificIdToProduct;
        private Product[] m_Products;
        private HashSet<Product> m_ProductSet = new HashSet<Product>();

        internal ProductCollection(Product[] products)
        {
            AddProducts(products);
        }

        internal void AddProducts(IEnumerable<Product> products)
        {
            m_ProductSet.UnionWith(products);
            m_Products = m_ProductSet.ToArray();
            m_IdToProduct = m_Products.ToDictionary(x => x.definition.id);
            m_StoreSpecificIdToProduct = m_Products.ToDictionary(x => x.definition.storeSpecificId);
        }

        /// <summary>
        /// The hash set of all products
        /// </summary>
        public HashSet<Product> set
        {
            get { return m_ProductSet; }
        }

        /// <summary>
        /// The array of all products
        /// </summary>
        public Product[] all
        {
            get { return m_Products; }
        }

        /// <summary>
        /// Gets a product matching an id
        /// </summary>
        /// <param name="id"> The id of the desired product </param>
        /// <returns> The product matching the id, or null if not found </returns>
        public Product WithID(string id)
        {
            Product result = null;
            m_IdToProduct.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets a product matching a store-specific id
        /// </summary>
        /// <param name="id"> The store-specific id of the desired product </param>
        /// <returns> The product matching the id, or null if not found </returns>
        public Product WithStoreSpecificID(string id)
        {
            Product result = null;
            if (id != null)
            {
                m_StoreSpecificIdToProduct.TryGetValue(id, out result);
            }
            return result;
        }
    }
}
