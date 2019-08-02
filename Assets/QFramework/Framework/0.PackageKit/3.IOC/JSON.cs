//#define USE_SharpZipLib

/* * * * *
 * \dontinclude SimpleJSON.cs
 * A simple JSON Parser / builder
 * ------------------------------
 *
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 *
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 *
 * Written by Bunny83
 * 2012-06-09
 *
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JSONArray), objects(JSONClass) and values(JSONData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed JSON is parsed the result is undefined
 *
 *
 * 2012-12-17 Update:
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 *
 * * * * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.Json 
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonProperty : Attribute
    {

    }
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
            if (jsonClass == null) return null;

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
    public static class JSON
    {
        public static JSONNode Parse(string aJSON)
        {
            return JSONNode.Parse(aJSON);
        }
    }
    public enum JSONBinaryTag
    {
        Array = 1,
        Class = 2,
        Value = 3,
        IntValue = 4,
        DoubleValue = 5,
        BoolValue = 6,
        FloatValue = 7,
    }
    public class JSONClass : JSONNode, IEnumerable
    {
        private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                    yield return N.Value;
            }
        }

        public override int Count
        {
            get { return m_Dict.Count; }
        }

        public override JSONNode this[string aKey]
        {
            get
            {
                if (m_Dict.ContainsKey(aKey))
                    return m_Dict[aKey];
                else
                    return new JSONLazyCreator(this, aKey);
            }
            set
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey, value);
            }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return null;
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = aItem;
                else
                    m_Dict.Add(aKey, aItem);
            }
            else
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                yield return N;
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
            JSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            var item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                var item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Class);
            aWriter.Write(m_Dict.Count);
            foreach (string K in m_Dict.Keys)
            {
                aWriter.Write(K);
                m_Dict[K].Serialize(aWriter);
            }
        }

        public override string ToString()
        {
            return ToString(false);
        }
        public override string ToString(bool isRoot)
        {

            if (isRoot)
            {
                TabIndex = 0;
            }
            string result = TabString + "{";
            result += Environment.NewLine;
            TabIndex++;
            var i = 0;
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (i > 0)
                    result += TabString + ", " + Environment.NewLine;
                result += TabString + string.Format("\"{0}\":{1}", Escape(N.Key), N.Value.ToString());
                i++;
            }
            TabIndex--;
            result += TabString + "}" + Environment.NewLine;
            return result;
        }

        public override string ToString(string aPrefix)
        {
            string result = "{ ";
            result += Environment.NewLine;
            TabIndex++;
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (result.Length > 3)
                    result += ", ";
                result += string.Format("\n{0}   ", aPrefix);
                result += string.Format("\"{0}\" : {1}", Escape(N.Key), N.Value.ToString(aPrefix + "   "));
            }
            result += string.Format("\n{0}}}", aPrefix);
            TabIndex--;
            result += "}" + Environment.NewLine;
            return result;
        }
    }
    public class JSONData : JSONNode
    {
        private string m_Data;

        public override string Value
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public JSONData(Vector3 value)
        {
            AsVector3 = value;
        }
        public JSONData(Vector2 value)
        {
            AsVector2 = value;
        }
#if UNITY
        public JSONData(Quaternion value)
        {
            AsQuaternion = value;
        }
#endif

        public JSONData(string aData)
        {
            m_Data = aData;
        }

        public JSONData(string aData, bool forceString)
        {
            m_Data = aData;
            _forceString = forceString;
        }

        private bool _forceString { get; set; }

        public JSONData(float aData)
        {
            AsFloat = aData;
        }

        public JSONData(double aData)
        {
            AsDouble = aData;
        }

        public JSONData(bool aData)
        {
            AsBool = aData;
        }

        public JSONData(int aData)
        {
            AsInt = aData;
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            var tmp = new JSONData("");

            tmp.AsInt = AsInt;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.IntValue);
                aWriter.Write(AsInt);
                return;
            }
            tmp.AsFloat = AsFloat;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.FloatValue);
                aWriter.Write(AsFloat);
                return;
            }
            tmp.AsDouble = AsDouble;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.DoubleValue);
                aWriter.Write(AsDouble);
                return;
            }

            tmp.AsBool = AsBool;
            if (tmp.m_Data == this.m_Data)
            {
                aWriter.Write((byte)JSONBinaryTag.BoolValue);
                aWriter.Write(AsBool);
                return;
            }
            aWriter.Write((byte)JSONBinaryTag.Value);
            aWriter.Write(m_Data);
        }

        public override string ToString()
        {

            if (_forceString)
            {
                return string.Format("\"{0}\"", Escape(m_Data));
            }

            var tmp = new JSONData("");

            tmp.AsInt = AsInt;

            if (tmp.m_Data == this.m_Data)
            {
                return string.Format("{0}", Escape(m_Data));
            }

            tmp.AsBool = AsBool;

            if (tmp.m_Data == this.m_Data)
            {
                return string.Format("{0}", Escape(m_Data));
            }

            return string.Format("\"{0}\"", Escape(m_Data));
        }

        public override string ToString(string aPrefix)
        {



            if (_forceString)
            {
                return string.Format("\"{0}\"", Escape(m_Data));
            }

            var tmp = new JSONData("");
            tmp.AsInt = AsInt;

            if (tmp.m_Data == this.m_Data)
            {
                return string.Format("{0}", Escape(m_Data));
            }

            tmp.AsBool = AsBool;

            if (tmp.m_Data == this.m_Data)
            {
                return string.Format("{0}", Escape(m_Data));
            }

            return string.Format("\"{0}\"", Escape(m_Data));
        }
    }
    public class JSONLazyCreator : JSONNode
    {
        private string m_Key = null;
        private JSONNode m_Node = null;

        public override JSONArray AsArray
        {
            get
            {
                JSONArray tmp = new JSONArray();
                Set(tmp);
                return tmp;
            }
        }

        public override bool AsBool
        {
            get
            {
                JSONData tmp = new JSONData(false);
                Set(tmp);
                return false;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override double AsDouble
        {
            get
            {
                JSONData tmp = new JSONData(0.0);
                Set(tmp);
                return 0.0;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override float AsFloat
        {
            get
            {
                JSONData tmp = new JSONData(0.0f);
                Set(tmp);
                return 0.0f;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override int AsInt
        {
            get
            {
                JSONData tmp = new JSONData(0);
                Set(tmp);
                return 0;
            }
            set
            {
                JSONData tmp = new JSONData(value);
                Set(tmp);
            }
        }

        public override JSONClass AsObject
        {
            get
            {
                JSONClass tmp = new JSONClass();
                Set(tmp);
                return tmp;
            }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                return new JSONLazyCreator(this);
            }
            set
            {
                var tmp = new JSONArray();
                tmp.Add(value);
                Set(tmp);
            }
        }

        public override JSONNode this[string aKey]
        {
            get
            {
                return new JSONLazyCreator(this, aKey);
            }
            set
            {
                var tmp = new JSONClass();
                tmp.Add(aKey, value);
                Set(tmp);
            }
        }

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }

        public static bool operator ==(JSONLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public override void Add(JSONNode aItem)
        {
            var tmp = new JSONArray();
            tmp.Add(aItem);
            Set(tmp);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            var tmp = new JSONClass();
            tmp.Add(aKey, aItem);
            Set(tmp);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return System.Object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "";
        }

        public override string ToString(string aPrefix)
        {
            return "";
        }

        private void Set(JSONNode aVal)
        {
            if (m_Key == null)
            {
                m_Node.Add(aVal);
            }
            else
            {
                m_Node.Add(m_Key, aVal);
            }
            m_Node = null; // Be GC friendly.
        }
    }
    public class JSONNode
    {
        public static int TabIndex { get; set; }

        public static string TabString
        {
            get
            {
                var str = "";
                for (var i = 0; i < TabIndex; i++)
                {
                    str += "\t";
                }
                return str;
            }
        }
        #region common interface

        public virtual IEnumerable<JSONNode> Childs { get { yield break; } }

        public virtual int Count { get { return 0; } }

        public IEnumerable<JSONNode> DeepChilds
        {
            get
            {
                foreach (var C in Childs)
                    foreach (var D in C.DeepChilds)
                        yield return D;
            }
        }

        public virtual string Value { get { return ""; } set { } }

        public virtual JSONNode this[int aIndex] { get { return new JSONNode(); } set { } }

        public virtual JSONNode this[string aKey] { get { return new JSONNode(); } set { } }

        public virtual void Add(string aKey, JSONNode aItem)
        {
        }

        public virtual void Add(JSONNode aItem)
        {
            Add("", aItem);
        }

        public virtual JSONNode Remove(string aKey)
        {
            return null;
        }

        public virtual JSONNode Remove(int aIndex)
        {
            return null;
        }

        public virtual JSONNode Remove(JSONNode aNode)
        {
            return aNode;
        }

        public virtual string ToString(bool isRoot)
        {
            return ToString();
        }
        public override string ToString()
        {
            return "JSONNode";
        }

        public virtual string ToString(string aPrefix)
        {
            return "JSONNode";
        }

        #endregion common interface

        #region typecasting properties

        public virtual JSONArray AsArray
        {
            get
            {
                return this as JSONArray ?? new JSONArray();
            }
        }

        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value) ? "true" : "false";
            }
        }

        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value, out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString();
            }
        }

        public virtual float AsFloat
        {
            get
            {
                float v = 0.0f;
                if (Value == null) return 0.0f;
                if (float.TryParse(Value, out v))
                    return v;
                return 0.0f;
            }
            set
            {
                Value = value.ToString();
            }
        }

        public virtual int AsInt
        {
            get
            {
                int v = 0;
                if (int.TryParse(Value, out v))
                    return v;
                return 0;
            }
            set
            {
                Value = value.ToString();
            }
        }

        public virtual JSONClass AsObject
        {
            get
            {
                return this as JSONClass;
            }
        }
#if !PORTABLE && !DLL
        public virtual Quaternion AsQuaternion
        {
            get
            {
                return Quaternion.Euler(AsVector3);
            }
            set
            {
                var euler = value.eulerAngles;
                var ob = this.AsObject;
                ob.Add("x", new JSONData(euler.x));
                ob.Add("y", new JSONData(euler.y));
                ob.Add("z", new JSONData(euler.z));
            }
        }
#endif
        public virtual Vector2 AsVector2
        {
            get
            {
                var cl = this as JSONClass;
                return new Vector2(cl["x"].AsFloat, cl["y"].AsFloat);
            }
            set
            {
                var ob = this.AsObject;
                ob.Add("x", new JSONData(value.x));
                ob.Add("y", new JSONData(value.y));
            }
        }

        public virtual Vector3 AsVector3
        {
            get
            {
                return new Vector3(this["x"].AsFloat, this["y"].AsFloat, this["z"].AsFloat);
            }
            set
            {
                var ob = this.AsObject;
                ob.Add("x", new JSONData(value.x));
                ob.Add("y", new JSONData(value.y));
                ob.Add("z", new JSONData(value.z));
                //this["x"].AsFloat = value.x;
                //this["y"].AsFloat = value.x;
                //this["z"].AsFloat = value.x;
            }
        }

        public virtual Vector4 AsVector4
        {
            get
            {
                var cl = this as JSONClass;
                return new Vector4(cl["x"].AsFloat, cl["y"].AsFloat, cl["z"].AsFloat, cl["w"].AsFloat);
            }
            set
            {
                var ob = this.AsObject;
                ob.Add("x", new JSONData(value.x));
                ob.Add("y", new JSONData(value.y));
                ob.Add("z", new JSONData(value.z));
                ob.Add("w", new JSONData(value.w));
            }
        }

        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string s)
        {
            return new JSONData(s);
        }

        public static implicit operator string(JSONNode d)
        {
            return (d == null) ? null : d.Value;
        }

        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }

        public static bool operator ==(JSONNode a, object b)
        {
            if (b == null && a is JSONLazyCreator)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public override bool Equals(object obj)
        {
            return System.Object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators

        public static JSONNode Deserialize(System.IO.BinaryReader aReader)
        {
            JSONBinaryTag type = (JSONBinaryTag)aReader.ReadByte();
            switch (type)
            {
                case JSONBinaryTag.Array:
                    {
                        int count = aReader.ReadInt32();
                        JSONArray tmp = new JSONArray();
                        for (int i = 0; i < count; i++)
                            tmp.Add(Deserialize(aReader));
                        return tmp;
                    }
                case JSONBinaryTag.Class:
                    {
                        int count = aReader.ReadInt32();
                        JSONClass tmp = new JSONClass();
                        for (int i = 0; i < count; i++)
                        {
                            string key = aReader.ReadString();
                            var val = Deserialize(aReader);
                            tmp.Add(key, val);
                        }
                        return tmp;
                    }
                case JSONBinaryTag.Value:
                    {
                        return new JSONData(aReader.ReadString());
                    }
                case JSONBinaryTag.IntValue:
                    {
                        return new JSONData(aReader.ReadInt32());
                    }
                case JSONBinaryTag.DoubleValue:
                    {
                        return new JSONData(aReader.ReadDouble());
                    }
                case JSONBinaryTag.BoolValue:
                    {
                        return new JSONData(aReader.ReadBoolean());
                    }
                case JSONBinaryTag.FloatValue:
                    {
                        return new JSONData(aReader.ReadSingle());
                    }

                default:
                    {
                        throw new Exception("Error deserializing JSON. Unknown tag: " + type);
                    }
            }
        }

        public static JSONNode LoadFromBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromStream(stream);
        }

        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromFile(string aFileName)
        {
            using (var F = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromStream(F);
            }
        }

        public static JSONNode LoadFromStream(System.IO.Stream aData)
        {
            using (var R = new System.IO.BinaryReader(aData))
            {
                return Deserialize(R);
            }
        }

        public static JSONNode Parse(string aJSON)
        {
            Stack<JSONNode> stack = new Stack<JSONNode>();
            JSONNode ctx = null;
            int i = 0;
            string Token = "";
            string TokenName = "";
            bool QuoteMode = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        stack.Push(new JSONClass());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }

                        stack.Push(new JSONArray());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName, stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (Token != "")
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        TokenName = Token;
                        Token = "";
                        break;

                    case '"':
                        QuoteMode ^= true;
                        break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        if (Token != "")
                        {
                            if (ctx is JSONArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (QuoteMode)
                            Token += aJSON[i];
                        break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = aJSON[i];
                            switch (C)
                            {
                                case 't': Token += '\t'; break;
                                case 'r': Token += '\r'; break;
                                case 'n': Token += '\n'; break;
                                case 'b': Token += '\b'; break;
                                case 'f': Token += '\f'; break;
                                case 'u':
                                    {
                                        string s = aJSON.Substring(i + 1, 4);
                                        Token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        i += 4;
                                        break;
                                    }
                                default: Token += C; break;
                            }
                        }
                        break;

                    default:
                        Token += aJSON[i];
                        break;
                }
                ++i;
            }
            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }

        public string SaveToBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

        public string SaveToCompressedBase64()
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public void SaveToStream(System.IO.Stream aData)
        {
            var W = new System.IO.BinaryWriter(aData);
            Serialize(W);
        }

        public virtual void Serialize(System.IO.BinaryWriter aWriter)
        {
        }

        public static string Escape(string aText)
        {


            if (aText == null)
                return string.Empty;
            var sb = new StringBuilder();
            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}

		public void SaveToCompressedFile(string aFileName)
		{
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}

#else
#endif
#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
#else
#endif
    }
    public class JSONArray : JSONNode, IEnumerable
    {
        private List<JSONNode> m_List = new List<JSONNode>();

        public override IEnumerable<JSONNode> Childs
        {
            get
            {
                foreach (JSONNode N in m_List)
                    yield return N;
            }
        }

        public override int Count
        {
            get { return m_List.Count; }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return new JSONLazyCreator(this);
                return m_List[aIndex];
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }

        public override JSONNode this[string aKey]
        {
            get { return new JSONLazyCreator(this); }
            set { m_List.Add(value); }
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            m_List.Add(aItem);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (JSONNode N in m_List)
                yield return N;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
            JSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Array);
            aWriter.Write(m_List.Count);
            for (int i = 0; i < m_List.Count; i++)
            {
                m_List[i].Serialize(aWriter);
            }
        }

        public override string ToString()
        {
            return ToString(false);
        }
        public override string ToString(bool isRoot)
        {
            if (isRoot)
            {
                TabIndex = 0;
            }
            string result = "[ ";
            TabIndex++;
            result += Environment.NewLine;
            for (int index = 0; index < m_List.Count; index++)
            {
                JSONNode N = m_List[index];
                if (index > 0)
                    result += ", ";
                result += N.ToString();
            }
            TabIndex--;
            result += " ]";
            result +=Environment.NewLine;
            return result;
        }

        public override string ToString(string aPrefix)
        {
            string result = "[ ";
            foreach (JSONNode N in m_List)
            {
                if (result.Length > 3)
                    result += ", ";
                result += string.Format("\n{0}   ", aPrefix);
                result += N.ToString(string.Format("{0}   ", aPrefix));
            }
            result += string.Format("\n{0}]", aPrefix);
            return result;
        }
    }
    // End of JSONNode

}