using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace UI.Xml.Tags
{
    public class TextTagHandler : ElementTagHandler
    {
        public static List<string> TextAttributes = new List<string>()
        {
            "text", "fontstyle", "font", "fontsize",
            "horizontalOverflow", "verticalOverflow",
            "resizeTextForBestFit", "resizeTextMinSize", "resizeTextMaxSize"
        };

        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Text>();
            }
        }        

        public override bool ParseChildElements(System.Xml.XmlNode xmlNode)
        {
            var innerXml = xmlNode.InnerXml
                                  .Replace(" xmlns=\"http://www.w3schools.com\"", "")
                                  .Replace("<![CDATA[", "")
                                  .Replace("]]>", "");                                  

            innerXml = ReplaceIgnoreCase(innerXml, "<textcolor color=", "<color=");
            innerXml = ReplaceIgnoreCase(innerXml, "</textcolor", "</color");

            innerXml = ReplaceIgnoreCase(innerXml, "<textsize size=", "<size=");
            innerXml = ReplaceIgnoreCase(innerXml, "</textsize", "</size");

            innerXml = innerXml.Trim();

            var textComponent = primaryComponent as Text;
            textComponent.text = innerXml;            

            return true;
        }

        private string ReplaceIgnoreCase(string source, string match, string replace)
        {
            var regex = new Regex(match, RegexOptions.IgnoreCase);
            
            return regex.Replace(source, replace);
        }
    }
}
