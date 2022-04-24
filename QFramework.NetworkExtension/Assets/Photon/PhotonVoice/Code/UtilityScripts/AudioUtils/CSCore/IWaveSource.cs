namespace CSCore
{
    /// <summary>
    ///     Defines the base for all audio streams which provide raw byte data.
    /// </summary>
    /// <remarks>
    ///     Compared to the <see cref="ISampleSource" />, the <see cref="IWaveSource" /> provides raw bytes instead of samples.
    ///     That means that the <see cref="IAudioSource.Position" /> and the <see cref="IAudioSource.Position" /> properties are
    ///     expressed in bytes.
    ///     Also the <see cref="IReadableAudioSource{T}.Read" /> method provides samples instead of raw bytes.
    /// </remarks>
    public interface IWaveSource : IReadableAudioSource<byte>
    {
        /*/// <summary>
        ///     Reads a sequence of bytes from the <see cref="IWaveSource" /> and advances the position within the stream by the
        ///     number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the <paramref name="buffer" /> contains the specified
        ///     byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> +
        ///     <paramref name="count" /> - 1) replaced by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in the <paramref name="buffer" /> at which to begin storing the data
        ///     read from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to read from the current source.</param>
        /// <returns>The total number of bytes read into the buffer.</returns>
        int Read(byte[] buffer, int offset, int count);*/
    }
}