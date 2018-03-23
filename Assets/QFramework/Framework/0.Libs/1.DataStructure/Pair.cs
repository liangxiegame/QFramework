/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

namespace QFramework
{
    using System;
    using System.Collections.Generic;

    // Pair is used for Observable.Pairwise
    [Serializable]
    public struct Pair<T> : IEquatable<Pair<T>>
    {
        readonly T previous;
        readonly T current;

        public T Previous
        {
            get { return previous; }
        }

        public T Current
        {
            get { return current; }
        }

        public Pair(T previous, T current)
        {
            this.previous = previous;
            this.current = current;
        }

        public override int GetHashCode()
        {
            var comparer = EqualityComparer<T>.Default;

            int h0;
            h0 = comparer.GetHashCode(previous);
            h0 = (h0 << 5) + h0 ^ comparer.GetHashCode(current);
            return h0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pair<T>)) return false;

            return Equals((Pair<T>)obj);
        }

        public bool Equals(Pair<T> other)
        {
            var comparer = EqualityComparer<T>.Default;

            return comparer.Equals(previous, other.Previous) &&
                comparer.Equals(current, other.Current);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", previous, current);
        }
    }
}