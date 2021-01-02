using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

namespace UI.Xml.Tags
{
    public class SliderTagHandler : ElementTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Slider>();
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

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                    {
                        var slider = (Slider)primaryComponent;
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        slider.onValueChanged.AddListener((e) =>
                        {
                            string methodName = eventData[0];
                            string _value = value;
                            var valueLower = value.ToLower();

                            if (valueLower == "selectedvalue")
                            {
                                _value = e.ToString();
                            }

                            layout.XmlLayoutController.ReceiveMessage(methodName, _value, transform);
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
            return element.GetComponent<Slider>().value.ToString();
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            base.ApplyAttributes(attributes);

            var slider = primaryComponent.GetComponent<XmlLayoutSlider>();

            if (attributes.ContainsKey("backgroundcolor"))
            {
                slider.Background.color = attributes["backgroundcolor"].ToColor();
            }

            if (attributes.ContainsKey("backgroundimage"))
            {
                slider.Background.sprite = attributes["backgroundimage"].ToSprite();
            }

            if (attributes.ContainsKey("fillcolor"))
            {
                slider.Fill.color = attributes["fillcolor"].ToColor();
            }

            if (attributes.ContainsKey("fillimage"))
            {
                slider.Fill.sprite = attributes["fillimage"].ToSprite();
            }

            var handle = slider.Slider.targetGraphic as Image;
            if (attributes.ContainsKey("handleimage"))
            {
                handle.sprite = attributes["handleimage"].ToSprite();
            }

            if (attributes.ContainsKey("handlepreserveaspect"))
            {
                handle.preserveAspect = attributes["handlepreserveaspect"].ToBoolean();
            }
        }
    }
}
