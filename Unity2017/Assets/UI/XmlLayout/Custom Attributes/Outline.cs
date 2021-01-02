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
    public class OutlineAttribute: CustomXmlAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {
            if (value.Equals("None", StringComparison.OrdinalIgnoreCase)) return;
            if (!elementAttributes.ContainsKey("outline")) return;
            
            var elementTransform = xmlElement.rectTransform;
            var outlineColor = elementAttributes["outline"].ToColor();            

            var outline = elementTransform.GetComponent<Outline>();
            if (outline == null)
            {
                outline = elementTransform.gameObject.AddComponent<Outline>();
            }
            
            outline.effectColor = outlineColor;

            if (elementAttributes.ContainsKey("outlinesize"))
            {
                outline.effectDistance = elementAttributes["outlinesize"].ToVector2();
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

    public class OutlineSizeAttribute : OutlineAttribute 
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
