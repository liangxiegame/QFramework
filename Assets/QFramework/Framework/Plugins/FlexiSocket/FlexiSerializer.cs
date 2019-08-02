// *************************************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2016 Sean
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *************************************************************************************************
// Project source: https://github.com/theoxuan/FlexiSocket

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace QF
{
    public sealed class FlexiSerializer
    {
        public readonly object context;
        public FlexiSerializationLayout Layout { get; set; }

        public FlexiSerializer(object context)
        {
            this.context = context;
        }

        public FlexiSerializer()
        {
        }

        /// <summary>
        /// Serialize object to stream
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="stream">Output stream</param>
        public void Serialize(object obj, Stream stream)
        {
            if (obj == null)
                throw new ArgumentNullException();

            Serialize(obj.GetType(), obj, new BinaryWriter(stream));
        }

        /// <summary>
        /// Deserialize object from stream
        /// </summary>
        /// <param name="type">Object type</param>
        /// <param name="stream">Input stream</param>
        /// <returns>Deserialized object</returns>
        public object Deserialize(Type type, Stream stream)
        {
            return Deserialize(type, new BinaryReader(stream));
        }

        /// <summary>
        /// Deserialize object from stream
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="stream">Input stream</param>
        /// <returns>Deserialized object</returns>
        public T Deserialize<T>(Stream stream)
        {
            return (T) Deserialize(typeof (T), stream);
        }

        /// <summary>
        /// Encode object to string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="builder">Output StringBuilder</param>
        public void Encode(object obj, StringBuilder builder)
        {
            if (obj == null)
                throw new ArgumentNullException();

            Encode(obj.GetType(), obj, builder);
            if (builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);
        }

        /// <summary>
        /// Decode object from string
        /// </summary>
        /// <param name="type">Object type</param>
        /// <param name="content">Input string</param>
        /// <returns>Deserialized object</returns>
        public object Decode(Type type, string content)
        {
            return Encode(type, new MatchReader(content));
        }

        /// <summary>
        /// Decode object from string
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="content">Input string</param>
        /// <returns>Deserialized object</returns>
        public T Decode<T>(string content)
        {
            return (T) Decode(typeof (T), content);
        }

        private IEnumerable<MemberInfo> Reflect(Type type)
        {
            var attribute =
                Attribute.GetCustomAttribute(type, typeof (FlexiSerializableAttribute), false) as
                    FlexiSerializableAttribute;
            var options = attribute == null ? FlexiSerializationOptions.Default : attribute.Options;
            var flags = BindingFlags.Instance;
            if ((options & FlexiSerializationOptions.DeclaredOnly) == FlexiSerializationOptions.DeclaredOnly)
                flags |= BindingFlags.DeclaredOnly;
            var members = Enumerable.Empty<MemberInfo>();
            if ((options & FlexiSerializationOptions.Fields) == FlexiSerializationOptions.Fields)
                members =
                    members.Concat(
                        type.GetFields(flags | BindingFlags.Public)
                            .Where(
                                m =>
                                    !m.IsDefined(typeof (FlexiMemberIgnoreAttribute), false) && !m.FieldType.IsInterface &&
                                    !m.FieldType.IsAbstract)
                            .Concat(
                                type.GetFields(flags | BindingFlags.NonPublic)
                                    .Where(
                                        m =>
                                            m.IsDefined(typeof (FlexiMemberIncludeAttribute), false) &&
                                            !m.FieldType.IsInterface && !m.FieldType.IsAbstract))
                            .Cast<MemberInfo>());
            if ((options & FlexiSerializationOptions.Properties) == FlexiSerializationOptions.Properties)
                members =
                    members.Concat(
                        type.GetProperties(flags | BindingFlags.Public)
                            .Where(
                                m =>
                                    m.CanRead && m.CanWrite && m.GetIndexParameters().Length == 0 &&
                                    !m.IsDefined(typeof (FlexiMemberIgnoreAttribute), false) &&
                                    !m.PropertyType.IsInterface && !m.PropertyType.IsAbstract)
                            .Concat(
                                type.GetProperties(flags | BindingFlags.NonPublic)
                                    .Where(
                                        m =>
                                            m.CanRead && m.CanWrite && m.GetIndexParameters().Length == 0 &&
                                            m.IsDefined(typeof (FlexiMemberIncludeAttribute), false) &&
                                            !m.PropertyType.IsInterface && !m.PropertyType.IsAbstract))
                            .Cast<MemberInfo>());
            switch (Layout)
            {
                case FlexiSerializationLayout.MetadataToken:
                    return members.OrderBy(m => m.MetadataToken);
                case FlexiSerializationLayout.Alphabetical:
                    return members.OrderBy(m => m.Name);
                case FlexiSerializationLayout.Mapping:
                    return members;
                case FlexiSerializationLayout.Explicit:
                    return
                        members.Where(m => m.IsDefined(typeof (FlexiMemberOrderAttribute), false))
                            .OrderBy(
                                m =>
                                    ((FlexiMemberOrderAttribute)
                                        Attribute.GetCustomAttribute(m, typeof (FlexiMemberOrderAttribute))).order);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Serialize(Type type, object obj, BinaryWriter writer)
        {
            if (type.IsInterface || type.IsAbstract)
                throw new InvalidOperationException("Unsupported type: " + type.FullName);

            if (obj == null)
                writer.Write(false);
            else
            {
                if (!type.IsValueType)
                    writer.Write(true);

                if (context != null)
                {
                    var contextType = context.GetType();
                    var converterType = typeof (IFlexiSerializer<>).MakeGenericType(type);
                    if (converterType.IsAssignableFrom(contextType))
                    {
                        contextType.GetInterfaceMap(converterType)
                            .InterfaceMethods.First(m => m.ReturnType == typeof (void))
                            .Invoke(context, new[] {obj, writer});
                        return;
                    }
                }
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        writer.Write((bool) obj);
                        break;
                    case TypeCode.Char:
                        writer.Write((char) obj);
                        break;
                    case TypeCode.SByte:
                        writer.Write(Convert.ToSByte(obj));
                        break;
                    case TypeCode.Byte:
                        writer.Write(Convert.ToByte(obj));
                        break;
                    case TypeCode.Int16:
                        writer.Write(Convert.ToInt16(obj));
                        break;
                    case TypeCode.UInt16:
                        writer.Write(Convert.ToUInt16(obj));
                        break;
                    case TypeCode.Int32:
                        writer.Write(Convert.ToInt32(obj));
                        break;
                    case TypeCode.UInt32:
                        writer.Write(Convert.ToUInt32(obj));
                        break;
                    case TypeCode.Int64:
                        writer.Write(Convert.ToInt64(obj));
                        break;
                    case TypeCode.UInt64:
                        writer.Write(Convert.ToUInt64(obj));
                        break;
                    case TypeCode.Single:
                        writer.Write((float) obj);
                        break;
                    case TypeCode.Double:
                        writer.Write((double) obj);
                        break;
                    case TypeCode.Decimal:
                        writer.Write((decimal) obj);
                        break;
                    case TypeCode.DateTime:
                        writer.Write(((DateTime) obj).ToBinary());
                        break;
                    case TypeCode.String:
                        writer.Write((string) obj);
                        break;
                    case TypeCode.Object:
                        if (type.IsArray)
                        {
                            var array = (Array) obj;
                            var elementType = type.GetElementType();
                            writer.Write(array.Length);
                            if (array.Length > 0)
                            {
                                switch (Type.GetTypeCode(elementType))
                                {
                                    case TypeCode.Char:
                                        writer.Write((char[]) array);
                                        break;
                                    case TypeCode.Byte:
                                        writer.Write((byte[]) array);
                                        break;
                                    default:
                                        foreach (var item in array)
                                            Serialize(elementType, item, writer);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var memberInfo in Reflect(type))
                            {
                                if (Layout == FlexiSerializationLayout.Mapping)
                                    writer.Write(memberInfo.Name);
                                Serialize(memberInfo.MemberType == MemberTypes.Field
                                    ? ((FieldInfo) memberInfo).FieldType
                                    : ((PropertyInfo) memberInfo).PropertyType,
                                    memberInfo.MemberType == MemberTypes.Field
                                        ? ((FieldInfo) memberInfo).GetValue(obj)
                                        : ((PropertyInfo) memberInfo).GetValue(obj, null), writer);
                            }

                            if (type.IsGenericType)
                            {
                                var def = type.GetGenericTypeDefinition();
                                if (def == typeof (List<>))
                                {
                                    var itemType = type.GetGenericArguments()[0];
                                    writer.Write(((ICollection) obj).Count);
                                    foreach (var item in (IList) obj)
                                        Serialize(itemType, item, writer);
                                }
                                else if (def == typeof (Dictionary<,>))
                                {
                                    var keyType = type.GetGenericArguments()[0];
                                    var valueType = type.GetGenericArguments()[1];
                                    writer.Write(((ICollection) obj).Count);
                                    foreach (var key in ((IDictionary) obj).Keys)
                                    {
                                        Serialize(keyType, key, writer);
                                        Serialize(valueType, ((IDictionary) obj)[key], writer);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new NotSupportedException(type.FullName);
                }
            }
        }

        private object Deserialize(Type type, BinaryReader reader)
        {
            if (type.IsInterface || type.IsAbstract)
                throw new InvalidOperationException("Unsupported type: " + type.FullName);

            if (!type.IsValueType && !reader.ReadBoolean())
                return null;

            if (context != null)
            {
                var contextType = context.GetType();
                var converterType = typeof (IFlexiSerializer<>).MakeGenericType(type);
                if (converterType.IsAssignableFrom(contextType))
                {
                    return contextType.GetInterfaceMap(converterType)
                        .InterfaceMethods.First(m => m.ReturnType == type)
                        .Invoke(context, new object[] {reader});
                }
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return reader.ReadBoolean();
                case TypeCode.Char:
                    return reader.ReadChar();
                case TypeCode.SByte:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadSByte()) : reader.ReadSByte();
                case TypeCode.Byte:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadByte()) : reader.ReadByte();
                case TypeCode.Int16:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadInt16()) : reader.ReadInt16();
                case TypeCode.UInt16:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadUInt16()) : reader.ReadUInt16();
                case TypeCode.Int32:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadInt32()) : reader.ReadInt32();
                case TypeCode.UInt32:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadUInt32()) : reader.ReadUInt32();
                case TypeCode.Int64:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadInt64()) : reader.ReadInt64();
                case TypeCode.UInt64:
                    return type.IsEnum ? Enum.ToObject(type, reader.ReadUInt64()) : reader.ReadUInt64();
                case TypeCode.Single:
                    return reader.ReadSingle();
                case TypeCode.Double:
                    return reader.ReadDouble();
                case TypeCode.Decimal:
                    return reader.ReadDecimal();
                case TypeCode.DateTime:
                    return DateTime.FromBinary(reader.ReadInt64());
                case TypeCode.String:
                    return reader.ReadString();
                case TypeCode.Object:
                    if (type.IsArray)
                    {
                        var elementType = type.GetElementType();
                        var length = reader.ReadInt32();
                        switch (Type.GetTypeCode(elementType))
                        {
                            case TypeCode.Char:
                                return length > 0 ? reader.ReadChars(length) : new char[0];
                            case TypeCode.Byte:
                                return length > 0 ? reader.ReadBytes(length) : new byte[0];
                            default:
                                var array = Array.CreateInstance(elementType, length);
                                for (var i = 0; i < length; i++)
                                    array.SetValue(Deserialize(elementType, reader), i);
                                return array;
                        }
                    }

                    var obj = FormatterServices.GetUninitializedObject(type);
                    var members = Reflect(type).ToArray();
                    if (Layout == FlexiSerializationLayout.Mapping)
                    {
                        var count = members.Length;
                        while (count-- > 0)
                        {
                            var name = reader.ReadString();
                            var member = members.First(m => m.Name == name);
                            if (member.MemberType == MemberTypes.Field)
                                ((FieldInfo) member).SetValue(obj,
                                    Deserialize(((FieldInfo) member).FieldType, reader));
                            else
                                ((PropertyInfo) member).SetValue(obj,
                                    Deserialize(((PropertyInfo) member).PropertyType, reader), null);
                        }
                    }
                    else
                    {
                        foreach (var member in members)
                        {
                            if (member.MemberType == MemberTypes.Field)
                                ((FieldInfo) member).SetValue(obj,
                                    Deserialize(((FieldInfo) member).FieldType, reader));
                            else
                                ((PropertyInfo) member).SetValue(obj,
                                    Deserialize(((PropertyInfo) member).PropertyType, reader), null);
                        }
                    }

                    if (type.IsGenericType)
                    {
                        var def = type.GetGenericTypeDefinition();
                        if (def == typeof (List<>))
                        {
                            var itemType = type.GetGenericArguments()[0];
                            var count = reader.ReadInt32();
                            while (count-- > 0)
                            {
                                var item = Deserialize(itemType, reader);
                                ((IList) obj).Add(item);
                            }
                        }
                        else if (def == typeof (Dictionary<,>))
                        {
                            var keyType = type.GetGenericArguments()[0];
                            var valueType = type.GetGenericArguments()[1];
                            var count = reader.ReadInt32();
                            while (count-- > 0)
                                ((IDictionary) obj).Add(Deserialize(keyType, reader), Deserialize(valueType, reader));
                        }
                    }
                    return obj;
                default:
                    throw new NotSupportedException();
            }
        }

        private void Encode(Type type, object obj, StringBuilder builder)
        {
            if (type.IsInterface || type.IsAbstract)
                throw new InvalidOperationException("Unsupported type: " + type.FullName);

            if (obj == null)
                builder.AppendFormat("{0},", '*');
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        builder.AppendFormat("{0},", (bool) obj ? 1 : 0);
                        break;
                    case TypeCode.Char:
                        builder.AppendFormat("{0},", (ushort) (char) obj);
                        break;
                    case TypeCode.SByte:
                        builder.AppendFormat("{0},", (sbyte) obj);
                        break;
                    case TypeCode.Byte:
                        builder.AppendFormat("{0},", (byte) obj);
                        break;
                    case TypeCode.Int16:
                        builder.AppendFormat("{0},", (short) obj);
                        break;
                    case TypeCode.UInt16:
                        builder.AppendFormat("{0},", (ushort) obj);
                        break;
                    case TypeCode.Int32:
                        builder.AppendFormat("{0},", (int) obj);
                        break;
                    case TypeCode.UInt32:
                        builder.AppendFormat("{0},", (uint) obj);
                        break;
                    case TypeCode.Int64:
                        builder.AppendFormat("{0},", (long) obj);
                        break;
                    case TypeCode.UInt64:
                        builder.AppendFormat("{0},", (ulong) obj);
                        break;
                    case TypeCode.Single:
                        builder.AppendFormat("{0},", (float) obj);
                        break;
                    case TypeCode.Double:
                        builder.AppendFormat("{0},", (double) obj);
                        break;
                    case TypeCode.Decimal:
                        builder.AppendFormat("{0},", (decimal) obj);
                        break;
                    case TypeCode.DateTime:
                        builder.AppendFormat("{0},", ((DateTime) obj).ToBinary());
                        break;
                    case TypeCode.String:
                        builder.AppendFormat("{0},", Convert.ToBase64String(Encoding.UTF8.GetBytes((string) obj)));
                        break;
                    case TypeCode.Object:
                        if (type.IsArray)
                        {
                            var array = (Array) obj;
                            var elementType = type.GetElementType();
                            if (array.Length > 0)
                            {
                                switch (Type.GetTypeCode(elementType))
                                {
                                    case TypeCode.Char:
                                        builder.AppendFormat("{0},",
                                            Convert.ToBase64String(Encoding.UTF8.GetBytes((char[]) array)));
                                        break;
                                    case TypeCode.Byte:
                                        builder.AppendFormat("{0},", Convert.ToBase64String((byte[]) array));
                                        break;
                                    default:
                                        builder.AppendFormat("{0}!", array.Length);
                                        foreach (var item in array)
                                            Encode(elementType, item, builder);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var memberInfo in Reflect(type))
                            {
                                if (Layout == FlexiSerializationLayout.Mapping)
                                    builder.AppendFormat("{0}:", memberInfo.Name);
                                Encode(memberInfo.MemberType == MemberTypes.Field
                                    ? ((FieldInfo) memberInfo).FieldType
                                    : ((PropertyInfo) memberInfo).PropertyType,
                                    memberInfo.MemberType == MemberTypes.Field
                                        ? ((FieldInfo) memberInfo).GetValue(obj)
                                        : ((PropertyInfo) memberInfo).GetValue(obj, null), builder);
                            }

                            if (type.IsGenericType)
                            {
                                var def = type.GetGenericTypeDefinition();
                                if (def == typeof (List<>))
                                {
                                    var itemType = type.GetGenericArguments()[0];
                                    builder.AppendFormat("{0}!", ((ICollection) obj).Count);
                                    foreach (var item in (IList) obj)
                                        Encode(itemType, item, builder);
                                }
                                else if (def == typeof (Dictionary<,>))
                                {
                                    var keyType = type.GetGenericArguments()[0];
                                    var valueType = type.GetGenericArguments()[1];
                                    builder.AppendFormat("{0}!", ((ICollection) obj).Count);
                                    foreach (var key in ((IDictionary) obj).Keys)
                                    {
                                        Encode(keyType, key, builder);
                                        Encode(valueType, ((IDictionary) obj)[key], builder);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new NotSupportedException(type.FullName);
                }
            }
        }

        private object Encode(Type type, MatchReader reader)
        {
            if (type.IsInterface || type.IsAbstract)
                throw new InvalidOperationException("Unsupported type: " + type.FullName);

            throw new NotImplementedException();
        }
    }

    public sealed class MatchReader
    {
        private readonly string[] _matches;

        public MatchReader(string content)
        {
            _matches = Regex.Split(content, ",:!;?");
        }

        public int Position { get; set; }
        public int Length { get { return _matches.Length; } }

        public string Read()
        {
            if (Position < Length)
                return _matches[Position++];
            return null;
        }

        public string Peek()
        {
            if (Position < Length)
                return _matches[Position];
            return null;
        }
    }
}