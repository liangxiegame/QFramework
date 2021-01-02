using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public class MaskTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<Mask>();
            }
        }
    }
}
