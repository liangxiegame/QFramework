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
    public class PaginationTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<HorizontalOrVerticalLayoutGroup>();
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
                return "pagedRect";
            }
        }

        public override string elementChildType
        {
            get
            {
                return "pagination";
            }
        }        

        public override void ApplyAttributes(AttributeDictionary attributes)
        {                        
            var pagedRectTagHandler = PagedRectTagHandler.CurrentPagedRectTagHandler;

            if (pagedRectTagHandler == null)
            {
                Debug.Log("[XmlLayout] Error: Pagination: Unable to locate PagedRect instance.");
            }
            else
            {                
                var pagedRectInstance = pagedRectTagHandler.currentInstanceTransform.GetComponent<PagedRect>();

                var sizeAttributes = new string[] { "width", "height" };
                if (attributes.Any(a => sizeAttributes.Contains(a.Key)))
                {
                    // try and preserve default positioning of the pagination container if width or height attributes are provided
                    if (!attributes.ContainsKey("rectAlignment"))
                    {                        
                        if (pagedRectTagHandler.tagType.Contains("Vertical"))
                        {
                            attributes.Add("rectAlignment", "MiddleLeft");
                        }
                        else
                        {
                            attributes.Add("rectAlignment", "LowerCenter");
                        }
                    }

                    var viewportRectTransform = (RectTransform)pagedRectInstance.GetComponentInChildren<Viewport>().transform;
                    // try and resize the viewport if the pagination container size changes
                    if (pagedRectTagHandler.tagType.Contains("Vertical"))
                    {
                        if (attributes.ContainsKey("width"))
                        {
                            var rectAlignment = attributes.GetValue("rectAlignment") ?? "MiddleLeft";

                            if (rectAlignment.Contains("Left"))
                            {
                                viewportRectTransform.offsetMin = new Vector2(attributes.GetValue<float>("width"), viewportRectTransform.offsetMin.y);
                                viewportRectTransform.offsetMax = new Vector2(0, viewportRectTransform.offsetMax.y);
                            }
                            else if (rectAlignment.Contains("Right"))
                            {
                                viewportRectTransform.offsetMin = new Vector2(0, viewportRectTransform.offsetMin.y);
                                viewportRectTransform.offsetMax = new Vector2(-attributes.GetValue<float>("width"), viewportRectTransform.offsetMax.y);
                            }
                        }
                    }
                    else
                    {
                        if (attributes.ContainsKey("height"))
                        {
                            var rectAlignment = attributes.GetValue("rectAlignment") ?? "LowerCenter";

                            if (rectAlignment.Contains("Lower"))
                            {
                                viewportRectTransform.offsetMin = new Vector2(viewportRectTransform.offsetMin.x, attributes.GetValue<float>("height"));
                                viewportRectTransform.offsetMax = new Vector2(viewportRectTransform.offsetMax.x, 0);
                            }
                            else if (rectAlignment.Contains("Upper"))
                            {
                                viewportRectTransform.offsetMin = new Vector2(viewportRectTransform.offsetMin.x, 0);
                                viewportRectTransform.offsetMax = new Vector2(viewportRectTransform.offsetMax.x, -attributes.GetValue<float>("height"));
                            }
                        }
                    }

                }

                
                var pagination = pagedRectInstance.Pagination;

                var backupTransform = this.currentInstanceTransform;
                this.SetInstance(pagination.transform as RectTransform, this.currentXmlLayoutInstance);
                base.ApplyAttributes(attributes);
                this.SetInstance(backupTransform, this.currentXmlLayoutInstance);
            }
        }

        public override Dictionary<string, string> attributes
        {
            get    
            {
                return new Dictionary<string, string>()
                {
                    {"padding", "xmlLayout:rectOffset"},
                    {"spacing", "xs:float"},
                    {"childForceExpandHeight", "xs:boolean"},
                    {"childForceExpandWidth", "xs:boolean"},
                    {"childAlignment", "xmlLayout:alignment"},
                    {"color", "xmlLayout:color"}
                };
            }
        }
    }
}
#endif
