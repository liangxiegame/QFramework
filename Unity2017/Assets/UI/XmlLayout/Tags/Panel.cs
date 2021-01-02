using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public class PanelTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<SimpleLayoutGroup>();
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            base.ApplyAttributes(attributes);

            var padding = attributes.GetValue<RectOffset>("padding");
            if (padding != null)
            {
                var layoutGroup = primaryComponent as SimpleLayoutGroup;
                layoutGroup.padding = padding;
                layoutGroup.enabled = true;
            }
        }
    }
}
