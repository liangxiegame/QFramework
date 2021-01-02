using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.UI;

namespace UI.Xml.CustomAttributes
{    
    public class OpacityAttribute: CustomXmlAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {
            var parsedValue = float.Parse(value);

            xmlElement.DefaultOpacity = parsedValue;
            xmlElement.CanvasGroup.alpha = parsedValue;                        
        }

        public override string ValueDataType
        {
            get
            {
                return "xs:float";
            }
        }
    }
}
