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
    public class PaginationButtonTemplateTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return null;
            }
        }             

        /// <summary>
        /// This is a custom element, add it to the XSD file
        /// </summary>
        public override bool isCustomElement
        {
            get
            {
                return true;
            }
        }

        public override bool renderElement
        {
            get
            {
                return false;
            }
        }

        public override string prefabPath
        {
            get
            {
                return null; // don't use a prefab, just an empty gameobject
            }
        }

        /// <summary>
        /// This element may only be a child of PagedRect
        /// </summary>
        public override string elementGroup
        {
            get
            {
                return "pagination";
            }
        }

        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"textColor", "xmlLayout:color"},
                    {"color", "xmlLayout:color"},
                    {"colors", "xmlLayout:colorblock"},
                    {"type", "CurrentPage,OtherPages,DisabledPages,Next,Previous,First,Last"},
                    {"font", "xs:string"},
                    {"fontSize", "xs:float"},
                    {"fontStyle", "Normal,Bold,Italic,BoldAndItalic"}
                };
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {                        
            var pagedRectTagHandler = PagedRectTagHandler.CurrentPagedRectTagHandler;

            if (pagedRectTagHandler == null)
            {
                Debug.Log("[XmlLayout] Error: PaginationButtonTemplate: Unable to locate PagedRect instance.");
            }
            else
            {
                var pagedRectInstance = pagedRectTagHandler.currentInstanceTransform.GetComponent<PagedRect>();

                var type = attributes.GetValue("type");

                Button buttonInstance = null;

                switch (type.ToLower())
                {
                    case "currentpage":
                        if (pagedRectInstance.ButtonTemplate_CurrentPage != null) buttonInstance = pagedRectInstance.ButtonTemplate_CurrentPage.Button;
                        break;
                    case "otherpages":
                        if (pagedRectInstance.ButtonTemplate_OtherPages != null) buttonInstance = pagedRectInstance.ButtonTemplate_OtherPages.Button;
                        break;
                    case "disabledpages":
                        if(pagedRectInstance.ButtonTemplate_DisabledPage != null) buttonInstance = pagedRectInstance.ButtonTemplate_DisabledPage.Button;
                        break;

                    case "next":
                        if (pagedRectInstance.Button_NextPage != null) buttonInstance = pagedRectInstance.Button_NextPage.Button;
                        break;
                    case "previous":
                        if (pagedRectInstance.Button_PreviousPage != null) buttonInstance = pagedRectInstance.Button_PreviousPage.Button;
                        break;
                    case "first":
                        if (pagedRectInstance.Button_FirstPage != null) buttonInstance = pagedRectInstance.Button_FirstPage.Button;
                        break;
                    case "last":
                        if (pagedRectInstance.Button_LastPage != null) buttonInstance = pagedRectInstance.Button_LastPage.Button;
                        break;
                }

                if (buttonInstance == null)
                {
                    Debug.Log("[XmlLayout][PaginationButtonTemplate] Unable to locate button template for '" + type + "'");
                }
                else
                {
                    var tagHandler = XmlLayoutUtilities.GetXmlTagHandler("PaginationButton");
                    tagHandler.SetInstance(buttonInstance.transform as RectTransform, this.currentXmlLayoutInstance);
                    attributes.Remove("type");                    
                    tagHandler.ApplyAttributes(attributes);
                }
            }            
        }
    }
}
#endif
