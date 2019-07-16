using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModestTree;

namespace Zenject
{
    [DebuggerStepThrough]
    public struct TypeValuePair
    {
        public Type Type;
        public object Value;

        public TypeValuePair(Type type, object value)
        {
            Type = type;
            Value = value;
        }
    }

    [DebuggerStepThrough]
    public static class InjectUtil
    {
        public static List<TypeValuePair> CreateArgList(IEnumerable<object> args)
        {
            Assert.That(!args.ContainsItem(null),
                "Cannot include null values when creating a zenject argument list because zenject has no way of deducing the type from a null value.  If you want to allow null, use the Explicit form.");
            return args.Select(x => new TypeValuePair(x.GetType(), x)).ToList();
        }

        public static TypeValuePair CreateTypePair<T>(T param)
        {
            // Use the most derived type that we can find here
            return new TypeValuePair(
                param == null ? typeof(T) : param.GetType(), param);
        }

        public static List<TypeValuePair> CreateArgListExplicit<T>(T param)
        {
            return new List<TypeValuePair>
            {
                CreateTypePair(param)
            };
        }

        public static List<TypeValuePair> CreateArgListExplicit<TParam1, TParam2>(TParam1 param1, TParam2 param2)
        {
            return new List<TypeValuePair>
            {
                CreateTypePair(param1),
                CreateTypePair(param2)
            };
        }

        public static List<TypeValuePair> CreateArgListExplicit<TParam1, TParam2, TParam3>(
            TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return new List<TypeValuePair>
            {
                CreateTypePair(param1),
                CreateTypePair(param2),
                CreateTypePair(param3)
            };
        }

        public static List<TypeValuePair> CreateArgListExplicit<TParam1, TParam2, TParam3, TParam4>(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return new List<TypeValuePair>
            {
                CreateTypePair(param1),
                CreateTypePair(param2),
                CreateTypePair(param3),
                CreateTypePair(param4)
            };
        }

        public static List<TypeValuePair> CreateArgListExplicit<TParam1, TParam2, TParam3, TParam4, TParam5>(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return new List<TypeValuePair>
            {
                CreateTypePair(param1),
                CreateTypePair(param2),
                CreateTypePair(param3),
                CreateTypePair(param4),
                CreateTypePair(param5)
            };
        }

        public static List<TypeValuePair> CreateArgListExplicit<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            return new List<TypeValuePair>
            {
                CreateTypePair(param1),
                CreateTypePair(param2),
                CreateTypePair(param3),
                CreateTypePair(param4),
                CreateTypePair(param5),
                CreateTypePair(param6)
            };
        }

        // Find the first match with the given type and remove it from the list
        // Return true if it was removed
        public static bool PopValueWithType(
            List<TypeValuePair> extraArgMap, Type injectedFieldType, out object value)
        {
            for (int i = 0; i < extraArgMap.Count; i++)
            {
                var arg = extraArgMap[i];

                if (arg.Type.DerivesFromOrEqual(injectedFieldType))
                {
                    value = arg.Value;
                    extraArgMap.RemoveAt(i);
                    return true;
                }
            }

            value = injectedFieldType.GetDefaultValue();
            return false;
        }
    }
}
