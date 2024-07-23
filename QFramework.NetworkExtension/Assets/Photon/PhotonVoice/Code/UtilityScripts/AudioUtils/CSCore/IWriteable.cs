namespace CSCore
{
    /// <summary>
    ///     Provides the <see cref="Write" /> method.
    /// </summary>
    public interface IWriteable
    {
        /// <summary>
        ///     Used to write down raw byte data.
        /// </summary>
        /// <param name="buffer">Byte array which contains the data to write down.</param>
        /// <param name="offset">Zero-based offset in the <paramref name="buffer" />.</param>
        /// <param name="count">Number of bytes to write.</param>
        void Write(byte[] buffer, int offset, int count);
    }
}