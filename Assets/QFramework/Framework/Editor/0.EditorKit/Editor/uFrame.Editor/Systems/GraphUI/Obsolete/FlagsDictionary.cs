using System.Collections.Generic;
using QFramework;
using QFramework.Json;

namespace QFramework.GraphDesigner
{
    public class FlagsDictionary : Dictionary<string,bool>, IJsonObject
    {


        public void Serialize(JSONClass cls)
        {
            foreach (var item in this)
            {
                cls.Add(item.Key,new JSONData(item.Value));
            }
        }

        public void Deserialize(JSONClass cls)
        {
            this.Clear();
            foreach (KeyValuePair<string,JSONNode> jsonNode in cls)
            {
                this.Add(jsonNode.Key,jsonNode.Value.AsBool);
            }
            var removeKeys = new List<string>();
            foreach (var item in this)
            {
                if (item.Value == false)
                {
                    removeKeys.Add(item.Key);
                }
            }
            foreach (var removeKey in removeKeys)
            {
                this.Remove(removeKey);
            }
        }
    }
}