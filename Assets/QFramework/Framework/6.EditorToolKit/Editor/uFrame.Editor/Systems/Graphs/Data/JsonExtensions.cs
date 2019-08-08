using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using QF.Json;
using UnityEngine;

namespace QF
{
    public static class JsonExtensions
    {

        public static T DeserializeObject<T>(string json)
        {
            return DeserializeObject<T>(Json.JSON.Parse(json));
        }

        public static T DeserializeObject<T>(JSONNode node)
        {
            return (T)DeserializeObject(typeof(T), node);
        }

        public static object DeserializeObject(Type type, JSONNode node)
        {
            //Handle special types first

            if (type == typeof(Guid)) return new Guid(node.Value);


            if (type == typeof(int)) return node.AsInt;
            if (type == typeof(Vector2)) return node.AsVector2;
            if (type == typeof(Vector3)) return node.AsVector3;
            if (type == typeof(string)) return node.Value;
            if (type == typeof(bool)) return node.AsBool;
            if (type == typeof(float)) return node.AsFloat;
            if (type == typeof(double)) return node.AsDouble;

            //if
            if (type == typeof(DateTime))
            {
                return DateTime.Parse(node.Value);
            }

            //if none of above works, check for collection
            if (typeof(IList).IsAssignableFrom(type)) // collection detected
            {
                var jsonArray = node as JSONArray;
                if (jsonArray == null) return null;

                var itemType = type.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(new[] { itemType });
                var list = (IList)System.Activator.CreateInstance(listType);

                foreach (var instance in jsonArray.Childs)
                {
                    list.Add(DeserializeObject(itemType, instance));
                }

                return list;
            }

            //try enum detection

            if (type.IsEnum)
            {
                return Enum.ToObject(type, node.AsInt);
            }

            //If enum is not the case, POCO-TIME!

            var jsonClass = node as JSONClass;
            if (jsonClass == null) return null;

            var poco = DeserializeExistingObject(Activator.CreateInstance(type), jsonClass);

            return poco;
        }

        public static object DeserializeExistingObject(object poco, JSONClass jsonClass)
        {


            var properties = poco.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.IsDefined(typeof(JsonProperty), true) || propertyInfo.Name == "Identifier")
                {
                    if (jsonClass[propertyInfo.Name] != null)
                    {
                        var deserializedObject = DeserializeObject(propertyInfo.PropertyType, jsonClass[propertyInfo.Name]);
                        propertyInfo.SetValue(poco, deserializedObject, null);
                    }
                }
            }
            return poco;
        }

        public static JSONNode SerializeObject(object target)
        {
            if (target == null) return new JSONNode();

            var objectType = target.GetType();


            //Handle special types first
            if (objectType == typeof(Guid))
            {
                return new JSONData(target.ToString());
            }
            //primitive detection
            var node = SerializePrimitive(target);
            if (node != null)
            {
                return node;
            }

            //if
            if (objectType == typeof(DateTime))
            {
                var t = (DateTime)target;
                return new JSONData(t.ToString());
            }


            if (typeof(IList).IsAssignableFrom(objectType)) // collection detected
            {

                //var type = objectType.GetGenericArguments()[0];
                var list = target as IEnumerable;
                var jsonArray = new JSONArray();

                if (list != null)
                    foreach (var instance in list)
                    {
                        jsonArray.Add(SerializeObject(instance));
                    }

                return jsonArray;
            }





            //enum detection

            if (objectType.IsEnum)
            {
                return new JSONData((int)target);
            }


            //poco-like detection

            JSONClass result = new JSONClass();

            var properties = target.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.IsDefined(typeof(JsonProperty), true) || propertyInfo.Name == "Identifier")
                {
                    var value = propertyInfo.GetValue(target, null);
                    if (value != null)
                    {
                        result.Add(propertyInfo.Name, SerializeObject(value));
                    }

                }

            }

            return result;

        }

        private static JSONNode SerializePrimitive(object value)
        {
            JSONNode node = null;
            var type = value.GetType();
            if (type == typeof(int)) node = new JSONData((int)value);
            else if (type == typeof(Vector2)) node = new JSONClass() { AsVector2 = (Vector2)value };
            else if (type == typeof(Vector3)) node = new JSONClass() { AsVector3 = (Vector3)value };
            else if (type == typeof(string)) node = new JSONData((string)value);
            else if (type == typeof(bool)) node = new JSONData((bool)value);
            else if (type == typeof(float)) node = new JSONData((float)value);
            else if (type == typeof(double)) node = new JSONData((double)value);
            return node;
        }

    }
//    public static class JsonExtensions
//    {
//        public static void DeserializeProperty(this object obj, PropertyInfo property, JSONClass cls)
//        {
//            var propertyName = property.Name;
//            if (cls[propertyName] == null) return;
//            var propertyType = property.PropertyType;
//            if (typeof(IJsonObject).IsAssignableFrom(propertyType))
//            {
//                var value = cls[propertyName].DeserializeObject(null, false);
//                property.SetValue(obj, value, null);
//            }
//            if (typeof(Enum).IsAssignableFrom(property.PropertyType))
//            {
//                var value = cls[propertyName].Value;
//                property.SetValue(obj, Enum.Parse(propertyType, value), null);
//            }
//            else if (propertyType == typeof(int))
//            {
//                property.SetValue(obj, cls[propertyName].AsInt, null);
//            }
//            else if (propertyType == typeof(string))
//            {
//                property.SetValue(obj, cls[propertyName].Value, null);
//            }
//            else if (propertyType == typeof(float))
//            {
//                property.SetValue(obj, cls[propertyName].AsFloat, null);
//            }
//            else if (propertyType == typeof(bool))
//            {
//                property.SetValue(obj, cls[propertyName].AsBool, null);
//            }
//            else if (propertyType == typeof(double))
//            {
//                property.SetValue(obj, cls[propertyName].AsDouble, null);
//            }
//            else if (propertyType == typeof(Vector2))
//            {
//                property.SetValue(obj, cls[propertyName].AsVector2, null);
//            }
//            else if (propertyType == typeof(Vector3))
//            {
//                property.SetValue(obj, cls[propertyName].AsVector3, null);
//            }
//#if UNITY_EDITOR
//            else if (propertyType == typeof(Quaternion))
//            {
//                property.SetValue(obj, cls[propertyName].AsQuaternion, null);
//            }
//#endif
//            else if (propertyType == typeof(Color))
//            {
//                property.SetValue(obj, (Color)cls[propertyName].AsVector4, null);
//            }
//        }
//        public static void SerializeProperty(this object obj, PropertyInfo property, JSONClass cls)
//        {
//            var value = property.GetValue(obj, null);
//            if (value != null)
//            {
//                var propertyName = property.Name;
//                var propertyType = property.PropertyType;
//                if (typeof(IJsonObject).IsAssignableFrom(propertyType))
//                {
//                    cls.Add(propertyName, SerializeObject(((IJsonObject)value)));
//                }
//                else if (typeof(Enum).IsAssignableFrom(propertyType))
//                {
//                    cls.Add(propertyName, new JSONData(value.ToString()));
//                }
//                else if (propertyType == typeof(int))
//                {
//                    cls.Add(propertyName, new JSONData((int)value));
//                }
//                else if (propertyType == typeof(string))
//                {
//                    cls.Add(propertyName, new JSONData((string)value));
//                }
//                else if (propertyType == typeof(float))
//                {
//                    cls.Add(propertyName, new JSONData((float)value));
//                }
//                else if (propertyType == typeof(bool))
//                {
//                    cls.Add(propertyName, new JSONData((bool)value));
//                }
//                else if (propertyType == typeof(double))
//                {
//                    cls.Add(propertyName, new JSONData((double)value));
//                }
//                else if (propertyType == typeof(Color))
//                {
//                    var vCls = new JSONClass();
//                    var color = (Color)value;
//                    vCls.AsVector4 = new Vector4(color.r, color.g, color.b, color.a);
//                    cls.Add(propertyName, vCls);
//                }
//                else if (propertyType == typeof(Vector2))
//                {
//                    var vCls = new JSONClass();
//                    vCls.AsVector2 = (Vector2)value;
//                    cls.Add(propertyName, vCls);
//                }
//                else if (propertyType == typeof(Vector3))
//                {
//                    var vCls = new JSONClass();
//                    vCls.AsVector3 = (Vector3)value;
//                    cls.Add(propertyName, vCls);
//                }
//#if UNITY_EDITOR
//                else if (propertyType == typeof(Quaternion))
//                {
//                    var vCls = new JSONClass();
//                    vCls.AsQuaternion = (Quaternion)value;
//                    cls.Add(propertyName, vCls);
//                }
//#endif
//                else
//                {
//                    throw new Exception(
//                        string.Format("{0} property can't be serialized. Override Serialize method to serialize it.",
//                            propertyName));
//                }
//            }
//        }
//        public static void AddObject(this JSONClass cls, string name, IJsonObject jsonObject)
//        {
//            if (jsonObject != null)
//                cls.Add(name, SerializeObject(jsonObject));
//        }
//        public static IEnumerable<T> DeserializePrimitiveArray<T>(this JSONNode array, Func<JSONNode, T> deserialize)
//        {
//            if (array == null) yield break;
//            foreach (JSONNode item in array.AsArray)
//            {
//                yield return deserialize(item);
//            }
//        }
//        public static IEnumerable<T> DeserializePrimitiveArray<T>(this JSONArray array, Func<JSONNode, T> deserialize)
//        {
//            if (array == null) yield break;
//            foreach (JSONNode item in array)
//            {
//                yield return deserialize(item);
//            }
//        }
//        public static void AddPrimitiveArray<T>(this JSONClass cls, string name, IEnumerable<T> arr, Func<T, JSONNode> serializeItem)
//        {
//            var jsonArray = new JSONArray();
//            foreach (var item in arr)
//            {
//                jsonArray.Add(serializeItem(item));
//            }
//            cls.Add(name, jsonArray);
//        }
//        public static void AddObjectArray<T>(this JSONClass cls, string name, IEnumerable<T> array) where T : IJsonObject
//        {
//            if (array == null) return;
//            cls.Add(name, SerializeObjectArray(array));
//        }
//        public static JSONArray SerializeObjectArray<T>(this IEnumerable<T> array) where T : IJsonObject
//        {
//            var jsonArray = new JSONArray();
//            foreach (var item in array)
//            {
//                jsonArray.Add(item.SerializeObject());
//            }
//            return jsonArray;
//        }
//        public static JSONClass SerializeObject(this IJsonObject obj)
//        {
//            var cls = new JSONClass() { { "_CLRType", obj.GetType().AssemblyQualifiedName } };
//            obj.Serialize(cls);
//            return cls;
//        }
//        public static IEnumerable<T> DeserializeObjectArray<T>(this JSONNode array)
//        {
//            return array.AsArray.DeserializeObjectArray<T>(null);
//        }
//        public static IEnumerable<T> DeserializeObjectArray<T>(this JSONArray array, Func<JSONNode, T> missingTypeHandler)
//        {
//            foreach (JSONNode item in array)
//            {
//                var obj = DeserializeObject(item, null, missingTypeHandler == null);
//                if (obj != null)
//                    yield return (T)obj;
//                else if (missingTypeHandler != null)
//                {
//                    yield return missingTypeHandler(item);
//                }
//            }
//        }
//        public static Type FindType(string name)
//        {
//            //if (string.IsNullOrEmpty(name)) return null;

//            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//            {
//                // if (!assembly.FullName.StartsWith("Invert") && !assembly.FullName.StartsWith("Syste")) continue;
//                var t = assembly.GetType(name);
//                if (t != null)
//                {
//                    return t;
//                }
//            }
//            return null;
//        }

//        public static Type FindTypeByName(string name)
//        {
//            //if (string.IsNullOrEmpty(name)) return null;

//            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//            {
//                // if (!assembly.FullName.StartsWith("Invert") && !assembly.FullName.StartsWith("Syste")) continue;
//                try
//                {
//                    foreach (var item in assembly.GetTypes())
//                    {
//                        if (item.Name == name)
//                            return item;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    continue;
//                }

//            }
//            return null;
//        }
//        public static IJsonTypeResolver TypeResolver { get; set; }
//        public static IJsonObject DeserializeObject(this JSONNode node, Type genericTypeArg = null, bool failOnMissingType = true)
//        {
//            if (node == null) return null;
//            var clrTypeString = node["_CLRType"].Value;
//            if (string.IsNullOrEmpty(clrTypeString))
//            {
//                throw new Exception("CLR Type is null can't load the type");

//            }
//            var clrType = Type.GetType(clrTypeString);
//            if (clrType == null)
//            {
//                clrType = FindType(clrTypeString);
//            }
//            if (clrType == null)
//            {
//                clrType = FindTypeByName(clrTypeString);
//            }
//            if (clrType == null && TypeResolver != null)
//            {
//                clrType = TypeResolver.FindType(clrTypeString);
//            }
//            if (clrType == null)
//            {
//                if (!failOnMissingType)
//                {
//                    return null;
//                }
//                else
//                {
//                    throw new Exception("Could not find type " + clrTypeString);
//                }
//            }

//            if (clrType.IsGenericTypeDefinition && genericTypeArg != null)
//            {
//                clrType = clrType.MakeGenericType(genericTypeArg);
//            }
//            else if (clrType.IsGenericTypeDefinition)
//            {
//                return null;
//            }
//            var obj = Activator.CreateInstance(clrType) as IJsonObject;
//            if (obj != null)
//            {
//                obj.Deserialize(node as JSONClass);
//                return obj;
//            }

//            throw new Exception("Type must be of type IJsonObject" + clrTypeString);

//        }
//    }
}