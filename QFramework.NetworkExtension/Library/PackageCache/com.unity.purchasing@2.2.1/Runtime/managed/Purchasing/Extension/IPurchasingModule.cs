namespace UnityEngine.Purchasing.Extension
{
    /// <summary>
    /// Configures Unity Purchasing for one or more
    /// stores.
    ///
    /// Store implementations must provide an
    /// implementation of this interface.
    /// </summary>
    public interface IPurchasingModule
    {
        /// <summary>
        /// Configures the purchasing module.
        /// </summary>
        /// <param name="binder"> The object binding the purchasing with store implementations </param>
        void Configure(IPurchasingBinder binder);
    }
}
