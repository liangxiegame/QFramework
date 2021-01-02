using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml
{
    [AttributeUsage(AttributeTargets.Field)]
    public class XmlFieldName : Attribute
    {
        public string fieldName;

        public XmlFieldName(string fieldName)
        {
            this.fieldName = fieldName;
        }
    }

    public partial class ElementTagHandler
    {
        private static Dictionary<Type, Dictionary<string, FieldInfo>> cachedComponentFields = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        private static Dictionary<string, FieldInfo> GetComponentXmlFields(Type type)
        {            
            if (!cachedComponentFields.ContainsKey(type))
            {
                Dictionary<string, FieldInfo> result = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);

                var bindingFlags = System.Reflection.BindingFlags.Public
                                 | System.Reflection.BindingFlags.IgnoreCase
                                 | System.Reflection.BindingFlags.Instance;            

                var fields = type.GetFields(bindingFlags);

                foreach (var field in fields)
                {
                    var attribute = ((XmlFieldName[])field.GetCustomAttributes(typeof(XmlFieldName), false)).FirstOrDefault();

                    if (attribute != null)
                    {
                        result.Add(attribute.fieldName, field);
                    }
                    else
                    {
                        result.Add(field.Name, field);
                    }
                }

                cachedComponentFields.Add(type, result);
            }
            
            return cachedComponentFields[type];            
        }

        private static FieldInfo GetComponentXmlField(Type type, string fieldName)
        {            
            var fields = GetComponentXmlFields(type);
            var key = fields.Keys.FirstOrDefault(k => k.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

            if (key != null)
            {
                return fields[key];
            }
            
            return null;
        }
    }
}
