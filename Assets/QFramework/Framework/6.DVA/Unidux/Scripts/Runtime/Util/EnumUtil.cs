using System;
using System.Collections.Generic;

namespace Unidux.Util
{
    public static class EnumUtil
    {
        public static IEnumerable<TEnum> All<TEnum>()
            where TEnum : struct
        {
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
            {
                yield return value;
            }
        }
    }
}