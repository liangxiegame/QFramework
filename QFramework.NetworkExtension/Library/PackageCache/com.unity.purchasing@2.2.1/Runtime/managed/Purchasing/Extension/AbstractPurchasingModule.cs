namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// Base class for Purchasing Modules.
    ///
    /// In addition to providing helper methods, use of an abstract
    /// class allows addition of IPurchasingModule methods without
    /// breaking compatibility with existing plugins.
    /// </summary>
    public abstract class AbstractPurchasingModule : IPurchasingModule
    {
        /// <summary>
        /// Object that binds this module with store implementations.
        /// </summary>
        protected IPurchasingBinder m_Binder;

        /// <summary>
        /// Configures the purchasing module.
        /// </summary>
        /// <param name="binder"> The object binding the purchasing with store implementations </param>
        public void Configure(IPurchasingBinder binder)
        {
            this.m_Binder = binder;
            Configure();
        }

        /// <summary>
        /// Registers a store with the purchasing binder.
        /// </summary>
        /// <param name="name"> The store name </param>
        /// <param name="store"> The store's instance </param>
        protected void RegisterStore(string name, IStore store)
        {
            m_Binder.RegisterStore(name, store);
        }

        /// <summary>
        /// Binds the store extension with the purchasing binder.
        /// </summary>
        /// <typeparam name="T"> Implementation of <c>IStoreExtension</c>. </typeparam>
        /// <param name="instance"> Instance of the store extension </param>
        protected void BindExtension<T>(T instance) where T : IStoreExtension
        {
            m_Binder.RegisterExtension(instance);
        }

        /// <summary>
        /// Binds the store configuration with the purchasing binder.
        /// </summary>
        /// <typeparam name="T"> Implementation of <c>IStoreConfiguration</c>. </typeparam>
        /// <param name="instance"> Instance of the store configuration </param>
        protected void BindConfiguration<T>(T instance) where T : IStoreConfiguration
        {
            m_Binder.RegisterConfiguration(instance);
        }

        /// <summary>
        /// Configures the purchasing module with default settings.
        /// </summary>
        public abstract void Configure();
    }
}
