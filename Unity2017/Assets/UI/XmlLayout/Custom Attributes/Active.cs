using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml.CustomAttributes
{    
    public class ActiveAttribute: CustomXmlAttribute
    {                
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {                        
            xmlElement.gameObject.SetActive(value.ToBoolean());            
        }

        public override string ValueDataType
        {
            get
            {
                return "xs:boolean";
            }
        }
    }
}
