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
    public class WidthAttribute: SizeAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        {            
            var elementTransform = xmlElement.rectTransform;

            var alignment = elementAttributes.ContainsKey("rectAlignment") ? GetRectAlignment(elementAttributes["rectAlignment"]) : RectAlignment.MiddleCenter;

            if (elementAttributes.ContainsKey("position"))
            {
                elementTransform.position = elementAttributes["position"].ToVector2();
            }

            var position = elementTransform.position;

            var width = float.Parse(value.Replace("%", ""));

            if (value.Contains("%"))
            {
                // Use a percentage-based width value
                elementTransform.sizeDelta = Vector2.zero;

                var workingWidth = width / 100f;

                var vector = ApplyAlignment(new Vector2(workingWidth, 0), alignment);

                elementTransform.anchorMin = new Vector2(vector.x, elementTransform.anchorMin.y);
                elementTransform.anchorMax = new Vector2(vector.x + workingWidth, elementTransform.anchorMax.y);                
            }
            else
            {
                // Use a fixed width value
                var alignmentStruct = GetAlignmentStruct(width, 0, position, alignment);
                
                elementTransform.anchorMin = new Vector2(alignmentStruct.AnchorMin.x, elementTransform.anchorMin.y);
                elementTransform.anchorMax = new Vector2(alignmentStruct.AnchorMax.x, elementTransform.anchorMax.y);

                elementTransform.pivot = new Vector2(alignmentStruct.Pivot.x, elementTransform.pivot.y);

                elementTransform.sizeDelta = new Vector2(width, elementTransform.sizeDelta.y);                
            }            
        }
    }
}
