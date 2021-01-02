using UnityEngine;

namespace UI.Xml.Configuration
{    
    public class XmlLayoutConfiguration : ScriptableObject
    {
        public Object XSDFile;
        
        /// <summary>
        /// This is used as a base file which then dynamically has custom-attributes and tags added to it before being compiled into XSDFile.
        /// </summary>
        public Object BaseXSDFile;

        [Tooltip("If this is set to true, then XmlLayout will no longer output a message to the console whenever the XSD file has been updated.")]
        public bool SuppressXSDUpdateMessage = false;        
    }
}
