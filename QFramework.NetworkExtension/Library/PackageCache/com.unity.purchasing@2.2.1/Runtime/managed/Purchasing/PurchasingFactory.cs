using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Purchasing.Extension;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Manages instantiation of specific store services based on provided <c>IPurchasingModule</c>s.
    /// </summary>
    internal class PurchasingFactory : IPurchasingBinder, IExtensionProvider
    {
        private Dictionary<Type, IStoreConfiguration> m_ConfigMap = new Dictionary<Type, IStoreConfiguration>();
        private Dictionary<Type, IStoreExtension> m_ExtensionMap = new Dictionary<Type, IStoreExtension>();
        private IStore m_Store;
		private ICatalogProvider m_CatalogProvider;

        public PurchasingFactory(IPurchasingModule first, params IPurchasingModule[] remainingModules)
        {
            first.Configure(this);
            foreach (var module in remainingModules)
                module.Configure(this);
        }

        public string storeName { get; private set; }

        public IStore service
        {
            get
            {
                if (m_Store != null)
                    return m_Store;
                throw new InvalidOperationException("No impl available!");
            }

            set { m_Store = value; }
        }

        public void RegisterStore(string name, IStore s)
        {
            // We use the first store that supports our current
            // platform.
            if (m_Store == null && s != null)
            {
                storeName = name;
                service = s;
            }
        }

        public void RegisterExtension<T>(T instance) where T : IStoreExtension
        {
            m_ExtensionMap[typeof(T)] = instance;
        }

        public void RegisterConfiguration<T>(T instance) where T : IStoreConfiguration
        {
            m_ConfigMap[typeof(T)] = instance;
        }

        /// <summary>
        /// Get the specified <c>IStoreConfiguration</c>.
        ///
        /// If the store implements the requested interface,
        /// it will be returned.
        /// </summary>
        public T GetConfig<T>() where T : IStoreConfiguration
        {
            if (service is T)
                return (T)service;

            var type = typeof(T);
            if (m_ConfigMap.ContainsKey(type))
                return (T)m_ConfigMap[type];

            throw new ArgumentException("No binding for config type " + type);
        }

        /// Get the specified <c>IStoreExtension</c>.
        ///
        /// If the store implements the requested interface,
        /// it will be returned.
        public T GetExtension<T>() where T : IStoreExtension
        {
            if (service is T)
            {
                return (T)service;
            }

            var t = typeof(T);
            if (m_ExtensionMap.ContainsKey(t))
            {
                return (T)m_ExtensionMap[t];
            }

            throw new ArgumentException("No binding for type " + t);
        }

		public void SetCatalogProvider (ICatalogProvider provider)
		{
			m_CatalogProvider = provider;
		}

		public void SetCatalogProviderFunction(Action<Action<HashSet<ProductDefinition>>> func)
		{
			m_CatalogProvider = new SimpleCatalogProvider (func);
		}

		internal ICatalogProvider GetCatalogProvider ()
		{
			return m_CatalogProvider;
		}
	}
}
