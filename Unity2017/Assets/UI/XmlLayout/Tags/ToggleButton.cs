using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UI.Xml;

namespace UI.Xml.Tags
{
    public class ToggleButtonTagHandler : ButtonTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;
                
                return currentInstanceTransform.GetComponent<XmlLayoutToggleButton>();
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

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            base.ApplyAttributes(attributes);

            var toggleComponent = currentInstanceTransform.GetComponent<Toggle>();

            if (attributes.ContainsKey("colors"))
            {
                toggleComponent.colors = attributes["colors"].ToColorBlock();
            }

            if (attributes.ContainsKey("ison"))
            {
                toggleComponent.isOn = attributes["ison"].ToBoolean();                
            }

            if (ToggleGroupTagHandler.CurrentToggleGroupInstance != null)
            {
                var xmlLayoutToggleGroupInstance = ToggleGroupTagHandler.CurrentToggleGroupInstance;

                xmlLayoutToggleGroupInstance.AddToggle(toggleComponent);
                xmlLayoutToggleGroupInstance.UpdateToggleElement(toggleComponent);

                toggleComponent.onValueChanged.AddListener((e) =>
                {
                    if (e)
                    {
                        var value = xmlLayoutToggleGroupInstance.GetValueForElement(toggleComponent);
                        xmlLayoutToggleGroupInstance.SetSelectedValue(value);
                    }                    
                });
            }
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                    {
                        var toggle = primaryComponent.GetComponent<Toggle>();
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        toggle.onValueChanged.AddListener((e) =>
                        {
                            string _value = value;
                            var valueLower = value.ToLower();

                            if (valueLower == "selectedvalue")
                            {
                                _value = e.ToString();
                            }

                            layout.XmlLayoutController.ReceiveMessage(eventData[0], _value, transform);
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
            return element.GetComponent<Toggle>().isOn.ToString();
        }
    }
}