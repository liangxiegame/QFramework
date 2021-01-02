using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Reflection;

namespace UI.Xml
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ElementTagHandlerAttribute : Attribute
    {
        public string TagName;

        public ElementTagHandlerAttribute(string tagName)
        {
            TagName = tagName;
        }
    }    
}
