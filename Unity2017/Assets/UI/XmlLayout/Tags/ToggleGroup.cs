using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

namespace UI.Xml.Tags
{
    public class ToggleGroupTagHandler : ElementTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<XmlLayoutToggleGroup>();
            }
        }

        private List<string> _eventAttributeNames = new List<string>()
        {
            "onClick",
            "onMouseEnter",
            "onMouseExit",
            "onValueChanged"
        };

        protected override List<string> eventAttributeNames
        {
            get
            {
                return _eventAttributeNames;
            }
        }

        private static XmlLayoutToggleGroup previousToggleGroupInstance = null;
        public static XmlLayoutToggleGroup CurrentToggleGroupInstance;

        public override void Open(AttributeDictionary attributes)
        {
            base.Open(attributes);

            previousToggleGroupInstance = CurrentToggleGroupInstance;
            CurrentToggleGroupInstance = this.primaryComponent as XmlLayoutToggleGroup;
        }

        public override void Close()
        {            
            base.Close();

            //CurrentToggleGroupInstance.SetSelectedValue(CurrentToggleGroupInstance.GetSelectedValue(), false);

            CurrentToggleGroupInstance = previousToggleGroupInstance;
        }        
        
        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                    {
                        var xmlLayoutToggleGroup = (XmlLayoutToggleGroup)primaryComponent;
                        var transform = currentInstanceTransform;                        

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                                  .Split(',', ' ', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        xmlLayoutToggleGroup.AddOnValueChangedEventHandler((int e) =>
                            {
                                string _value = value;
                                var valueLower = value.ToLower();

                                if (valueLower == "selectedvalue")
                                {
                                    _value = e.ToString();
                                }
                                else if (valueLower == "selectedtext")
                                {
                                    _value = xmlLayoutToggleGroup.GetTextValueForIndex(e);
                                }

                                currentXmlLayoutInstance.XmlLayoutController.ReceiveMessage(eventData[0], _value, transform);
                            });
                    }
                    break;

                default:
                    base.HandleEventAttribute(eventName, eventValue);
                    break;
            }
        }

        public string GetValue(XmlElement element)
        {
            return element.GetComponent<XmlLayoutToggleGroup>().GetSelectedValue().ToString();
        }
    }
}
