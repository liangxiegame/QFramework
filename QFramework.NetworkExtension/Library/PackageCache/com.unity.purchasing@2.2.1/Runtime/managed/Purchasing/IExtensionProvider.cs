using System;

namespace UnityEngine.Purchasing
{
    /// <summary>
    /// Interface providing access to various store extensions.
    /// </summary>
    public interface IExtensionProvider
    {
        /// <summary>
        /// Get an implementation of a store extension specified by the template parameter.
        /// </summary>
        /// <typeparam name="T"> Implementation of <c>IStoreExtension</c> </typeparam>
        /// <returns> The store extension requested. </returns>
        T GetExtension<T>() where T : IStoreExtension;
    }
}
