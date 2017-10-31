/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

// original code from rx.codeplex.com

/* ------------------ */

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    /// <summary>
    /// Represents a value associated with time interval information.
    /// The time interval can represent the time it took to produce the value, the interval relative to a previous value, the value's delivery time relative to a base, etc.
    /// </summary>
    /// <typeparam name="T">The type of the value being annotated with time interval information.</typeparam>
    [Serializable]
    public struct TimeInterval<T> : IEquatable<TimeInterval<T>>
    {
        private readonly TimeSpan _interval;
        private readonly T _value;

        /// <summary>
        /// Constructs a time interval value.
        /// </summary>
        /// <param name="value">The value to be annotated with a time interval.</param>
        /// <param name="interval">Time interval associated with the value.</param>
        public TimeInterval(T value, TimeSpan interval)
        {
            _interval = interval;
            _value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        public TimeSpan Interval
        {
            get { return _interval; }
        }

        /// <summary>
        /// Determines whether the current TimeInterval&lt;T&gt; value has the same Value and Interval as a specified TimeInterval&lt;T&gt; value.
        /// </summary>
        /// <param name="other">An object to compare to the current TimeInterval&lt;T&gt; value.</param>
        /// <returns>true if both TimeInterval&lt;T&gt; values have the same Value and Interval; otherwise, false.</returns>
        public bool Equals(TimeInterval<T> other)
        {
            return other.Interval.Equals(Interval) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the two specified TimeInterval&lt;T&gt; values have the same Value and Interval.
        /// </summary>
        /// <param name="first">The first TimeInterval&lt;T&gt; value to compare.</param>
        /// <param name="second">The second TimeInterval&lt;T&gt; value to compare.</param>
        /// <returns>true if the first TimeInterval&lt;T&gt; value has the same Value and Interval as the second TimeInterval&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator ==(TimeInterval<T> first, TimeInterval<T> second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Determines whether the two specified TimeInterval&lt;T&gt; values don't have the same Value and Interval.
        /// </summary>
        /// <param name="first">The first TimeInterval&lt;T&gt; value to compare.</param>
        /// <param name="second">The second TimeInterval&lt;T&gt; value to compare.</param>
        /// <returns>true if the first TimeInterval&lt;T&gt; value has a different Value or Interval as the second TimeInterval&lt;T&gt; value; otherwise, false.</returns>
        public static bool operator !=(TimeInterval<T> first, TimeInterval<T> second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current TimeInterval&lt;T&gt;.
        /// </summary>
        /// <param name="obj">The System.Object to compare with the current TimeInterval&lt;T&gt;.</param>
        /// <returns>true if the specified System.Object is equal to the current TimeInterval&lt;T&gt;; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TimeInterval<T>))
                return false;

            var other = (TimeInterval<T>)obj;
            return this.Equals(other);
        }

        /// <summary>
        /// Returns the hash code for the current TimeInterval&lt;T&gt; value.
        /// </summary>
        /// <returns>A hash code for the current TimeInterval&lt;T&gt; value.</returns>
        public override int GetHashCode()
        {
            var valueHashCode = Value == null ? 1963 : Value.GetHashCode();

            return Interval.GetHashCode() ^ valueHashCode;
        }

        /// <summary>
        /// Returns a string representation of the current TimeInterval&lt;T&gt; value.
        /// </summary>
        /// <returns>String representation of the current TimeInterval&lt;T&gt; value.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}@{1}", Value, Interval);
        }
    }
}
