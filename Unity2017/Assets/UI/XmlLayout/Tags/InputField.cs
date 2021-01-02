using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.Events;

namespace UI.Xml.Tags
{
    public class InputFieldTagHandler : InputBaseTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<InputField>();
            }
        }

        private List<string> _eventAttributeNames = new List<string>()
        {
            "onClick",
            "onMouseEnter",
            "onMouseExit",
            "onValueChanged",
            "onEndEdit"
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

            var inputField = primaryComponent as InputField;

            var textComponents = new List<Text> { inputField.textComponent };
            if (inputField.placeholder != null)
            {
                var placeholderText = inputField.placeholder.GetComponent<Text>();
                if (placeholderText != null)
                {
                    textComponents.Add(placeholderText);
                }

                if (attributes.ContainsKey("placeholdertext"))
                {
                    placeholderText.text = attributes["placeholdertext"];
                }
            }

            foreach (var textComponent in textComponents)
            {
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
            }
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                case "onendedit":
                    {
                        var inputField = (InputField)primaryComponent;
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        var listener = new UnityAction<string>(
                            (e) =>
                            {
                                string _value = value;
                                var valueLower = value.ToLower();

                                if (valueLower == "value")
                                {
                                    _value = e.ToString();
                                }

                                layout.XmlLayoutController.ReceiveMessage(eventData[0], _value, transform);
                            });

                        if (eventName == "onvaluechanged")
                        {
                            inputField.onValueChanged.AddListener(listener);
                        }
                        else if (eventName == "onendedit")
                        {
                            inputField.onEndEdit.AddListener(listener);
                        }
                    }
                    break;

                default:
                    base.HandleEventAttribute(eventName, eventValue);
                    break;
            }
        }

        public string GetValue(XmlElement element)
        {
            return element.GetComponent<InputField>().text;
        }
    }
}
