namespace Unidux.Util
{
    public static class StructUtil
    {
        public static int? ParseInt(this string text)
        {
            int value;

            if (int.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }

        public static uint? ParseUInt(this string text)
        {
            uint value;

            if (uint.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }

        public static long? ParseLong(this string text)
        {
            long value;

            if (long.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }

        public static ulong? ParseULong(this string text)
        {
            ulong value;

            if (ulong.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }
        
        public static byte? ParseByte(this string text)
        {
            byte value;

            if (byte.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }
        
        public static float? ParseFloat(this string text)
        {
            float value;

            if (float.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }
        
        public static double? ParseDouble(this string text)
        {
            double value;

            if (double.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }
        
        public static bool? ParseBool(this string text)
        {
            bool value;

            if (bool.TryParse(text, out value))
            {
                return value;
            }

            return null;
        }
    }
}