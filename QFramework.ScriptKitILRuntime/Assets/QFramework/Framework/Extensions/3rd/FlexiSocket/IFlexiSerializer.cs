using System.IO;

namespace QFramework
{ 
    /// <summary>
    /// Custom serialization callback
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public interface IFlexiSerializer<T>
    {
        /// <summary>
        /// Serialize value to stream
        /// </summary>
        /// <param name="value">Data</param>
        /// <param name="writer">Output stream writer</param>
        void Serialize(T value, BinaryWriter writer);

        /// <summary>
        /// Deserialize value from stream
        /// </summary>
        /// <param name="reader">Input stream reader</param>
        /// <returns>Deserialized value</returns>
        T Deserialize(BinaryReader reader);
    }
}