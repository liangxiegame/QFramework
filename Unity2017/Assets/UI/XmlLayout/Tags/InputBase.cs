using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public abstract class InputBaseTagHandler : ElementTagHandler
    {        
        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            base.ApplyAttributes(attributes);

            var textComponent = currentInstanceTransform.GetComponentInChildren<Text>();

            if (attributes.ContainsKey("text"))
            {
                textComponent.text = attributes["text"];
            }

            if (attributes.ContainsKey("textColor"))
            {
                textComponent.color = attributes["textcolor"].ToColor();
            }

            if (attributes.ContainsKey("backgroundColor"))
            {
                var propertyInfo = primaryComponent.GetType().GetProperty("targetGraphic");
                if (propertyInfo != null)
                {
                    var targetGraphic = propertyInfo.GetValue(primaryComponent, null) as Image;
                    if (targetGraphic != null)
                    {
                        targetGraphic.color = attributes["backgroundColor"].ToColor();
                    }
                }                
            }
           
        }
    }
}
