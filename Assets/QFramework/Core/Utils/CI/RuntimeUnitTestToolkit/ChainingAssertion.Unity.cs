// ChainingAssertion for Unity
// https://github.com/neuecc/ChainingAssertion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeUnitTestToolkit
{
    public static partial class AssertExtensions
    {
        /// <summary>Assert.AreEqual.</summary>
        public static void AssertIs<T>(this T actual, T expected, string message = "")
        {
            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>Assert.IsTrue(predicate(value))</summary>
        public static void AssertIs<T>(this T value, Func<T, bool> predicate, string message = "")
        {
            Assert.IsTrue(predicate(value), message);
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void AssertIsCollection<T>(this IEnumerable<T> actual, params T[] expected)
        {
            AssertIsCollection(actual, expected.AsEnumerable());
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void AssertIsCollection<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message = "")
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), message);
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void AssertIsCollection<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer, string message = "")
        {
            AssertIsCollection(actual, expected, comparer.Equals, message);
        }

        /// <summary>CollectionAssert.AreEqual</summary>
        public static void AssertIsCollection<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<T, T, bool> equalityComparison, string message = "")
        {
            CollectionAssert.AreEqual(expected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
        }

        /// <summary>Assert.AreNotEqual</summary>
        public static void AssertIsNot<T>(this T actual, T notExpected, string message = "")
        {
            Assert.AreNotEqual(notExpected, actual, message);
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void AssertIsNotCollection<T>(this IEnumerable<T> actual, params T[] notExpected)
        {
            AssertIsNotCollection(actual, notExpected.AsEnumerable());
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void AssertIsNotCollection<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, string message = "")
        {
            CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), message);
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void AssertIsNotCollection<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, IEqualityComparer<T> comparer, string message = "")
        {
            AssertIsNotCollection(actual, notExpected, comparer.Equals, message);
        }

        /// <summary>CollectionAssert.AreNotEqual</summary>
        public static void AssertIsNotCollection<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, Func<T, T, bool> equalityComparison, string message = "")
        {
            CollectionAssert.AreNotEqual(notExpected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison), message);
        }

        /// <summary>Asset Collefction is empty?</summary>
        public static void AssertIsEmpty<T>(this IEnumerable<T> source)
        {
            source.Any().AssertIsFalse();
        }

        /// <summary>Assert.IsNull</summary>
        public static void AssertIsNull<T>(this T value, string message = "")
        {
            Assert.IsNull(value, message);
        }

        /// <summary>Assert.IsNotNull</summary>
        public static void AssertIsNotNull<T>(this T value, string message = "")
        {
            Assert.IsNotNull(value, message);
        }

        /// <summary>Is(true)</summary>
        public static void AssertIsTrue(this bool value, string message = "")
        {
            value.AssertIs(true, message);
        }

        /// <summary>Is(false)</summary>
        public static void AssertIsFalse(this bool value, string message = "")
        {
            value.AssertIs(false, message);
        }

        /// <summary>Assert.AreSame</summary>
        public static void AssertIsSameReferenceAs<T>(this T actual, T expected, string message = "")
        {
            Assert.AreSame(expected, actual, message);
        }

        /// <summary>Assert.AreNotSame</summary>
        public static void AssertIsNotSameReferenceAs<T>(this T actual, T notExpected, string message = "")
        {
            Assert.AreNotSame(notExpected, actual, message);
        }

        /// <summary>Assert.IsInstanceOf</summary>
        public static TExpected AssertIsInstanceOf<TExpected>(this object value, string message = "")
        {
            Assert.IsInstanceOfType(value, typeof(TExpected), message);
            return (TExpected)value;
        }

        /// <summary>Assert.IsNotInstanceOf</summary>
        public static void AssertIsNotInstanceOf<TWrong>(this object value, string message = "")
        {
            Assert.IsNotInstanceOfType(value, typeof(TWrong), message);
        }

        /// <summary>EqualityComparison to IComparer Converter for CollectionAssert</summary>
        private class ComparisonComparer<T> : IComparer
        {
            readonly Func<T, T, bool> comparison;

            public ComparisonComparer(Func<T, T, bool> comparison)
            {
                this.comparison = comparison;
            }

            public int Compare(object x, object y)
            {
                return (comparison != null)
                    ? comparison((T)x, (T)y) ? 0 : -1
                    : object.Equals(x, y) ? 0 : -1;
            }
        }
    }
}