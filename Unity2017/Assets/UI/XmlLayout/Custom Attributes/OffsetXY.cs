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
    public class OffsetXYAttribute: CustomXmlAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {            
            var offset = value.ToVector2();

            var elementTransform = xmlElement.rectTransform;
            elementTransform.localPosition = new Vector2(elementTransform.localPosition.x + offset.x, elementTransform.localPosition.y + offset.y);            
        }

        public override eAttributeGroup AttributeGroup
        {
            get
            {
                return eAttributeGroup.RectPosition;
            }
        }

        public override string ValueDataType
        {
            get
            {
                return "vector2";
            }
        }
    }            
}
