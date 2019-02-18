using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using QFramework.Json;
using uFrame.Serialization;

public static class SerializationExtensions{



        public static T DeserializeObject<T>(string json)
        {
            return DeserializeObject<T>(JSON.Parse(json));
        }

        public static T DeserializeObject<T>(JSONNode node)
        {
            return (T) DeserializeObject(typeof (T), node);
        }

        public static object DeserializeObject(Type type, JSONNode node)
        {
            //Handle special types first

            if (type == typeof (Guid)) return new Guid(node.Value);


            if (type == typeof(int)) return node.AsInt;
            if (type == typeof (Vector2)) return node.AsVector2;
            if (type == typeof (Vector3)) return node.AsVector3;
            if (type == typeof (string)) return node.Value;
            if (type == typeof (bool)) return node.AsBool;
            if (type == typeof (float)) return node.AsFloat;
            if (type == typeof(double)) return node.AsDouble;
            
            //if none of above works, check for collection

            if (typeof(IList).IsAssignableFrom(type)) // collection detected
            {
                var jsonArray = node as JSONArray;
                if (jsonArray == null) return null;

                var itemType = type.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(new [] { itemType } );
                var list = (IList)System.Activator.CreateInstance(listType);

                foreach (var instance in jsonArray.Childs)
                {
                    list.Add(DeserializeObject(itemType,instance));
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

            var poco = System.Activator.CreateInstance(type);

            var properties = type.GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (jsonClass[propertyInfo.Name] != null)
                {
                    var deserializedObject = DeserializeObject(propertyInfo.PropertyType, jsonClass[propertyInfo.Name]);
                    propertyInfo.SetValue(poco,deserializedObject,null);
                }
            }

            return poco;
        }
        



        public static JSONNode SerializeObject(object target)
        {
            if(target == null) return new JSONNode();
          
            var objectType = target.GetType();


            //Handle special types first
            if (objectType == typeof (Guid))
            {
                return new JSONData(target.ToString());
            }

            if (typeof(IList).IsAssignableFrom(objectType)) // collection detected
            {

                //var type = objectType.GetGenericArguments()[0];
                var list = target as IEnumerable;
                var jsonArray = new JSONArray();

                foreach (var instance in list)
                {
                    jsonArray.Add(SerializeObject(instance));
                }

                return jsonArray;
            }

            //primitive detection
            
            var node = SerializePrimitive(target);
            if (node != null){
                return node;
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
                result.Add(propertyInfo.Name, SerializeObject(propertyInfo.GetValue(target, null)));
            }

            return result;

        }

        private static JSONNode SerializePrimitive(object value)
        {
            JSONNode node = null;
            var type = value.GetType();
            if (type == typeof (int))           node = new JSONData((int)value);
            else if (type == typeof(Vector2))   node = new JSONClass() { AsVector2 = (Vector2)value };
            else if (type == typeof(Vector3))   node = new JSONClass() { AsVector3 = (Vector3)value }; 
            else if (type == typeof (string))   node = new JSONData((string) value);
            else if (type == typeof (bool))     node = new JSONData((bool)value);
            else if (type == typeof (float))    node = new JSONData((float)value);
            else if (type == typeof (double))   node = new JSONData((double)value);
            return node;
        }


}
