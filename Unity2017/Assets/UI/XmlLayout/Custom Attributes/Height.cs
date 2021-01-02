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
    public class HeightAttribute: SizeAttribute
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

            var height = float.Parse(value.Replace("%", ""));

            if (value.Contains("%"))
            {
                // Use a percentage-based width value
                elementTransform.sizeDelta = Vector2.zero;

                var workingHeight = height / 100f;

                var vector = ApplyAlignment(new Vector2(0, workingHeight), alignment);

                elementTransform.anchorMin = new Vector2(elementTransform.anchorMin.x, vector.y);
                elementTransform.anchorMax = new Vector2(elementTransform.anchorMax.x, vector.y + workingHeight);                
            }
            else
            {
                // Use a fixed width value
                var alignmentStruct = GetAlignmentStruct(0, height, position, alignment);
                elementTransform.anchorMin = new Vector2(elementTransform.anchorMin.x, alignmentStruct.AnchorMin.y);
                elementTransform.anchorMax = new Vector2(elementTransform.anchorMax.x, alignmentStruct.AnchorMax.y);
                elementTransform.pivot = new Vector2(elementTransform.pivot.x, alignmentStruct.Pivot.y);
                elementTransform.sizeDelta = new Vector2(elementTransform.sizeDelta.x, height);
            }            
        }
    }
}
