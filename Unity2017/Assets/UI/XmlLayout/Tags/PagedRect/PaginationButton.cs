// In order to use this, you need to a) have PagedRect installed within the project, and b) make sure PAGEDRECT_PRESENT is defined
// You can do this by
// a) defining it here: #define PAGEDRECT_PRESENT (not reccommended as it will be overwritten if you update XmlLayout)
// b) defining it in your "Player Settings" -> "Scripting Define Symbols"
// c) Adding a file called "smcs.rsp" to your PROJECT_DIR/Assets/ folder, and putting the following text in it "-define: PAGEDRECT_PRESENT" (you will need to restart Visual Studio after doing so)
#if PAGEDRECT_PRESENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UI.Pagination;

namespace UI.Xml.Tags
{
    public class PaginationButtonTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<Button>();
            }
        }             

        /// <summary>
        /// don't add this to the XSD file
        /// </summary>
        public override bool isCustomElement
        {
            get
            {
                return false;
            }
        }        

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            base.ApplyAttributes(attributes);

            var textComponent = currentInstanceTransform.GetComponentInChildren<Text>();            

            var tagHandler = XmlLayoutUtilities.GetXmlTagHandler("Text");                        
            tagHandler.SetInstance(textComponent.rectTransform, this.currentXmlLayoutInstance);
            
            var textAttributes = new AttributeDictionary(
                                            attributes.Where(a => TextTagHandler.TextAttributes.Contains(a.Key, StringComparer.OrdinalIgnoreCase))
                                                      .ToDictionary(a => a.Key, b => b.Value));
            
            if (attributes.ContainsKey("textshadow")) textAttributes.Add("shadow", attributes["textshadow"]);
            if (attributes.ContainsKey("textoutline")) textAttributes.Add("outline", attributes["textoutline"]);
            if (attributes.ContainsKey("textcolor")) textAttributes.Add("color", attributes["textcolor"]);
            if (attributes.ContainsKey("textalignment")) textAttributes.Add("alignment", attributes["textalignment"]);
            
            tagHandler.ApplyAttributes(textAttributes);

            // preserve aspect for button background
            var imageComponent = currentInstanceTransform.GetComponent<Image>();
            if (attributes.ContainsKey("preserveaspect")) imageComponent.preserveAspect = attributes["preserveaspect"].ToBoolean();



            // I've encountered a bit of a problem where the XmlElement object added to the PaginationButton's text object by ApplyAttributes->tagHandler->SetInstance
            // is intercepting and preventing click events from being fired for these buttons
            // I've tried to figure out a way to prevent this from happening entirely, as it is an issue I'd like to resolve, but I haven't had any luck
            // so instead, what we're going to do, for now, is disable the XmlElement object on the text object (as we don't need it anyway once the attributes have been applied)            
            var xmlElement = textComponent.GetComponent<XmlElement>();

            if (xmlElement != null)
            {                
                xmlElement.enabled = false;
            }            
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            // do nothing (not even default XmlLayout event handling) - leave the events as they were set up by PagedRect
        }                
    }
}
#endif
