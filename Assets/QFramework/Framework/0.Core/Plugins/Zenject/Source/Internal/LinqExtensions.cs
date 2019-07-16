using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using ModestTree.Util;

namespace ModestTree
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        // Return the first item when the list is of length one and otherwise returns default
        public static TSource OnlyOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            Assert.IsNotNull(source);

            if (source.Count() > 1)
            {
                return default(TSource);
            }

            return source.FirstOrDefault();
        }

        // These are more efficient than Count() in cases where the size of the collection is not known
        public static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount).Count() == amount;
        }

        public static bool HasMoreThan<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.HasAtLeast(amount+1);
        }

        public static bool HasLessThan<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.HasAtMost(amount-1);
        }

        public static bool HasAtMost<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount + 1).Count() <= amount;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T item)
        {
            return list.Except(item.Yield());
        }

        // LINQ already has a method called "Contains" that does the same thing as this
        // BUT it fails to work with Mono 3.5 in some cases.
        // For example the following prints False, True in Mono 3.5 instead of True, True like it should:
        //
        // IEnumerable<string> args = new string[]
        // {
        //     "",
        //     null,
        // };

        // Log.Info(args.ContainsItem(null));
        // Log.Info(args.Where(x => x == null).Any());
        public static bool ContainsItem<T>(this IEnumerable<T> list, T value)
        {
            // Use object.Equals to support null values
            return list.Where(x => object.Equals(x, value)).Any();
        }
    }
}
