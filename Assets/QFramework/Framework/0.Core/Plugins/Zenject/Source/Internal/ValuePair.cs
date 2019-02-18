using System;
using System.Collections.Generic;

namespace ModestTree.Util
{
    public class ValuePair<T1, T2>
    {
        public readonly T1 First;
        public readonly T2 Second;

        public ValuePair()
        {
            First = default(T1);
            Second = default(T2);
        }

        public ValuePair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(Object obj)
        {
            var that = obj as ValuePair<T1, T2>;

            if (that == null)
            {
                return false;
            }

            return Equals(that);
        }

        public bool Equals(ValuePair<T1, T2> that)
        {
            if (that == null)
            {
                return false;
            }

            return object.Equals(First, that.First) && object.Equals(Second, that.Second);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (First == null ? 0 : First.GetHashCode());
                hash = hash * 29 + (Second == null ? 0 : Second.GetHashCode());
                return hash;
            }
        }
    }

    public class ValuePair<T1, T2, T3>
    {
        public readonly T1 First;
        public readonly T2 Second;
        public readonly T3 Third;

        public ValuePair()
        {
            First = default(T1);
            Second = default(T2);
            Third = default(T3);
        }

        public ValuePair(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public override bool Equals(Object obj)
        {
            var that = obj as ValuePair<T1, T2, T3>;

            if (that == null)
            {
                return false;
            }

            return Equals(that);
        }

        public bool Equals(ValuePair<T1, T2, T3> that)
        {
            if (that == null)
            {
                return false;
            }

            return object.Equals(First, that.First) && object.Equals(Second, that.Second) && object.Equals(Third, that.Third);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (First == null ? 0 : First.GetHashCode());
                hash = hash * 29 + (Second == null ? 0 : Second.GetHashCode());
                hash = hash * 29 + (Third == null ? 0 : Third.GetHashCode());
                return hash;
            }
        }
    }

    public class ValuePair<T1, T2, T3, T4>
    {
        public readonly T1 First;
        public readonly T2 Second;
        public readonly T3 Third;
        public readonly T4 Fourth;

        public ValuePair()
        {
            First = default(T1);
            Second = default(T2);
            Third = default(T3);
            Fourth = default(T4);
        }

        public ValuePair(T1 first, T2 second, T3 third, T4 fourth)
        {
            First = first;
            Second = second;
            Third = third;
            Fourth = fourth;
        }

        public override bool Equals(Object obj)
        {
            var that = obj as ValuePair<T1, T2, T3, T4>;

            if (that == null)
            {
                return false;
            }

            return Equals(that);
        }

        public bool Equals(ValuePair<T1, T2, T3, T4> that)
        {
            if (that == null)
            {
                return false;
            }

            return object.Equals(First, that.First) && object.Equals(Second, that.Second)
                && object.Equals(Third, that.Third) && object.Equals(Fourth, that.Fourth);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (First == null ? 0 : First.GetHashCode());
                hash = hash * 29 + (Second == null ? 0 : Second.GetHashCode());
                hash = hash * 29 + (Third == null ? 0 : Third.GetHashCode());
                hash = hash * 29 + (Fourth == null ? 0 : Fourth.GetHashCode());
                return hash;
            }
        }
    }

    public static class ValuePair
    {
        public static ValuePair<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            return new ValuePair<T1, T2>(first, second);
        }

        public static ValuePair<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            return new ValuePair<T1, T2, T3>(first, second, third);
        }

        public static ValuePair<T1, T2, T3, T4> New<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            return new ValuePair<T1, T2, T3, T4>(first, second, third, fourth);
        }
    }
}

