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
    public class PageTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                return currentInstanceTransform.GetComponent<Page>();
            }
        }
        
        public override string prefabPath
        {
            get
            {
                return "XmlLayout Prefabs/PagedRect/Page";
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

        public override Dictionary<string, string> attributes
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"pageTitle", "xs:string"},
                    {"pageEnabled", "xs:boolean"},
                    {"showOnPagination", "xs:boolean"},
                    {"usePageAnimationType", "xs:boolean"},
                    {"animationType", "xs:string"},             // ideally this should actually be an enum (XML: SimpleType with a restriction), but let's worry about that later
                    {"flipAnimationDirection", "xs:boolean"},
                    {"onShowEvent", "xs:string"},
                    {"onHideEvent", "xs:string"}
                };
            }
        }


        private List<string> _eventAttributeNames = new List<string>()
        {
            "onClick",
            "onMouseEnter",
            "onMouseExit",
            "onShowEvent",
            "onHideEvent"
        };

        protected override List<string> eventAttributeNames
        {
            get
            {
                return _eventAttributeNames;
            }
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onshowevent":
                case "onhideevent":
                    {
                        var page = (Page)primaryComponent;
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        var action = new UnityEngine.Events.UnityAction(
                            () => 
                            {
                                string methodName = eventData[0];
                                string _value = value;                                

                                layout.XmlLayoutController.ReceiveMessage(methodName, _value, transform);
                            });

                        if (eventName == "onshowevent") page.OnShowEvent.AddListener(action);                        
                        else if(eventName == "onhideevent") page.OnHideEvent.AddListener(action);                        
                    }
                    break;
                default:
                    base.HandleEventAttribute(eventName, eventValue);
                    break;
            }            
        }
    }
}
#endif
