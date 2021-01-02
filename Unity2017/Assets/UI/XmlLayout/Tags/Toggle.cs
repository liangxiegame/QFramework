using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

namespace UI.Xml.Tags
{
    public class ToggleTagHandler : InputBaseTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Toggle>();
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

            var toggle = primaryComponent as Toggle;
            var checkMark = toggle.graphic as Image;
            var textComponent = currentXmlElement.GetComponentInChildren<Text>();

            if (attributes.ContainsKey("checkcolor"))
            {
                checkMark.color = attributes["checkcolor"].ToColor();
            }

            var targetGraphic = toggle.targetGraphic as Image;
            if (attributes.ContainsKey("togglewidth"))
            {
                var width = float.Parse(attributes["togglewidth"]);
                targetGraphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                checkMark.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

                if (textComponent != null)
                {
                    textComponent.rectTransform.localPosition = new Vector2(width, textComponent.rectTransform.localPosition.y);
                }
            }

            if (attributes.ContainsKey("toggleheight"))
            {
                var height = float.Parse(attributes["toggleheight"]);
                targetGraphic.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                checkMark.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            if (ToggleGroupTagHandler.CurrentToggleGroupInstance != null)
            {
                var xmlLayoutToggleGroupInstance = ToggleGroupTagHandler.CurrentToggleGroupInstance;

                xmlLayoutToggleGroupInstance.AddToggle(toggle);
                xmlLayoutToggleGroupInstance.UpdateToggleElement(toggle);

                toggle.onValueChanged.AddListener((e) =>
                {
                    if (e)
                    {
                        var value = xmlLayoutToggleGroupInstance.GetValueForElement(toggle);
                        xmlLayoutToggleGroupInstance.SetSelectedValue(value);
                    }
                });
            }

            // Text attributes            
            if (textComponent != null)
            {
                var tagHandler = XmlLayoutUtilities.GetXmlTagHandler("Text");
                tagHandler.SetInstance(textComponent.rectTransform, this.currentXmlLayoutInstance);

                var textAttributes = new AttributeDictionary(
                                            attributes.Where(a => TextTagHandler.TextAttributes.Contains(a.Key, StringComparer.OrdinalIgnoreCase))
                                                      .ToDictionary(a => a.Key, b => b.Value));

                if (attributes.ContainsKey("textshadow")) textAttributes.Add("shadow", attributes["textshadow"]);
                if (attributes.ContainsKey("textoutline")) textAttributes.Add("outline", attributes["textoutline"]);
                if (attributes.ContainsKey("textcolor")) textAttributes.Add("color", attributes["textcolor"]);

                tagHandler.ApplyAttributes(textAttributes);

                // disable the XmlElement component, it can interfere with mouse clicks/etc.
                textComponent.GetComponent<XmlElement>().enabled = false;
            }

            if (attributes.ContainsKey("togglebackgroundimage"))
            {
                targetGraphic.sprite = attributes["togglebackgroundimage"].ToSprite();
            }

            if (attributes.ContainsKey("togglecheckmarkimage"))
            {
                checkMark.sprite = attributes["togglecheckmarkimage"].ToSprite();
            }
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                    {
                        var toggle = (Toggle)primaryComponent;
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
