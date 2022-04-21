using System;

namespace CSCore
{
    /// <summary>
    ///     Provides a few basic extensions.
    /// </summary>
    public static class Extensions
    {
// ReSharper disable once InconsistentNaming
        internal static bool IsPCM(this WaveFormat waveFormat)
        {
            if (waveFormat == null)
                throw new ArgumentNullException("waveFormat");
            if (waveFormat is WaveFormatExtensible)
                return ((WaveFormatExtensible) waveFormat).SubFormat == AudioSubTypes.Pcm;
            return waveFormat.WaveFormatTag == AudioEncoding.Pcm;
        }

        internal static bool IsIeeeFloat(this WaveFormat waveFormat)
        {
            if (waveFormat == null)
                throw new ArgumentNullException("waveFormat");
            if (waveFormat is WaveFormatExtensible)
                return ((WaveFormatExtensible) waveFormat).SubFormat == AudioSubTypes.IeeeFloat;
            return waveFormat.WaveFormatTag == AudioEncoding.IeeeFloat;
        }
    }
}

