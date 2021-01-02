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
    public class ImageAttribute: CustomXmlAttribute
    {
        public override AttributeDictionary Convert(string value, AttributeDictionary elementAttributes, XmlElement xmlElement)
        {
            var result = new AttributeDictionary()
            {
                {
                    "sprite", 
                    value.Replace(".png", "")
                         .Replace(".jpg", "")
                         .Replace(".jpeg", "")
                         .Replace(".bmp", "")
                         .Replace(".psd", "")
                }
            };

            if (!elementAttributes.ContainsKey("color"))
            {
                elementAttributes.Add("color", "white");
            }

            return result;
        }

        public override eAttributeGroup AttributeGroup
        {
            get
            {
                return eAttributeGroup.Image;
            }
        }
    }    

    public class SpriteAttribute : ImageAttribute 
    {
        public override bool KeepOriginalTag
        {
            get
            {
                return true;
            }
        }
    }    
}
