using System.Collections.Generic;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public class DataBag : IJsonObject
    {
        Dictionary<string,string>  _dict = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                if (_dict.ContainsKey(key))
                    return _dict[key];

                return null;
            }
            set
            {
                AddOrReplace(key,value);
            }
        }

        public void AddOrReplace(string key, string value)
        {
            if (_dict.ContainsKey(key))
            {
                if (value == null)
                {
                    _dict.Remove(key);
                    return;
                }
                _dict[key] = value;
            }
            else
            {
                _dict.Add(key, value);
            }
        }
        public void Serialize(JSONClass cls)
        {
            foreach (var item in _dict)
            {
                cls.Add(item.Key, new JSONData(item.Value));
            }
        }

        public void Deserialize(JSONClass cls)
        {
            _dict.Clear();
            foreach (KeyValuePair<string, JSONNode> jsonNode in cls)
            {
                _dict.Add(jsonNode.Key, jsonNode.Value.Value);
            }
        }
    }
}