using System;
using System.Collections.Generic;

using System.Text;


namespace MyJson
{
    public class MyJson
    {  
        internal static IJsonNode Scan(ScanObj scan)
        {
            for (int i = 0; i < scan.json.Length; i++)
            {
                IJsonNode node = ScanFirst(scan.json[i]);
                if (node != null)
                {
                    scan.seed = i;
                    node.Scan(scan);
                    return node;
                }
            }
            return null;
        }


        public static IJsonNode Parse(string json)
        {
            //try
            //{
            ScanObj obj = new ScanObj();
            obj.json = json;
            obj.seed = 0;
            IJsonNode node = Scan(obj);
            return node;
            //}
            //catch (Exception err)
            //{
            //    throw new Exception("parse err:" + json, err);
            //}
        }


        internal static IJsonNode ScanFirst(char c)
        {
            if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
            {
                return null;
            }
            if (c == '{')
            {
                return new JsonNode_Object();
            }
            else if (c == '[')
            {
                return new JsonNode_Array();
            }
            else if (c == '"')
            {
                return new JsonNode_ValueString();
            }
            else
            {
                return new JsonNode_ValueNumber();
            }
        }
    }
    public enum jsontype
    {
        Value_Number,
        Value_String,
        Array,
        Object,
    }

    public interface IJsonNode
    {
        jsontype type
        {
            get;

        }
        void ConvertToString(StringBuilder sb);

        void ConvertToStringPhp(StringBuilder sb);

        void ConvertToStringWithFormat(StringBuilder sb, int spacesub);

        void ConvertToStringPhpWithFormat(StringBuilder sb, int spacesub);

        void Scan(ScanObj scan);

        IJsonNode Get(string path);

        //增加大量快速访问方法
        IJsonNode GetArrayItem(int index);
        IJsonNode GetDictItem(string key);

        void AddArrayValue(IJsonNode node);
        void AddArrayValue(double value);
        void AddArrayValue(bool value);
        void AddArrayValue(string value);

        void SetArrayValue(int index, IJsonNode node);
        void SetArrayValue(int index, double value);
        void SetArrayValue(int index, bool value);
        void SetArrayValue(int index, string value);

        void SetDictValue(string key, IJsonNode node);
        void SetDictValue(string key, double value);
        void SetDictValue(string key, bool value);
        void SetDictValue(string key, string value);

        void SetValue(double value);
        void SetValue(string value);
        void SetValue(bool value);

        double AsDouble();
        int AsInt();
        bool AsBool();

        bool IsNull();
        String AsString();
        IList<IJsonNode> AsList();
        IDictionary<string, IJsonNode> asDict();

        bool HaveDictItem(string key);

        int GetListCount();
    }

   
    public class ScanObj
    {
        public string json;
        public int seed;
    }
    public class JsonNode_ValueNumber : IJsonNode
    {
        public JsonNode_ValueNumber()
        {

        }
        public JsonNode_ValueNumber(double value)
        {
            this.value = value;
            this.isBool = false;
        }
        public JsonNode_ValueNumber(bool value)
        {
            this.value = value ? 1 : 0;
            this.isBool = true;
        }
        public double value
        {
            get;
            set;
        }
        public bool isBool
        {
            get;
            private set;
        }
        public bool isNull
        {
            get;
            private set;
        }
        public void SetNull()
        {
            this.isNull = true;
            this.isBool = false;
        }
        public void SetBool(bool v)
        {
            this.value = v ? 1 : 0;
            this.isBool = true;
        }
        public override string ToString()
        {
            if (isBool)
            {
                return ((bool)this) ? "true" : "false";
            }
            else if (isNull)
            {
                return "null";
            }
            else
            {
                return value.ToString();
            }
        }

        public jsontype type
        {
            get
            {
                return jsontype.Value_Number;
            }
        }
        public void ConvertToString(StringBuilder sb)
        {
            sb.Append(ToString());
        }
        public void ConvertToStringWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int i = 0; i < space; i++)
            //    sb.Append(' ');
            ConvertToString(sb);
        }
        public void ConvertToStringPhp(StringBuilder sb)
        {

            sb.Append(ToString());
        }
        public void ConvertToStringPhpWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int i = 0; i < space; i++)
            //    sb.Append(' ');
            ConvertToStringPhp(sb);
        }
        public void Scan(ScanObj scan)
        {
            string number = "";
            for (int i = scan.seed; i < scan.json.Length; i++)
            {
                char c = scan.json[i];
                if (c != ',' && c != ']' && c != '}' && c != ' ')
                {
                    if (c != '\n')
                        number += c;
                }
                else
                {
                    scan.seed = i;
                    break;
                }
            }

            if (number.ToLower() == "true")
            {
                value = 1;
                isBool = true;
            }
            else if (number.ToLower() == "false")
            {
                value = 0;
                isBool = true;
            }
            else if (number.ToLower() == "null")
            {
                value = 0;
                isNull = true;
            }
            else
            {
                value = double.Parse(number);
                isBool = false;
            }
        }
        public static implicit operator double(JsonNode_ValueNumber m)
        {
            return m.value;
        }
        public static implicit operator float(JsonNode_ValueNumber m)
        {
            return (float)m.value;
        }
        public static implicit operator int(JsonNode_ValueNumber m)
        {
            return (int)m.value;
        }
        public static implicit operator uint(JsonNode_ValueNumber m)
        {
            return (uint)m.value;
        }

        public static implicit operator bool(JsonNode_ValueNumber m)
        {
            return (uint)m.value != 0;
        }


        public IJsonNode Get(string path)
        {
            if (string.IsNullOrEmpty(path)) return this;

            return null;
        }

        public IJsonNode GetArrayItem(int index)
        {
            throw new NotImplementedException();
        }

        public IJsonNode GetDictItem(string key)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(double value)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(bool value)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, double value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, string value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, double value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(double value)
        {
            this.value = value;
            this.isBool = false;
            this.isNull = false;
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(bool value)
        {
            this.value = value ? 1 : 0;
            this.isBool = true;
            this.isNull = false;
        }

        public double AsDouble()
        {
            if (!this.isNull && !this.isBool)
                return this.value;
            throw new Exception("Value type 不同");
        }

        public int AsInt()
        {
            if (!this.isNull && !this.isBool)
                return (int)this.value;
            throw new Exception("Value type 不同");
        }

        public bool AsBool()
        {
            if (this.isBool)
            {
                return (uint)value != 0;
            }
            throw new Exception("Value type 不同");
        }

        public bool IsNull()
        {
            return isNull;
        }

        public string AsString()
        {
            return value.ToString();
        }

        public IList<IJsonNode> AsList()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IJsonNode> asDict()
        {
            throw new NotImplementedException();
        }


        public bool HaveDictItem(string key)
        {
            throw new NotImplementedException();
        }

        public int GetListCount()
        {
            throw new NotImplementedException();
        }
    }
    public class JsonNode_ValueString : IJsonNode
    {
        public JsonNode_ValueString()
        {

        }
        public JsonNode_ValueString(string value)
        {
            this.value = value;
        }
        public string value
        {
            get;
            set;
        }
        public override string ToString()
        {
            return value;
        }

        public jsontype type
        {
            get
            {
                return jsontype.Value_String;
            }
        }
        public void ConvertToString(StringBuilder sb)
        {
            sb.Append('\"');
            if (value != null)
            {
                string v = value.Replace("\\", "\\\\");
                v = v.Replace("\"", "\\\"");
                sb.Append(v);
            }
            sb.Append('\"');
        }
        public void ConvertToStringWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int i = 0; i < space; i++)
            //    sb.Append(' ');
            ConvertToString(sb);
        }
        public void ConvertToStringPhp(StringBuilder sb)
        {
            sb.Append('\"');
            if (value != null)
            {
                string v = value.Replace("\\", "\\\\");
                v = v.Replace("\"", "\\\"");
                sb.Append(v);
            }
            sb.Append('\"');
        }
        public void ConvertToStringPhpWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int i = 0; i < space; i++)
            //    sb.Append(' ');
            ConvertToStringPhp(sb);
        }
        public void Scan(ScanObj scan)
        {
            string _value = "";
            for (int i = scan.seed + 1; i < scan.json.Length; i++)
            {
                char c = scan.json[i];
                if (c == '\\')
                {
                    i++;
                    c = scan.json[i];
                    _value += c;
                }

                else if (c != '\"')
                {

                    _value += c;
                }

                else
                {
                    scan.seed = i + 1;
                    break;
                }
            }
            value = _value;
        }

        public static implicit operator string(JsonNode_ValueString m)
        {
            return m.value;
        }



        public IJsonNode Get(string path)
        {
            if (string.IsNullOrEmpty(path)) return this;

            return null;
        }


        public IJsonNode GetArrayItem(int index)
        {
            throw new NotImplementedException();
        }

        public IJsonNode GetDictItem(string key)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(double value)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(bool value)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, double value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, string value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, double value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(double value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        public void SetValue(bool value)
        {
            throw new NotImplementedException();
        }

        public double AsDouble()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            throw new NotImplementedException();
        }

        public bool AsBool()
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            return false;
        }

        public string AsString()
        {
            return value;
        }

        public IList<IJsonNode> AsList()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IJsonNode> asDict()
        {
            throw new NotImplementedException();
        }


        public bool HaveDictItem(string key)
        {
            throw new NotImplementedException();
        }

        public int GetListCount()
        {
            throw new NotImplementedException();
        }
    }

    public class JsonNode_Array : List<IJsonNode>, IJsonNode
    {
        public jsontype type
        {
            get { return jsontype.Array; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ConvertToString(sb);
            return sb.ToString();
        }
        public void ConvertToString(StringBuilder sb)
        {
            sb.Append('[');
            for (int i = 0; i < this.Count; i++)
            {
                this[i].ConvertToString(sb);
                if (i != this.Count - 1)
                    sb.Append(',');
            }
            sb.Append(']');
        }
        public void ConvertToStringWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            sb.Append("[\n");
            for (int i = 0; i < this.Count; i++)
            {
                for (int _i = 0; _i < spacesub; _i++)
                    sb.Append(' ');

                this[i].ConvertToStringWithFormat(sb, spacesub + 4);
                if (i != this.Count - 1)
                    sb.Append(',');
                sb.Append('\n');
            }
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            for (int _i = 0; _i < spacesub; _i++)
                sb.Append(' ');
            sb.Append(']');
        }
        public void ConvertToStringPhp(StringBuilder sb)
        {
            sb.Append("Array(");
            for (int i = 0; i < this.Count; i++)
            {
                this[i].ConvertToStringPhp(sb);
                if (i != this.Count - 1)
                    sb.Append(',');
            }
            sb.Append(')');
        }
        public void ConvertToStringPhpWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            sb.Append("Array(\n");
            for (int i = 0; i < this.Count; i++)
            {
                for (int _i = 0; _i < spacesub; _i++)
                    sb.Append(' ');

                this[i].ConvertToStringPhpWithFormat(sb, spacesub + 4);
                if (i != this.Count - 1)
                    sb.Append(',');
                sb.Append('\n');
            }
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            for (int _i = 0; _i < spacesub; _i++)
                sb.Append(' ');
            sb.Append(')');
        }
        public void Scan(ScanObj scan)
        {
            for (int i = scan.seed + 1; i < scan.json.Length; i++)
            {
                char c = scan.json[i];
                if (c == ',')
                    continue;
                if (c == ']')
                {
                    scan.seed = i + 1;
                    break;
                }
                IJsonNode node = MyJson.ScanFirst(c);
                if (node != null)
                {
                    scan.seed = i;
                    node.Scan(scan);
                    i = scan.seed - 1;
                    this.Add(node);
                }

            }
        }

        public int GetFirstKey02(string path, int start, out string nextpath)
        {
            int _path = -1;
            for (int i = start + 1; i < path.Length; i++)
            {
                if (path[i] == '[')
                {
                    _path = GetFirstKey02(path, i, out nextpath);
                }
                if (path[i] == ']')
                {
                    nextpath = path.Substring(i + 1);
                    if (_path == -1)
                    {
                        _path = int.Parse(path.Substring(start + 1, i - start - 1));
                    }
                    return _path;
                }
            }
            nextpath = null;
            return -1;
        }

        public int GetFirstKey(string path, out string nextpath)
        {
            nextpath = null;
            int istart = 0;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '.' || path[i] == ' ')
                {
                    istart++;
                    continue;
                }
                if (path[i] == '[')
                {
                    return GetFirstKey02(path, i, out nextpath);
                }

            }

            return -1;


        }


        public IJsonNode Get(string path)
        {
            if (path.Length == 0) return this;
            string nextpath;
            int key = GetFirstKey(path, out nextpath);
            if (key >= 0 && key < this.Count)
            {
                return this[key];
            }
            else
            {
                return null;
            }
        }

        public IJsonNode GetArrayItem(int index)
        {
            return this[index];
        }

        public IJsonNode GetDictItem(string key)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(IJsonNode node)
        {
            this.Add(node);
        }

        public void AddArrayValue(double value)
        {
            this.Add(new JsonNode_ValueNumber(value));
        }

        public void AddArrayValue(bool value)
        {
            this.Add(new JsonNode_ValueNumber(value));
        }

        public void AddArrayValue(string value)
        {
            this.Add(new JsonNode_ValueString(value));
        }

        public void SetArrayValue(int index, IJsonNode node)
        {
            this[index] = node;
        }

        public void SetArrayValue(int index, double value)
        {
            this[index] = new JsonNode_ValueNumber(value);
        }

        public void SetArrayValue(int index, bool value)
        {
            this[index] = new JsonNode_ValueNumber(value);
        }

        public void SetArrayValue(int index, string value)
        {
            this[index] = new JsonNode_ValueString(value);
        }

        public void SetDictValue(string key, IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, double value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(double value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(bool value)
        {
            throw new NotImplementedException();
        }

        public double AsDouble()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            throw new NotImplementedException();
        }

        public bool AsBool()
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            return false;
        }

        public string AsString()
        {
            throw new NotImplementedException();
        }

        public IList<IJsonNode> AsList()
        {
            return this;
        }

        public IDictionary<string, IJsonNode> asDict()
        {
            throw new NotImplementedException();
        }


        public bool HaveDictItem(string key)
        {
            throw new NotImplementedException();
        }

        public int GetListCount()
        {
            return this.Count;
        }
    }
    public class JsonNode_Object : Dictionary<string, IJsonNode>, IJsonNode
    {
        public jsontype type
        {
            get { return jsontype.Object; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ConvertToString(sb);
            return sb.ToString();
        }
        public void ConvertToString(StringBuilder sb)
        {
            sb.Append('{');
            int i = Count;
            foreach (var item in this)
            {
                sb.Append('\"');
                sb.Append(item.Key);
                sb.Append("\":");
                item.Value.ConvertToString(sb);
                i--;
                if (i != 0) sb.Append(',');
            }
            sb.Append('}');
        }
        public void ConvertToStringWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            sb.Append("{\n");
            int i = Count;
            foreach (var item in this)
            {
                for (int _i = 0; _i < spacesub; _i++)
                    sb.Append(' ');

                sb.Append('\"');
                sb.Append(item.Key);
                sb.Append("\":");
                item.Value.ConvertToStringWithFormat(sb, spacesub + 4);
                i--;
                if (i != 0) sb.Append(',');
                sb.Append('\n');
            }
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            for (int _i = 0; _i < spacesub; _i++)
                sb.Append(' ');
            sb.Append('}');
        }
        public void ConvertToStringPhp(StringBuilder sb)
        {
            sb.Append("Array(");
            int i = Count;
            foreach (var item in this)
            {
                sb.Append('\"');
                sb.Append(item.Key);
                sb.Append("\"=>");
                item.Value.ConvertToStringPhp(sb);
                i--;
                if (i != 0) sb.Append(',');
            }
            sb.Append(')');
        }
        public void ConvertToStringPhpWithFormat(StringBuilder sb, int spacesub)
        {
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            sb.Append("Array(\n");
            int i = Count;
            foreach (var item in this)
            {
                for (int _i = 0; _i < spacesub; _i++)
                    sb.Append(' ');

                sb.Append('\"');
                sb.Append(item.Key);
                sb.Append("\"=>");
                item.Value.ConvertToStringPhpWithFormat(sb, spacesub + 4);
                i--;
                if (i != 0) sb.Append(',');
                sb.Append('\n');
            }
            //for (int _i = 0; _i < space; _i++)
            //    sb.Append(' ');
            for (int _i = 0; _i < spacesub; _i++)
                sb.Append(' ');
            sb.Append(')');
        }
        //public MyJson.IJsonNode  this[string key]
        //{
        //    get
        //    {
        //        if (this.ContainsKey(key))
        //        {
        //            return base[key];
        //        }

        //        throw new Exception("key not exist");

        //    }
        //    set
        //    {
        //        if (value == null)
        //        {

        //            throw new Exception("value is null. key:"+key);
        //        }
        //        base[key] = value;
        //    }
        //}

        public void Scan(ScanObj scan)
        {
            string key = null;
            int keystate = 0;//0 nokey 1scankey 2gotkey
            for (int i = scan.seed + 1; i < scan.json.Length; i++)
            {
                char c = scan.json[i];
                if (keystate != 1 && (c == ',' || c == ':'))
                    continue;
                if (c == '}')
                {
                    scan.seed = i + 1;
                    break;
                }
                if (keystate == 0)
                {
                    if (c == '\"')
                    {
                        keystate = 1;
                        key = "";
                    }
                }
                else if (keystate == 1)
                {
                    if (c == '\"')
                    {
                        keystate = 2;
                        //scan.seed = i + 1;
                        continue;
                    }
                    else
                    {
                        key += c;
                    }
                }
                else
                {
                    IJsonNode node = MyJson.ScanFirst(c);
                    if (node != null)
                    {
                        scan.seed = i;
                        node.Scan(scan);
                        i = scan.seed - 1;
                        this.Add(key, node);
                        keystate = 0;
                    }
                }

            }
        }
        public string GetFirstKey01(string path, int start, out string nextpath)
        {
            for (int i = start + 1; i < path.Length; i++)
            {
                if (path[i] == '\\') continue;
                if (path[i] == '\"')
                {
                    nextpath = path.Substring(i + 1);
                    var _path = path.Substring(start + 1, i - start - 1);
                    return _path;
                }
            }
            nextpath = null;
            return null;
        }
        public string GetFirstKey02(string path, int start, out string nextpath)
        {
            string _path = null;
            for (int i = start + 1; i < path.Length; i++)
            {
                if (path[i] == '[')
                {
                    _path = GetFirstKey02(path, i, out nextpath);
                }
                if (path[i] == '\"')
                {
                    _path = GetFirstKey01(path, i, out nextpath);
                    i += _path.Length + 2;
                }
                if (path[i] == ']')
                {
                    nextpath = path.Substring(i + 1);
                    if (_path == null)
                    {
                        _path = path.Substring(start + 1, i - start - 1);
                    }
                    return _path;
                }
            }
            nextpath = null;
            return null;
        }
        public string GetFirstKey(string path, out string nextpath)
        {
            nextpath = null;
            int istart = 0;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '.' || path[i] == ' ')
                {
                    istart++;
                    continue;
                }
                if (path[i] == '[')
                {
                    return GetFirstKey02(path, i, out nextpath);
                }
                else if (path[i] == '\"')
                {
                    return GetFirstKey01(path, i, out nextpath);
                }
                else
                {

                    int iend1 = path.IndexOf('[', i + 1);
                    if (iend1 == -1) iend1 = path.Length;
                    int iend2 = path.IndexOf('.', i + 1);
                    if (iend2 == -1) iend2 = path.Length;
                    int iss = Math.Min(iend1, iend2);

                    var _path = path.Substring(istart, iss - istart);
                    nextpath = path.Substring(iss);
                    return _path;
                }

            }

            return null;


        }
        public IJsonNode Get(string path)
        {
            if (path.Length == 0) return this;
            string nextpath;
            string key = GetFirstKey(path, out nextpath);
            if (this.ContainsKey(key))
            {
                return this[key].Get(nextpath);
            }
            else
            {
                return null;
            }

        }

        public IJsonNode GetArrayItem(int index)
        {
            throw new NotImplementedException();
        }

        public IJsonNode GetDictItem(string key)
        {
            return this[key];
        }

        public void AddArrayValue(IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(double value)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(bool value)
        {
            throw new NotImplementedException();
        }

        public void AddArrayValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, IJsonNode node)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, double value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetArrayValue(int index, string value)
        {
            throw new NotImplementedException();
        }

        public void SetDictValue(string key, IJsonNode node)
        {
            this[key] = node;
        }

        public void SetDictValue(string key, double value)
        {
            this[key] = new JsonNode_ValueNumber(value);
        }

        public void SetDictValue(string key, bool value)
        {
            this[key] = new JsonNode_ValueNumber(value);
        }

        public void SetDictValue(string key, string value)
        {
            this[key] = new JsonNode_ValueString(value);
        }

        public void SetValue(double value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string value)
        {
            throw new NotImplementedException();
        }

        public void SetValue(bool value)
        {
            throw new NotImplementedException();
        }

        public double AsDouble()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            throw new NotImplementedException();
        }

        public bool AsBool()
        {
            throw new NotImplementedException();
        }

        public bool IsNull()
        {
            return false;
        }

        public string AsString()
        {
            throw new NotImplementedException();
        }

        public IList<IJsonNode> AsList()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IJsonNode> asDict()
        {
            return this;
        }


        public bool HaveDictItem(string key)
        {
            return ContainsKey(key);
        }

        public int GetListCount()
        {
            throw new NotImplementedException();
        }
    }
}