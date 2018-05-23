/****************************************************************************
 * Copyright (c) 2017 liangxie
 * Copyright (c) 2018 liangxie
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Random = UnityEngine.Random;
    
    public static class TypeEx
    {
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object DefaultForType(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }

    /// <summary>
    /// 通过编写方法并且添加属性可以做到转换至String 如：
    /// 
    /// <example>
    /// [ToString]
    /// public static string ConvertToString(TestObj obj)
    /// </example>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ToString : Attribute{}

    /// <summary>
    /// 通过编写方法并且添加属性可以做到转换至String 如：
    /// 
    /// <example>
    /// [FromString]
    /// public static TestObj ConvertFromString(string str)
    /// </example>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class FromString : Attribute { }

    public static class StringExtention
    {
        public static void Example()
        {
            var emptyStr = string.Empty;
            Debug.Log(emptyStr.IsNotNullAndEmpty());
            Debug.Log(emptyStr.IsNullOrEmpty());
            emptyStr = emptyStr.Append("appended").Append("1").ToString();
            Debug.Log(emptyStr);
            Debug.Log(emptyStr.IsNullOrEmpty());
        }
        
        /// <summary>
        /// Check Whether string is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string selfStr)
        {
            return string.IsNullOrEmpty(selfStr);
        }

        /// <summary>
        /// Check Whether string is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
        public static bool IsNotNullAndEmpty(this string selfStr)
        {
            return !string.IsNullOrEmpty(selfStr);
        }

        /// <summary>
        /// Check Whether string trim is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
        public static bool IsTrimNotNullAndEmpty(this string selfStr)
        {
            return !string.IsNullOrEmpty(selfStr.Trim());
        }

        /// <summary>
        /// 避免每次都用.
        /// </summary>
        private static readonly char[] mCachedSplitCharArray = {'.'};

        public static string[] Split(this string selfStr, char splitSymbol)
        {
            mCachedSplitCharArray[0] = splitSymbol;
            return selfStr.Split(mCachedSplitCharArray);
        }

        public static string UppercaseFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string LowercaseFirst(this string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        public static string ToUnixLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string ToCSV(this string[] values)
        {
            return string.Join(", ", values
                .Where(value => !string.IsNullOrEmpty(value))
                .Select(value => value.Trim())
                .ToArray()
            );
        }

        public static string[] ArrayFromCSV(this string values)
        {
            return values
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        public static string ToSpacedCamelCase(this string text)
        {
            var sb = new StringBuilder(text.Length * 2);
            sb.Append(char.ToUpper(text[0]));
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    sb.Append(' ');
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 有点不安全,编译器不会帮你排查错误。
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FillFormat(this string selfStr, params object[] args)
        {
            return string.Format(selfStr, args);
        }

        public static StringBuilder Append(this string selfStr, string toAppend)
        {
            return new StringBuilder(selfStr).Append(toAppend);
        }

        public static string AddPrefix(this string selfStr, string toPrefix)
        {
            return new StringBuilder(toPrefix).Append(selfStr).ToString();
        }

        public static StringBuilder AppendFormat(this string selfStr, string toAppend, params object[] args)
        {
            return new StringBuilder(selfStr).AppendFormat(toAppend, args);
        }

        public static string LastWord(this string selfUrl)
        {
            return selfUrl.Split('/').Last();
        }
        
        public static int ToInt(this string selfStr, int defaulValue = 0)
        {
            var retValue = defaulValue;
            return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }
 
        public static float ToFloat(this string selfStr,float defaulValue = 0)
        {
            var retValue = defaulValue;
            return float.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

        private const char Spriter1 = ',';
        private const char Spriter2 = ':';

        private const char FBracket1 = '(';
        private const char BBracket1 = ')';

        public static T GetValue<T>(this string value)
        {
            return string.IsNullOrEmpty(value) ? default(T) : value.TryGetValue((T) typeof(T).DefaultForType());
        }

        /// <summary>
        /// 从字符串中获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T TryGetValue<T>(this string value, T defultValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            return (T) TryGetValue(value, typeof(T), defultValue);
        }

        public static object GetValue(this string value, Type type)
        {
            return value.TryGetValue(type, type.DefaultForType());
        }

        /// <summary>
        /// 从字符串中获取值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object TryGetValue(this string value, Type type, object defultValue)
        {
            try
            {
                if (type == null) return "";
                if (string.IsNullOrEmpty(value))
                {
                    return type.IsValueType ? Activator.CreateInstance(type) : null;
                }

                if (type == typeof(string))
                {
                    return value;
                }

                if (type == typeof(int))
                {
                    return Convert.ToInt32(Convert.ToDouble(value));
                }

                if (type == typeof(float))
                {
                    return float.Parse(value);
                }

                if (type == typeof(byte))
                {
                    return Convert.ToByte(Convert.ToDouble(value));
                }

                if (type == typeof(sbyte))
                {
                    return Convert.ToSByte(Convert.ToDouble(value));
                }

                if (type == typeof(uint))
                {
                    return Convert.ToUInt32(Convert.ToDouble(value));
                }

                if (type == typeof(short))
                {
                    return Convert.ToInt16(Convert.ToDouble(value));
                }

                if (type == typeof(long))
                {
                    return Convert.ToInt64(Convert.ToDouble(value));
                }

                if (type == typeof(ushort))
                {
                    return Convert.ToUInt16(Convert.ToDouble(value));
                }

                if (type == typeof(ulong))
                {
                    return Convert.ToUInt64(Convert.ToDouble(value));
                }

                if (type == typeof(double))
                {
                    return double.Parse(value);
                }

                if (type == typeof(bool))
                {
                    return bool.Parse(value);
                }

                if (type.BaseType == typeof(Enum))
                {
                    return GetValue(value, Enum.GetUnderlyingType(type));
                }

                if (type == typeof(Vector2))
                {
                    Vector2 vector;
                    ParseVector2(value, out vector);
                    return vector;
                }

                if (type == typeof(Vector3))
                {
                    Vector3 vector;
                    ParseVector3(value, out vector);
                    //Debug.LogError(vector.ToString());
                    return vector;
                }

                if (type == typeof(Vector4))
                {
                    Vector4 vector;
                    ParseVector4(value, out vector);
                    return vector;
                }

                if (type == typeof(Quaternion))
                {
                    Quaternion quaternion;
                    ParseQuaternion(value, out quaternion);
                    return quaternion;
                }

                if (type == typeof(Color))
                {
                    Color color;
                    ParseColor(value, out color);
                    return color;
                }

                object constructor;
                object genericArgument;
                //词典
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    Type[] genericArguments = type.GetGenericArguments();
                    Dictionary<string, string> dictionary = ParseMap(value, Spriter2, Spriter1);
                    constructor = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    foreach (KeyValuePair<string, string> pair in dictionary)
                    {
                        var genericArgument1 = GetValue(pair.Key, genericArguments[0]);
                        genericArgument = GetValue(pair.Value, genericArguments[1]);
                        type.GetMethod("Add").Invoke(constructor, new[] {genericArgument1, genericArgument});
                    }

                    return constructor;
                }

                //列表
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    var type2 = type.GetGenericArguments()[0];
                    var list = ParseList(value, Spriter1);

                    constructor = Activator.CreateInstance(type);
                    foreach (var str in list)
                    {
                        genericArgument = GetValue(str, type2);
                        Debug.Log(str + "  " + type2.Name);
                        type.GetMethod("Add").Invoke(constructor, new[] {genericArgument});
                    }

                    return constructor;
                }

                if (type == typeof(ArrayList))
                {
                    return value.GetValue<List<string>>() ?? defultValue;
                }

                if (type == typeof(Hashtable))
                {
                    return value.GetValue<Dictionary<string, string>>() ?? defultValue;
                }

                //数组
                if (type.IsArray)
                {
                    var elementType = Type.GetType(
                        type.FullName.Replace("[]", string.Empty));
                    var elStr = value.Split(Spriter1);
                    var array = Array.CreateInstance(elementType, elStr.Length);

                    for (var i = 0; i < elStr.Length; i++)
                    {
                        array.SetValue(elStr[i].GetValue(elementType), i);
                    }

                    return array;
                }

                if (CanConvertFromString(type))
                {
                    return ParseFromStringableObject(value, type);
                }

                Log.W("字符转换", "没有适合的转换类型，返回默认值");
                return defultValue != type.DefaultForType() ? defultValue : type.DefaultForType();
            }
            catch (Exception e)
            {
                Log.E(e);
                Log.W("字符转换", "解析失败，返回默认值");
                return type.DefaultForType();
            }
        }

        #region FromString

        /// <summary>
        /// 解析颜色
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <param name="colorSpriter"></param>
        /// <returns></returns>
        public static bool ParseColor(string _inputString, out Color result, char colorSpriter = ',')
        {
            string str = _inputString.Trim();
            str = str.Replace(FBracket1.ToString(), "");
            str = str.Replace(BBracket1.ToString(), "");
            result = Color.clear;
            if (str.Length < 9)
            {
                return false;
            }

            try
            {
                var strArray = str.Split(colorSpriter);
                if (strArray.Length != 4)
                {
                    return false;
                }

                result = new Color(float.Parse(strArray[0]) / 255f, float.Parse(strArray[1]) / 255f,
                    float.Parse(strArray[2]) / 255f, float.Parse(strArray[3]) / 255f);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 解析列表
        /// </summary>
        /// <param name="strList"></param>
        /// <param name="listSpriter"></param>
        /// <returns></returns>
        public static List<string> ParseList(this string strList, char listSpriter = ',')
        {
            var list = new List<string>();
            if (!string.IsNullOrEmpty(strList))
            {
                var str = strList.Trim();
                if (string.IsNullOrEmpty(strList))
                {
                    return list;
                }

                var strArray = str.Split(listSpriter);
                list.AddRange(from str2 in strArray where !string.IsNullOrEmpty(str2) select str2.Trim());
            }

            return list;
        }

        /// <summary>
        /// 解析词典
        /// </summary>
        /// <param name="strMap"></param>
        /// <param name="keyValueSpriter"></param>
        /// <param name="mapSpriter"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseMap(this string strMap, char keyValueSpriter = ':',
            char mapSpriter = ',')
        {
            var dictionary = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(strMap))
            {
                var strArray = strMap.Split(mapSpriter);
                foreach (var str in strArray)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        var strArray2 = str.Split(keyValueSpriter);
                        if ((strArray2.Length == 2) && !dictionary.ContainsKey(strArray2[0]))
                        {
                            dictionary.Add(strArray2[0].Trim(), strArray2[1].Trim());
                        }
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// 解析四维向量
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <param name="vectorSpriter"></param>
        /// <returns></returns>
        public static bool ParseVector4(string _inputString, out Vector4 result, char vectorSpriter = ',')
        {
            var str = _inputString.Trim();
            str = str.Replace(FBracket1.ToString(), "");
            str = str.Replace(BBracket1.ToString(), "");
            result = new Vector4();
            try
            {
                var strArray = str.Split(vectorSpriter);
                if (strArray.Length != 4)
                {
                    return false;
                }

                result.x = float.Parse(strArray[0]);
                result.y = float.Parse(strArray[1]);
                result.z = float.Parse(strArray[2]);
                result.w = float.Parse(strArray[3]);
                return true;
            }
            catch (Exception e)
            {
                Log.E(e);
                return false;
            }
        }

        /// <summary>
        /// 解析四元数
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <param name="spriter"></param>
        /// <returns></returns>
        public static bool ParseQuaternion(string _inputString, out Quaternion result, char spriter = ',')
        {
            var vec = new Vector4();
            var flag = ParseVector4(_inputString, out vec, spriter);
            result = new Quaternion(vec.x, vec.y, vec.z, vec.w);
            return flag;
        }

        /// <summary>
        /// 解析三维向量
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <param name="spriter"></param>
        /// <returns></returns>
        public static bool ParseVector3(string _inputString, out Vector3 result, char spriter = ',')
        {
            var str = _inputString.Trim();
            str = str.Replace(FBracket1.ToString(), "");
            str = str.Replace(BBracket1.ToString(), "");
            result = new Vector3();
            try
            {
                var strArray = str.Split(spriter);
                if (strArray.Length != 3)
                {
                    return false;
                }

                result.x = float.Parse(strArray[0]);
                result.y = float.Parse(strArray[1]);
                result.z = float.Parse(strArray[2]);
                return true;
            }
            catch (Exception e)
            {
                Log.E(e);
                return false;
            }
        }

        /// <summary>
        /// 解析二维向量
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <param name="spriter"></param>
        /// <returns></returns>
        public static bool ParseVector2(string _inputString, out Vector2 result, char spriter = ',')
        {
            var str = _inputString.Trim();
            str = str.Replace(FBracket1.ToString(), "");
            str = str.Replace(BBracket1.ToString(), "");
            result = new Vector2();
            try
            {
                var strArray = str.Split(spriter);
                if (strArray.Length != 2)
                {
                    return false;
                }

                result.x = float.Parse(strArray[0]);
                result.y = float.Parse(strArray[1]);
                return true;
            }
            catch (Exception e)
            {
                Log.E(e);
                return false;
            }
        }

        /// <summary>
        /// 解析可解析对象
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ParseFromStringableObject(string str, Type type)
        {
            var methodInfos = type.GetMethods();

            MethodInfo info = null;
            foreach (var method in methodInfos)
            {
                if (info != null) break;
                var attrs = method.GetCustomAttributes(false);
                
                if (attrs.OfType<FromString>().Any())
                {
                    info = method;
                }
            }

            return info.Invoke(null, new object[1] {str});
        }

        #endregion FromString 

        /// <summary>
        /// 从“？~？”的字符串中获取随机数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float GetRandom(this string str)
        {
            var strs = str.Split('~');
            var num1 = strs[0].GetValue<float>();
            var num2 = strs[1].GetValue<float>();
            return str.Length == 1 ? num1 : Random.Range(Mathf.Min(num1, num2), Mathf.Max(num1, num2));
        }

        /// <summary>
        /// 将值转化为字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConverToString(this object value)
        {
            //Debug.logger.Log("ConverToString " + Spriter1 + "  "+ Spriter2);
            if (value == null) return string.Empty;
            var type = value.GetType();
            if (type == typeof(Vector3))
            {
                return FBracket1.ToString() + ((Vector3) value).x + Spriter1 + ((Vector3) value).y +
                       Spriter1 + ((Vector3) value).z + BBracket1;
            }

            if (type == typeof(Vector2))
            {
                return FBracket1.ToString() + ((Vector2) value).x + Spriter1 + ((Vector2) value).y +
                       BBracket1;
            }

            if (type == typeof(Vector4))
            {
                return FBracket1.ToString() + ((Vector4) value).x + Spriter1 + ((Vector4) value).y +
                       Spriter1 + ((Vector4) value).z + Spriter1 + ((Vector4) value).w +
                       BBracket1;
            }

            if (type == typeof(Quaternion))
            {
                return FBracket1.ToString() + ((Quaternion) value).x + Spriter1 + ((Quaternion) value).y +
                       Spriter1 + ((Quaternion) value).z + Spriter1 + ((Quaternion) value).w +
                       BBracket1;
            }

            if (type == typeof(Color))
            {
                return FBracket1.ToString() + ((Color) value).r + Spriter1 + ((Color) value).g +
                       Spriter1 + ((Color) value).b + BBracket1;
            }

            if (type.BaseType == typeof(Enum))
            {
                return Enum.GetName(type, value);
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                var Count = (int) type.GetProperty("Count").GetValue(value, null);
                if (Count == 0) return string.Empty;
                var getIe = type.GetMethod("GetEnumerator");
                var enumerator = getIe.Invoke(value, new object[0]);
                var enumeratorType = enumerator.GetType();
                var moveToNextMethod = enumeratorType.GetMethod("MoveNext");
                var current = enumeratorType.GetProperty("Current");

                var stringBuilder = new StringBuilder();

                while (enumerator != null && (bool) moveToNextMethod.Invoke(enumerator, new object[0]))
                {
                    stringBuilder.Append(Spriter1 + ConverToString(current.GetValue(enumerator, null)));
                }

                return stringBuilder.ToString().ReplaceFirst(Spriter1.ToString(), "");

            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
            {
                var pairKey = type.GetProperty("Key").GetValue(value, null);
                var pairValue = type.GetProperty("Value").GetValue(value, null);

                var keyStr = ConverToString(pairKey);
                var valueStr = ConverToString(pairValue);
                return keyStr + Spriter2 + valueStr;
            }

            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                var Count = (int) type.GetProperty("Count").GetValue(value, null);
                if (Count == 0) return String.Empty;
                var mget = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);

                var stringBuilder = new StringBuilder();

                object item;
                string itemStr;

                for (var i = 0; i < Count - 1; i++)
                {
                    item = mget.Invoke(value, new object[] {i});
                    itemStr = item.ConverToString();
                    stringBuilder.Append(itemStr + Spriter1);
                }

                item = mget.Invoke(value, new object[] {Count - 1});
                itemStr = item.ConverToString();
                stringBuilder.Append(itemStr);

                return stringBuilder.ToString();
            }

            if (type == typeof(ArrayList))
            {
                var builder = new StringBuilder();
                var arrayList = value as ArrayList;
                for (var i = 0; i < arrayList.Count - 1; i++)
                {
                    builder.Append(arrayList[i].ConverToString()).Append(",");
                }

                builder.Append(arrayList[arrayList.Count - 1].ConverToString());
                return builder.ToString();
            }

            if (type == typeof(Hashtable))
            {
                var builder = new StringBuilder();
                var table = value as Hashtable;
                var e = table.Keys.GetEnumerator();
                while (e.MoveNext())
                {
                    var tableKey = e.Current.ConverToString();
                    var tableValue = table[e.Current].ConverToString();
                    builder.Append(tableKey).Append(Spriter2).Append(tableValue).Append(Spriter1);
                }

                builder.Remove(builder.Length - 2, 1);
                return builder.ToString();
            }

            if (type.IsArray)
            {
                var stringBuilder = new StringBuilder();
                var array = value as Array;
                if (array.Length > 0)
                {
                    stringBuilder.Append(ConverToString(array.GetValue(0)));
                    for (var i = 1; i < array.Length; i++)
                    {
                        stringBuilder.Append(Spriter1.ToString());
                        stringBuilder.Append(ConverToString(array.GetValue(i)));
                    }

                    return stringBuilder.ToString();
                }

                return string.Empty;
            }

            if (CanConvertToString(type))
            {
                return ToStringableObjectConvertToString(value, type);
            }

            return value.ToString();
        }


        #region ToString

        public static string Vector2ToString(Vector2 value)
        {
            return FBracket1.ToString() + value.x + Spriter1 + value.y +
                   BBracket1;
        }

        public static string Vector3ToString(Vector3 value)
        {
            return FBracket1.ToString() + value.x + Spriter1 + value.y +
                   Spriter1 + value.z + BBracket1;
        }

        public static string Vector4ToString(Vector4 value)
        {
            return FBracket1.ToString() + value.x + Spriter1 + value.y +
                   Spriter1 + value.z + Spriter1 + value.w +
                   BBracket1;
        }

        public static string ColorToString(Color value)
        {
            return FBracket1.ToString() + value.r + Spriter1 + value.g +
                   Spriter1 + value.b + BBracket1;
        }

        public static string QuaternionToString(Quaternion value)
        {
            return FBracket1.ToString() + value.x + Spriter1 + value.y +
                   Spriter1 + value.z + Spriter1 + value.w +
                   BBracket1;
        }


        /// <summary>
        /// 将列表转换至字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ListConvertToString<T>(this List<T> value, char split1 = ',')
        {
            var type = value.GetType();
            var Count = (int) type.GetProperty("Count").GetValue(value, null);
            if (Count == 0) return String.Empty;
            var mget = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);

            var stringBuilder = new StringBuilder();

            object item;
            string itemStr;

            for (var i = 0; i < Count - 1; i++)
            {
                item = mget.Invoke(value, new object[] {i});
                itemStr = item.ConverToString();
                stringBuilder.Append(itemStr + split1);
            }

            item = mget.Invoke(value, new object[] {Count - 1});
            itemStr = item.ConverToString();
            stringBuilder.Append(itemStr);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 数组转换至字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="split1"></param>
        /// <returns></returns>
        public static string ArrConvertToString(this Array value, char split1 = ',')
        {
            var stringBuilder = new StringBuilder();
            var array = value;
            if (array.Length > 0)
            {
                stringBuilder.Append(ConverToString(array.GetValue(0)));
                for (var i = 1; i < array.Length; i++)
                {
                    stringBuilder.Append(split1.ToString());
                    stringBuilder.Append(ConverToString(array.GetValue(i)));
                }

                return stringBuilder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// 将键值对转换至字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="split1"></param>
        /// <returns></returns>
        public static string KVPConvertToString<K, V>(this KeyValuePair<K, V> value, char split1 = ':')
        {
            var type = value.GetType();
            var pairKey = type.GetProperty("Key").GetValue(value, null);
            var pairValue = type.GetProperty("Value").GetValue(value, null);

            var keyStr = ConverToString(pairKey);
            var valueStr = ConverToString(pairValue);
            return keyStr + Spriter2 + valueStr;
        }

        /// <summary>
        /// 将Dictionary转换至字符串
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="value"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static string DictConvertToString<K, V>(this Dictionary<K, V> value, char split1 = ',',
            char split2 = ':')
        {
            var type = value.GetType();
            var Count = (int) type.GetProperty("Count").GetValue(value, null);
            if (Count == 0) return String.Empty;
            var getIe = type.GetMethod("GetEnumerator");
            var enumerator = getIe.Invoke(value, new object[0]);
            var enumeratorType = enumerator.GetType();
            var moveToNextMethod = enumeratorType.GetMethod("MoveNext");
            var current = enumeratorType.GetProperty("Current");

            var stringBuilder = new StringBuilder();

            while (enumerator != null && (bool) moveToNextMethod.Invoke(enumerator, new object[0]))
            {
                stringBuilder.Append(split1 + ConverToString(current.GetValue(enumerator, null)));
            }

            return stringBuilder.ToString().ReplaceFirst(split1.ToString(), "");
        }

        /// <summary>
        /// 将可转换至字符串的对象转换为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToStringableObjectConvertToString(this object obj, Type type)
        {
            var methodInfos = type.GetMethods();

            MethodInfo info = null;
            foreach (var method in methodInfos)
            {
                if (info != null) break;
                var attrs = method.GetCustomAttributes(false);
                
                if (attrs.OfType<ToString>().Any())
                {
                    info = method;
                }
            }

            return info.Invoke(null, new object[1] {obj}) as string;
        }

        #endregion ToString


        //可转换类型列表
        public static readonly List<Type> convertableTypes = new List<Type>
        {
            typeof(int),
            typeof(string),
            typeof(float),
            typeof(double),
            typeof(byte),
            typeof(long),
            typeof(bool),
            typeof(short),
            typeof(uint),
            typeof(ulong),
            typeof(ushort),
            typeof(sbyte),
            typeof(Vector3),
            typeof(Vector2),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Color),
            typeof(Dictionary<,>),
            typeof(KeyValuePair<,>),
            typeof(List<>),
            typeof(Enum),
            typeof(Array)
        };

        /// <summary>
        /// 通过文本获取类型：
        /// 注意！解析嵌套多泛型类型时会出现问题！
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Type GetTypeByString(this string str)
        {
            str = str.Trim();
            switch (str)
            {
                case "int":
                    return typeof(int);
                case "float":
                    return typeof(float);
                case "string":
                    return typeof(string);
                case "double":
                    return typeof(double);
                case "byte":
                    return typeof(byte);
                case "bool":
                    return typeof(bool);
                case "short":
                    return typeof(short);
                case "uint":
                    return typeof(uint);
                case "ushort":
                    return typeof(ushort);
                case "sbyte":
                    return typeof(sbyte);
                case "Vector3":
                    return typeof(Vector3);
                case "Vector2":
                    return typeof(Vector2);
                case "Vector4":
                    return typeof(Vector4);
                case "Quaternion":
                    return typeof(Quaternion);
                case "Color":
                    return typeof(Color);
            }

            if (str.StartsWith("List"))
            {
                var genType = str.Substring(str.IndexOf('<') + 1, str.IndexOf('>') - str.LastIndexOf('<') - 1)
                    .GetTypeByString();
                return Type.GetType("System.Collections.Generic.List`1[[" + genType.FullName + ", " +
                                    genType.Assembly.FullName + "]], " + typeof(List<>).Assembly.FullName);
            }

            if (str.StartsWith("Dictionary"))
            {
                var typeNames = str.Substring(str.IndexOf('<') + 1, str.IndexOf('>') - str.LastIndexOf('<') - 1)
                    .Split(',');
                var type1 = typeNames[0].Trim().GetTypeByString();
                var type2 = typeNames[1].Trim().GetTypeByString();
                var typeStr = "System.Collections.Generic.Dictionary`2[[" + type1.FullName + ", " +
                                 type1.Assembly.FullName + "]" +
                                 ",[" + type2.FullName + ", " + type2.Assembly.FullName + "]], " +
                                 typeof(Dictionary<,>).Assembly.FullName;
                return Type.GetType(typeStr);
            }

            //仅支持内置类型,支持多维数组
            if (str.Contains("[") && str.Contains("]"))
            {
                var itemTypeStr = str.Substring(0, str.IndexOf('['));
                var bracketStr = str.Substring(str.IndexOf('['), str.LastIndexOf(']') - str.IndexOf('[') + 1);
                var itemType = itemTypeStr.GetTypeByString();
                var typeStr = itemType.FullName + bracketStr;
                return Type.GetType(typeStr);
            }

            return Type.GetType(str);
        }

        /// <summary>
        /// 是否为可转换字符串的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConvertableType(this Type type)
        {
            return CanConvertFromString(type) && CanConvertToString(type) || convertableTypes.Contains(type);
        }

        /// <summary>
        /// 是否可以从String中转换出来
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanConvertFromString(this Type type)
        {
            var methodInfos = type.GetMethods();
            return methodInfos.Select(method => method.GetCustomAttributes(false)).Any(attrs => attrs.OfType<FromString>().Any());
        }

        /// <summary>
        /// 是否可以转换为String
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanConvertToString(this Type type)
        {
            var methodInfos = type.GetMethods();
            return methodInfos.SelectMany(method => method.GetCustomAttributes(false)).OfType<ToString>().Any();
        }

        /// <summary>
        /// 替换第一个匹配值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="startAt"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string input, string oldValue, string newValue, int startAt = 0)
        {
            var index = input.IndexOf(oldValue, startAt);
            if (index < 0)
            {
                return input;
            }

            return (input.Substring(0, index) + newValue + input.Substring(index + oldValue.Length));
        }

        /// <summary>
        /// 是否存在中文字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasChinese(this string input)
        {
            return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 是否存在空格
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasSpace(this string input)
        {
            return input.Contains(" ");
        }

        /// <summary>
        /// 将一系列的对象转换为字符串并且以符号分割
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static string Join<T>(this IEnumerable<T> source, string sp)
        {
            var result = new StringBuilder();
            var first = true;
            foreach (T item in source)
            {
                if (first)
                {
                    first = false;
                    result.Append(item.ConverToString());
                }
                else
                {
                    result.Append(sp).Append(item.ConverToString());
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 扩展方法来判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyR(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 删除特定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string RemoveString(this string str, params string[] targets)
        {
            return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
        }

        /// <summary>
        /// 拆分并去除空格
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitAndTrim(this string str, params char[] separator)
        {
            var res = str.Split(separator);
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = res[i].Trim();
            }

            return res;
        }

        /// <summary>
        /// 查找在两个字符串中间的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="front"></param>
        /// <param name="behined"></param>
        /// <returns></returns>
        public static string FindBetween(this string str, string front, string behined)
        {
            var startIndex = str.IndexOf(front) + front.Length;
            var endIndex = str.IndexOf(behined);
            if (startIndex < 0 || endIndex < 0)
                return str;
            return str.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 查找在字符后面的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="front"></param>
        /// <returns></returns>
        public static string FindAfter(this string str, string front)
        {
            var startIndex = str.IndexOf(front) + front.Length;
            return startIndex < 0 ? str : str.Substring(startIndex);
        }
    }
}