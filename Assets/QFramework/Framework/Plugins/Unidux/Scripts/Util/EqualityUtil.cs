using System;
using System.Collections;
using UnityEngine;

namespace Unidux.Util
{
    public static class EqualityUtil
    {
        public static bool EntityEquals(object thisObject, object targetObject)
        {
            if (thisObject == null || targetObject == null)
            {
                return thisObject == targetObject;
            }

            if (thisObject.GetType() != targetObject.GetType())
            {
                return false;
            }

            return FieldsEquals(thisObject, targetObject) && PropertiesEquals(thisObject, targetObject);
        }

        public static bool ObjectEquals(object thisObject, object targetObject)
        {
            if (thisObject == null || targetObject == null)
            {
                return thisObject == targetObject;
            }

            if (thisObject.GetType() != targetObject.GetType())
            {
                return false;
            }

            var type = thisObject.GetType();

            if (type.IsPrimitive)
            {
                if (type == typeof(double))
                {
                    return DoubleEquals((double) thisObject, (double) targetObject);
                }
                else if (type == typeof(float))
                {
                    return FloatEquals((float) thisObject, (float) targetObject);
                }

                return thisObject.Equals(targetObject);
            }

            if (type.IsEnum)
            {
                return thisObject.Equals(targetObject);
            }

            if (thisObject is IEnumerable)
            {
                return EnumerableEquals((IEnumerable) thisObject, (IEnumerable) targetObject);
            }

            return thisObject.Equals(targetObject);
        }

        public static bool FieldsEquals(object thisObject, object targetObject)
        {
            var fields = thisObject.GetType().GetFields();
            foreach (var field in fields)
            {
                var thisValue = field.GetValue(thisObject);
                var targetValue = field.GetValue(targetObject);

                if (!ObjectEquals(thisValue, targetValue))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PropertiesEquals(object thisObject, object targetObject)
        {
            var properties = thisObject.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!property.CanRead)
                {
                    continue;
                }

                var thisValue = property.GetValue(thisObject, null);
                var targetValue = property.GetValue(targetObject, null);

                if (!ObjectEquals(thisValue, targetValue))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool EnumerableEquals(IEnumerable thisEnumerable, IEnumerable targetEnumerable)
        {
            var thisEnumerator = thisEnumerable.GetEnumerator();
            var targetEnumerator = targetEnumerable.GetEnumerator();

            var thisNext = thisEnumerator.MoveNext();
            var targetNext = targetEnumerator.MoveNext();

            while (thisNext && targetNext)
            {
                var thisValue = thisEnumerator.Current;
                var targetValue = targetEnumerator.Current;

                if (!ObjectEquals(thisValue, targetValue))
                {
                    return false;
                }

                thisNext = thisEnumerator.MoveNext();
                targetNext = targetEnumerator.MoveNext();
            }

            return thisNext == targetNext;
        }

        public static bool FloatEquals(float originValue, float targetValue)
        {
            return FloatEquals(originValue, targetValue, Mathf.Epsilon);
        }

        public static bool FloatEquals(float originValue, float targetValue, float precision)
        {
            return Mathf.Abs(originValue - targetValue) <= precision;
        }

        public static bool DoubleEquals(double originValue, double targetValue)
        {
            return DoubleEquals(originValue, targetValue, Double.Epsilon);
        }

        public static bool DoubleEquals(double originValue, double targetValue, double precision)
        {
            return Math.Abs(originValue - targetValue) <= precision;
        }
    }
}