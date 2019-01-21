using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSG.Utils
{
    /// <summary>
    /// General extensions to LINQ.
    /// </summary>
    public static class LinqExts
    {
        /// <summary>
        /// Convert an ordinary object to an array.
        /// </summary>
        public static T[] ArrayWrap<T>(this T input)
        {
            return new T[] { input };
        }

        /// <summary>
        /// Return an empty enumerable.
        /// </summary>
        public static IEnumerable<T> Empty<T>()
        {
            return new T[0];
        }

        /// <summary>
        /// Convert a variable length argument list of items to an enumerable.
        /// </summary>
        public static IEnumerable<T> FromItems<T>(params T[] items)
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Insert 1 or more items onto a LINQ enumreable.
        /// </summary>
        public static IEnumerable<T> InsertItems<T>(this IEnumerable<T> source, params T[] items)
        {
            foreach (var item in items)
            {
                yield return item;
            }

            foreach (var sourceItem in source)
            {
                yield return sourceItem;
            }
        }

        /// <summary>
        /// Concatenate 1 or more items onto a LINQ enumreable.
        /// </summary>
        public static IEnumerable<T> ConcatItems<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Concat(items);
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<T> LazyEach<T>(this IEnumerable<T> source, Action<T> fn)
        {
            foreach (var item in source)
            {
                fn.Invoke(item);

                yield return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable LazyEach<T>(this IEnumerable source, Action<T> fn)
        {
            foreach (T item in source)
            {
                fn.Invoke(item);

                yield return item;
            }
        }

        public static IEnumerable<T> LazyEach<T>(this IEnumerable<T> source, Action<T, int> fn)
        {
            int index = 0;

            foreach (T item in source)
            {
                fn.Invoke(item, index);
                index++;

                yield return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Each<T>(this IEnumerable<T> source, Action<T> fn)
        {
            foreach (var item in source)
            {
                fn.Invoke(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Each<T>(this IEnumerable source, Action<T> fn)
        {
            foreach (T item in source)
            {
                fn.Invoke(item);
            }
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T, int> fn)
        {
            int index = 0;

            foreach (T item in source)
            {
                fn.Invoke(item, index);
                index++;
            }
        }

        /// <summary>
        /// Return the first value or the specified default.
        /// http://stackoverflow.com/questions/12972295/firstordefault-default-value-other-than-null
        /// </summary>
        public static T FirstOr<T>(this IEnumerable<T> source, T defaultValue)
        {
            foreach (T t in source)
                return t;
            return defaultValue;
        }

        /// <summary>
        /// Join an enumerable of strings.
        /// </summary>
        public static string Join(this IEnumerable<string> strs)
        {
            return string.Join("", strs.ToArray());
        }

        /// <summary>
        /// Join an enumerable of strings by the specified separator.
        /// </summary>
        public static string Join(this IEnumerable<string> strs, string separator)
        {
            return string.Join(separator, strs.ToArray());
        }

        /// <summary>
        /// Join an enumerable of strings by the specified separator.
        /// </summary>
        public static string Join(this IEnumerable<string> strs, char separator)
        {
            return string.Join(separator.ToString(), strs.ToArray());
        }

        /// <summary>
        /// Zip two lists into a single list.
        /// </summary>
        public static IEnumerable<D> Zip<T1, T2, D>(this IEnumerable<T1> list1, IEnumerable<T2> list2, Func<T1, T2, D> zipper)
        {
            var enumerator1 = list1.GetEnumerator();
            var enumerator2 = list2.GetEnumerator();

            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                yield return zipper(enumerator1.Current, enumerator2.Current);
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T obj) where T : class
        {
            int i = 0;
            foreach(T item in source)
            {
                if (item == obj)
                {
                    return i;
                }
                ++i;
            }
            return -1;
        }

        /// <summary>
        /// Function to dump a sequence to a logger.
        /// </summary>
        public static IEnumerable<T> Dump<T>(this IEnumerable<T> source, ILogger logger)
        {
            return source.Dump(logger, 0);
        }

        /// <summary>
        /// Function to dump a sequence to a logger.
        /// </summary>
        public static IEnumerable<T> Dump<T>(this IEnumerable<T> source, ILogger logger, int indent)
        {
            var indentStr = new string(' ', indent * 4);
            foreach (var item in source)
            {
                logger.LogInfo(indentStr + item.ToString());
            }

            return source;
        }

        /// <summary>
        /// Performance an action if the source is an empty sequence.
        /// </summary>
        public static IEnumerable<T> WhenEmpty<T>(this IEnumerable<T> source, Action action)
        {
            if (!source.Any())
            {
                action();
            }

            return source;
        }

        /// <summary>
        /// Run an action on each item in source that matches the specified type.
        /// Returns the source list unchanged.
        /// </summary>
        public static IEnumerable<T> LazyForType<T, FilteredT>(this IEnumerable<T> source, Action<FilteredT> action)
        {
            return source.LazyEach(item =>
            {
                if (item is FilteredT)
                {
                    action((FilteredT)(object)item);
                }
            });
        }

        /// <summary>
        /// Run an action on each item in source that matches the specified type.
        /// Returns the source list unchanged.
        /// </summary>
        public static void ForType<T, FilteredT>(this IEnumerable<T> source, Action<FilteredT> action)
        {
            source.Each(item =>
            {
                if (item is FilteredT)
                {
                    action((FilteredT)(object)item);
                }
            });
        }

        /// <summary>
        /// Invokes an action when the source contains no items.
        /// </summary>
        public static IEnumerable<T> WhereNone<T>(this IEnumerable<T> source, Action action)
        {
            if (!source.Any())
            {
                action();
            }

            return source;
        }

        /// <summary>
        /// Invokes an action when the source contains more than 1 item.
        /// </summary>
        public static IEnumerable<T> WhereMultiple<T>(this IEnumerable<T> source, Action action)
        {
            if (source.Skip(1).Any())
            {
                action();
            }

            return source;
        }

        /// <summary>
        /// Invokes an action when the source contains more than 1 item.
        /// </summary>
        public static IEnumerable<T> WhereMultiple<T>(this IEnumerable<T> source, Action<IEnumerable<T>> action)
        {
            if (source.Skip(1).Any())
            {
                action(source);
            }

            return source;
        }
    }
}
