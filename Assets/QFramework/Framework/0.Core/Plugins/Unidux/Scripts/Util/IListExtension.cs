using System.Collections.Generic;

namespace Unidux.Util
{
    public static class IListExtension
    {
        public static void RemoveLast<TValue>(this IList<TValue> list)
        {
            if (list.Count > 0)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}