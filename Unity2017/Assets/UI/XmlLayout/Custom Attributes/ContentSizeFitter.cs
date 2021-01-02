using UnityEngine;
using UnityEngine.UI;
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
    public class ContentSizeFitterAttribute: CustomXmlAttribute
    {                
        public override void Apply(XmlElement xmlElement, string value, AttributeDictionary elementAttributes)
        { 
            var contentSizeFitter = xmlElement.GetComponent<ContentSizeFitter>();

            if (contentSizeFitter == null) contentSizeFitter = xmlElement.gameObject.AddComponent<ContentSizeFitter>();

            if (value == "vertical")
            {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else if (value == "horizontal")
            {
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }            
        }        
    }
}
