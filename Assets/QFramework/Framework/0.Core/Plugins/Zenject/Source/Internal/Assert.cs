using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zenject;

namespace ModestTree
{
    public static class Assert
    {
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void That(bool condition)
        {
            if (!condition)
            {
                throw CreateException("Assert hit!");
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotEmpty(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw CreateException("Unexpected null or empty string");
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        // This is better because IsEmpty with IEnumerable causes a memory alloc
        public static void IsEmpty<T>(IList<T> list)
        {
            if (list.Count != 0)
            {
                throw CreateException(
                    "Expected collection to be empty but instead found '{0}' elements", list.Count);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsEmpty<T>(IEnumerable<T> sequence)
        {
            if (!sequence.IsEmpty())
            {
                throw CreateException("Expected collection to be empty but instead found '{0}' elements",
                    sequence.Count());
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsType<T>(object obj)
        {
            IsType<T>(obj, "");
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsType<T>(object obj, string message)
        {
            if (!(obj is T))
            {
                throw CreateException("Assert Hit! {0}\nWrong type found. Expected '{1}' (left) but found '{2}' (right). ", message, typeof(T).PrettyName(), obj.GetType().PrettyName());
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void DerivesFrom<T>(Type type)
        {
            if (!type.DerivesFrom<T>())
            {
                throw CreateException("Expected type '{0}' to derive from '{1}'", type.Name, typeof(T).Name);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void DerivesFromOrEqual<T>(Type type)
        {
            if (!type.DerivesFromOrEqual<T>())
            {
                throw CreateException("Expected type '{0}' to derive from or be equal to '{1}'", type.Name, typeof(T).Name);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void DerivesFrom(Type childType, Type parentType)
        {
            if (!childType.DerivesFrom(parentType))
            {
                throw CreateException("Expected type '{0}' to derive from '{1}'", childType.Name, parentType.Name);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void DerivesFromOrEqual(Type childType, Type parentType)
        {
            if (!childType.DerivesFromOrEqual(parentType))
            {
                throw CreateException("Expected type '{0}' to derive from or be equal to '{1}'", childType.Name, parentType.Name);
            }
        }

        // Use AssertEquals to get better error output (with values)
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsEqual(object left, object right)
        {
            IsEqual(left, right, "");
        }

        // Use AssertEquals to get better error output (with values)
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsEqual(object left, object right, Func<string> messageGenerator)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                throw CreateException("Assert Hit! {0}.  Expected '{1}' (left) but found '{2}' (right). ", messageGenerator(), left, right);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsApproximately(float left, float right, float epsilon = 0.00001f)
        {
            bool isEqual = Math.Abs(left - right) < epsilon;

            if (!isEqual)
            {
                throw CreateException("Assert Hit! Expected '{0}' (left) but found '{1}' (right). ", left, right);
            }
        }

        // Use AssertEquals to get better error output (with values)
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsEqual(object left, object right, string message)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                throw CreateException("Assert Hit! {0}\nExpected '{1}' (left) but found '{2}' (right). ", message, left, right);
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotEqual(object left, object right)
        {
            IsNotEqual(left, right, "");
        }

        // Use Assert.IsNotEqual to get better error output (with values)
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotEqual(object left, object right, Func<string> messageGenerator)
        {
            if(object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                throw CreateException("Assert Hit! {0}.  Expected '{1}' (left) to differ from '{2}' (right). ", messageGenerator(), left, right);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNull(object val)
        {
            if (val != null)
            {
                throw CreateException(
                    "Assert Hit! Expected null pointer but instead found '{0}'", val);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNull(object val, string message)
        {
            if (val != null)
            {
                throw CreateException(
                    "Assert Hit! {0}", message);
            }
        }

        // We don't use params here to avoid the memory alloc
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNull(object val, string message, object p1)
        {
            if (val != null)
            {
                throw CreateException(
                    "Assert Hit! {0}", message.Fmt(p1));
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotNull(object val)
        {
            if (val == null)
            {
                throw CreateException("Assert Hit! Found null pointer when value was expected");
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotNull(object val, string message)
        {
            if (val == null)
            {
                throw CreateException("Assert Hit! {0}", message);
            }
        }

        // We don't use params here to avoid the memory alloc
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotNull(object val, string message, object p1)
        {
            if (val == null)
            {
                throw CreateException("Assert Hit! {0}", message.Fmt(p1));
            }
        }

        // We don't use params here to avoid the memory alloc
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotNull(object val, string message, object p1, object p2)
        {
            if (val == null)
            {
                throw CreateException("Assert Hit! {0}", message.Fmt(p1, p2));
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotEmpty<T>(IEnumerable<T> val, string message = "")
        {
            if (!val.Any())
            {
                throw CreateException("Assert Hit! Expected empty collection but found {0} values. {1}", val.Count(), message);
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void IsNotEqual(object left, object right, string message)
        {
            if (object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                throw CreateException("Assert Hit! {0}. Unexpected value found '{1}'. ", message, left);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void Warn(bool condition)
        {
            if (!condition)
            {
                ModestTree.Log.Warn("Warning!  See call stack");
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void Warn(bool condition, Func<string> messageGenerator)
        {
            if (!condition)
            {
                ModestTree.Log.Warn("Warning Assert hit! " + messageGenerator());
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void That(
            bool condition, string message)
        {
            if (!condition)
            {
                throw CreateException("Assert hit! " + message);
            }
        }

        // We don't use params here to avoid the memory alloc
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void That(
            bool condition, string message, object p1)
        {
            if (!condition)
            {
                throw CreateException("Assert hit! " + message.Fmt(p1));
            }
        }

        // We don't use params here to avoid the memory alloc
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void That(
            bool condition, string message, object p1, object p2)
        {
            if (!condition)
            {
                throw CreateException("Assert hit! " + message.Fmt(p1, p2));
            }
        }

        // We don't use params here to avoid the memory alloc
#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void That(
            bool condition, string message, object p1, object p2, object p3)
        {
            if (!condition)
            {
                throw CreateException("Assert hit! " + message.Fmt(p1, p2, p3));
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void Warn(bool condition, string message)
        {
            if (!condition)
            {
                ModestTree.Log.Warn("Warning Assert hit! " + message);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void Throws(Action action)
        {
            Throws<Exception>(action);
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            throw CreateException(
                "Expected to receive exception of type '{0}' but nothing was thrown", typeof(TException).Name);
        }

        public static ZenjectException CreateException()
        {
            return new ZenjectException("Assert hit!");
        }

        public static ZenjectException CreateException(string message)
        {
            return new ZenjectException(message);
        }

        public static ZenjectException CreateException(string message, params object[] parameters)
        {
            return new ZenjectException(message.Fmt(parameters));
        }

        public static ZenjectException CreateException(Exception innerException, string message, params object[] parameters)
        {
            return new ZenjectException(message.Fmt(parameters), innerException);
        }
    }
}
