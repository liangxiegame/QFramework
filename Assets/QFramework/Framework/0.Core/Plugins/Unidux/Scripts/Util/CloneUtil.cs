using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Unidux.Util
{
    public static class CloneUtil
    {
        public static object MemoryClone(object clonee)
        {
            return MemoryClone(clonee, CreateDefaultSurrogateSelector());
        }

        public static object MemoryClone(object clonee, SurrogateSelector selector)
        {
            object result;
            IFormatter formatter = new BinaryFormatter();
            formatter.SurrogateSelector = selector;

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    formatter.Serialize(stream, clonee);
                    stream.Position = 0;
                    result = formatter.Deserialize(stream);
                }
                finally
                {
                    stream.Close();
                }
            }

            return result;
        }

        public static TInstance CopyEntity<TInstance>(TInstance thisInstance, TInstance newInstance)
            where TInstance : class
        {
            if (thisInstance == null || newInstance == null)
            {
                return null;
            }

            var type = thisInstance.GetType();

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var thisValue = field.GetValue(thisInstance);
                field.SetValue(newInstance, ObjectClone(thisValue));
            }

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var thisValue = property.GetValue(thisInstance, null);
                    property.SetValue(newInstance, ObjectClone(thisValue), null);
                }
            }

            return newInstance;
        }

        public static object ObjectClone(object instance)
        {
            if (instance == null)
            {
                return null;
            }

            if (instance is ICloneable)
            {
                return ((ICloneable) instance).Clone();
            }

            var type = instance.GetType();

            if (type.IsPrimitive || type.IsEnum || type.IsValueType || type == typeof(string))
            {
                return instance;
            }

            if (type.IsArray)
            {
                return ArrayClone((IList)instance);
            }

            if (instance is IList)
            {
                return ListClone((IList) instance);
            }

            if (instance is IDictionary)
            {
                return DictionaryClone((IDictionary) instance);
            }

            var newInstance = Activator.CreateInstance(type);
            return CopyEntity(instance, newInstance);
        }

        public static TValue DictionaryClone<TValue>(TValue instance) where TValue : IDictionary
        {
            var type = instance.GetType();

            var dict = (IDictionary) instance;
            var newDict = (TValue) Activator.CreateInstance(type);

            foreach (var key in dict.Keys)
            {
                newDict[key] = ObjectClone(dict[key]);
            }

            return newDict;
        }

        public static Array ArrayClone(IList array)
        {
            var newArray = Array.CreateInstance(array.GetType().GetElementType(), array.Count);
            
            for (var i = 0; i < array.Count; i++)
            {
                newArray.SetValue(ObjectClone(array[i]), i);
            }
            
            return newArray;
        }

        public static TValue ListClone<TValue>(TValue instance) where TValue : IList
        {
            var type = instance.GetType();
            var list = (IList) instance;

            IList newList = (IList) Activator.CreateInstance(type);

            foreach (var value in list)
            {
                newList.Add(ObjectClone(value));
            }

            return (TValue) newList;
        }

        public static SurrogateSelector CreateDefaultSurrogateSelector()
        {
            SurrogateSelector selector = new SurrogateSelector();
            selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All),
                new Vector2SerializationSurrogate());
            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All),
                new Vector3SerializationSurrogate());
            selector.AddSurrogate(typeof(Vector4), new StreamingContext(StreamingContextStates.All),
                new Vector4SerializationSurrogate());
            selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All),
                new ColorSerializationSurrogate());
            return selector;
        }
    }
}