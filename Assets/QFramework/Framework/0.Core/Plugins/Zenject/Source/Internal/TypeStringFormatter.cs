using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree
{
    public static class TypeStringFormatter
    {
        static readonly Dictionary<Type, string> _prettyNameCache = new Dictionary<Type, string>();

        public static string PrettyName(this Type type)
        {
            string prettyName;

            if (!_prettyNameCache.TryGetValue(type, out prettyName))
            {
                prettyName = PrettyNameInternal(type);
                _prettyNameCache.Add(type, prettyName);
            }

            return prettyName;
        }

        static string PrettyNameInternal(Type type)
        {
            var sb = new StringBuilder();

            if (type.IsNested)
            {
                sb.Append(type.DeclaringType.PrettyName());
                sb.Append(".");
            }

            if (type.IsArray)
            {
                sb.Append(type.GetElementType().PrettyName());
                sb.Append("[]");
            }
            else
            {
                var name = GetCSharpTypeName(type.Name);

                if (type.IsGenericType())
                {
                    var quoteIndex = name.IndexOf('`');

                    if (quoteIndex != -1)
                    {
                        sb.Append(name.Substring(0, name.IndexOf('`')));
                    }
                    else
                    {
                        sb.Append(name);
                    }

                    sb.Append("<");

                    if (type.IsGenericTypeDefinition())
                    {
                        var numArgs = type.GenericArguments().Count();

                        if (numArgs > 0)
                        {
                            sb.Append(new String(',', numArgs - 1));
                        }
                    }
                    else
                    {
                        sb.Append(string.Join(", ", type.GenericArguments().Select(t => t.PrettyName()).ToArray()));
                    }

                    sb.Append(">");
                }
                else
                {
                    sb.Append(name);
                }
            }

            return sb.ToString();
        }

        static string GetCSharpTypeName(string typeName)
        {
            switch (typeName)
            {
                case "String":
                case "Object":
                case "Void":
                case "Byte":
                case "Double":
                case "Decimal":
                    return typeName.ToLower();
                case "Int16":
                    return "short";
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                case "Single":
                    return "float";
                case "Boolean":
                    return "bool";
                default:
                    return typeName;
            }
        }
    }
}

