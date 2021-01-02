using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public class GridLayoutTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<GridLayoutGroup>();
            }
        }
    }
}
