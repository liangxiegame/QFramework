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
    public class ShadowAttribute: CustomXmlAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {
            if (value.Equals("None", StringComparison.OrdinalIgnoreCase)) return;
            if (!elementAttributes.ContainsKey("shadow")) return;

            var elementTransform = xmlElement.rectTransform;
            var shadowColor = elementAttributes["shadow"].ToColor();

            var shadow = elementTransform.GetComponent<Shadow>();
            if (shadow == null)
            {
                shadow = elementTransform.gameObject.AddComponent<Shadow>();
            }

            shadow.effectColor = shadowColor;

            if (elementAttributes.ContainsKey("shadowdistance"))
            {
                shadow.effectDistance = elementAttributes["shadowdistance"].ToVector2();
            }            
        }

        public override string ValueDataType
        {
            get
            {
                return "color";
            }
        }
    }

    public class ShadowDistanceAttribute : ShadowAttribute 
    {
        public override string ValueDataType
        {
            get
            {
                return "xs:float";
            }
        }
    }    
}
