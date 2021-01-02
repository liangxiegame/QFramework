using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace UI.Xml
{
    public partial class XmlLayout
    {
        public XmlLayoutLocalization LocalizationFile = null;
        public bool editor_showLocalization = false;

        protected string HandleLocalization(string xml)
        {            
            if (LocalizationFile != null)
            {
                var xmlStringBuilder = new StringBuilder(xml);

                foreach (var kvp in LocalizationFile.strings)
                {                                        
                    xmlStringBuilder.Replace("{" + String.Format("{0}", kvp.Key) + "}", kvp.Value);
                }

                xml = xmlStringBuilder.ToString();
            }

            return xml;
        }

        public void SetLocalizationFile(XmlLayoutLocalization newLocalizationFile)
        {
            this.LocalizationFile = newLocalizationFile;
            RebuildLayout(true, false);
        }
    }
}
