using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Purchasing.Extension;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Maps store specific Product identifiers to one
    /// or more store identifiers.
    ///
    /// The name is deliberately terse for use as a collection initializer.
    /// </summary>
    public class IDs : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> m_Dic = new Dictionary<string, string>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns> An IEnumerator object that can be used to iterate through the collection. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Dic.GetEnumerator();
        }

        /// <summary>
        /// Add a product identifier to a list of store names with string.
        /// </summary>
        /// <param name="id"> Product identifier. </param>
        /// <param name="stores"> List of stores by string, to which we the id will be mapped to. </param>
        public void Add(string id, params string[] stores)
        {
            foreach (var store in stores)
                m_Dic[store] = id;
        }

        /// <summary>
        /// Add a product identifier to a list of store names with non strings such as Enums.
        /// </summary>
        /// <param name="id"> Product identifier. </param>
        /// <param name="stores"> List of stores by other object, to which we the id will be mapped to. </param>
        public void Add(string id, params object[] stores)
        {
            foreach (var store in stores)
                m_Dic[store.ToString()] = id;
        }

        internal string SpecificIDForStore(string store, string defaultValue)
        {
            if (m_Dic.ContainsKey(store))
                return m_Dic[store];
            return defaultValue;
        }

        /// <summary>
        /// Retrieve an Enumerator with which can be used to iterate through the internal map structure.
        /// </summary>
        /// <returns> Enumerator as a Key/Value pair. </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return m_Dic.GetEnumerator();
        }
    }

    /// <summary>
    /// Builds configuration for Unity Purchasing,
    /// consisting of products and store specific configuration details.
    /// </summary>
    public class ConfigurationBuilder
    {
        private PurchasingFactory m_Factory;
        private HashSet<ProductDefinition> m_Products = new HashSet<ProductDefinition>();

        internal ConfigurationBuilder(PurchasingFactory factory)
        {
            m_Factory = factory;
        }

        /// <summary>
        /// Obsolete. Please use useCatalogProvider instead.
        /// </summary>
        /// <value> True it the project will use the catalog stored on the cloud. </value>
		[Obsolete("This property has been renamed 'useCatalogProvider'", false)]
        public bool useCloudCatalog
        {
            get;
            set;
        }

        /// <summary>
        /// Whether or not the project will use the catalog stored on the cloud or the one cached locally.
        /// </summary>
        /// <value> True if the project will use the catalog stored on the cloud. </value>
		public bool useCatalogProvider
		{
			get;
			set;
		}

        /// <summary>
        /// The set of products in the catalog.
        /// </summary>
        public HashSet<ProductDefinition> products
        {
            get { return m_Products; }
        }

        internal PurchasingFactory factory
        {
            get { return m_Factory; }
        }

        /// <summary>
        /// Configure the store as specified by the template parameter.
        /// </summary>
        /// <typeparam name="T"> Implementation of <c>IStoreConfiguration</c> </typeparam>
        /// <returns> The store configuration as an object. </returns>
        public T Configure<T>() where T : IStoreConfiguration
        {
            return m_Factory.GetConfig<T>();
        }

        /// <summary>
        /// Create an instance of the configuration builder.
        /// </summary>
        /// <param name="first"> The first purchasing module. </param>
        /// <param name="rest"> The remaining purchasing modules, excluding the one passes as first. </param>
        /// <returns> The instance of the configuration builder as specified. </returns>
        public static ConfigurationBuilder Instance(IPurchasingModule first, params IPurchasingModule[] rest)
        {
            PurchasingFactory factory = new PurchasingFactory(first, rest);
            return new ConfigurationBuilder(factory);
        }

        /// <summary>
        /// Add a product to the configuration builder.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="type"> The type of the product. </param>
        /// <returns> The instance of the configuration builder with the new product added. </returns>
        public ConfigurationBuilder AddProduct(string id, ProductType type)
        {
            return AddProduct(id, type, null);
        }

        /// <summary>
        /// Add a product to the configuration builder.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="type"> The type of the product. </param>
        /// <param name="storeIDs"> The object representing store IDs the product is to be added to. </param>
        /// <returns> The instance of the configuration builder with the new product added. </returns>
        public ConfigurationBuilder AddProduct(string id, ProductType type, IDs storeIDs)
        {
            return AddProduct(id, type, storeIDs, (IEnumerable<PayoutDefinition>)null);
        }

        /// <summary>
        /// Add a product to the configuration builder.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="type"> The type of the product. </param>
        /// <param name="storeIDs"> The object representing store IDs the product is to be added to. </param>
        /// <param name="payout"> The payout definition of the product. </param>
        /// <returns> The instance of the configuration builder with the new product added. </returns>
        public ConfigurationBuilder AddProduct(string id, ProductType type, IDs storeIDs, PayoutDefinition payout)
        {
            return AddProduct(id, type, storeIDs, new List<PayoutDefinition> { payout });
        }

        /// <summary>
        /// Add a product to the configuration builder.
        /// </summary>
        /// <param name="id"> The id of the product. </param>
        /// <param name="type"> The type of the product. </param>
        /// <param name="storeIDs"> The object representing store IDs the product is to be added to. </param>
        /// <param name="payouts"> The enumerator of the payout definitions of the product. </param>
        /// <returns> The instance of the configuration builder with the new product added. </returns>
        public ConfigurationBuilder AddProduct(string id, ProductType type, IDs storeIDs, IEnumerable<PayoutDefinition> payouts)
        {
            var specificId = id;
            // Extract our store specific ID if present, according to the current store.
            if (storeIDs != null)
                specificId = storeIDs.SpecificIDForStore(factory.storeName, id);
            var product = new ProductDefinition(id, specificId, type);
            product.SetPayouts(payouts);
            m_Products.Add(product);

            return this;
        }


        /// <summary>
        /// Add multiple products to the configuration builder.
        /// </summary>
        /// <param name="products"> The enumerator of the product definitions to be added. </param>
        /// <returns> The instance of the configuration builder with the new product added. </returns>
        public ConfigurationBuilder AddProducts(IEnumerable<ProductDefinition> products)
        {
            foreach (var product in products) {
                m_Products.Add(product);
            }

            return this;
        }
    }
}
