using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI.Xml.Tags
{
    public class XmlLayoutTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                //return currentInstanceTransform.GetComponent<RectTransform>();
                return null;
            }
        }        
    }
}
