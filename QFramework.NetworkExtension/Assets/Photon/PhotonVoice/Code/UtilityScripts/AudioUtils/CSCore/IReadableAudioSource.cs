namespace CSCore
{
    /// <summary>
    ///     Defines a generic base for all readable audio streams.
    /// </summary>
    /// <typeparam name="T">The type of the provided audio data.</typeparam>
    public interface IReadableAudioSource<in T> : IAudioSource
    {
        /// <summary>
        ///     Reads a sequence of elements from the <see cref="IReadableAudioSource{T}" /> and advances the position within the
        ///     stream by the
        ///     number of elements read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of elements. When this method returns, the <paramref name="buffer" /> contains the specified
        ///     array of elements with the values between <paramref name="offset" /> and (<paramref name="offset" /> +
        ///     <paramref name="count" /> - 1) replaced by the elements read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based offset in the <paramref name="buffer" /> at which to begin storing the data
        ///     read from the current stream.
        /// </param>
        /// <param name="count">The maximum number of elements to read from the current source.</param>
        /// <returns>The total number of elements read into the buffer.</returns>
        int Read(T[] buffer, int offset, int count);
    }
}