using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace UI.Xml.Tags
{
    public abstract class LayoutBaseTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<HorizontalOrVerticalLayoutGroup>();
            }
        }
    }
}
