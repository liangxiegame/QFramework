using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


namespace UI.Xml.CustomAttributes
{
    public abstract class AspectRatioFitterAttribute : CustomXmlAttribute
    {
        protected AspectRatioFitter GetAspectRatioFitter(XmlElement element)
        {
            var aspectRatioFitter = element.GetComponent<AspectRatioFitter>();

            if (aspectRatioFitter == null)
            {
                aspectRatioFitter = element.gameObject.AddComponent<AspectRatioFitter>();
            }

            return aspectRatioFitter;
        }

        public override eAttributeGroup AttributeGroup
        {
            get
            {
                return eAttributeGroup.RectTransform;
            }
        }
    }

    public class AspectRatioAttribute : AspectRatioFitterAttribute
    {                
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary attributes)
        {
            var aspectRatioFitter = GetAspectRatioFitter(xmlElement);
            aspectRatioFitter.aspectRatio = ParseAspectRatioStringValue(value);

            // Seems to only take effect if we set it again at the end of the frame
            XmlLayoutTimer.DelayedCall(0, () => {
                aspectRatioFitter.aspectRatio = ParseAspectRatioStringValue(value);                
            }, xmlElement);
        }

        protected float ParseAspectRatioStringValue(string value)
        {
            float aspectRatio = 1f;
            if (!float.TryParse(value, out aspectRatio))
            {                
                float[] values = value.Split(':').Select(v => float.Parse(v)).ToArray();                

                aspectRatio = values[0] / values[1];                
            }

            return aspectRatio;
        }
    }

    public class AspectModeAttribute : AspectRatioFitterAttribute
    {
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary attributes)
        {
            GetAspectRatioFitter(xmlElement).aspectMode = (AspectRatioFitter.AspectMode)Enum.Parse(typeof(AspectRatioFitter.AspectMode), value, true);
        }

        public override string ValueDataType
        {
            get
            {
                return String.Join(",", Enum.GetNames(typeof(AspectRatioFitter.AspectMode)));
            }
        }
    }


}
