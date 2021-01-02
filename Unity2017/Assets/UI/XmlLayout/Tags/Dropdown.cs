using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

namespace UI.Xml.Tags
{
    public class DropdownTagHandler : ElementTagHandler, IHasXmlFormValue
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Dropdown>();
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

            var dropdown = primaryComponent as Dropdown;
            var templateComponent = dropdown.template.GetComponent<Image>();
            var itemTemplateComponent = dropdown.template.GetComponentInChildren<Toggle>();
            var textComponent = dropdown.captionText;

            var textTagHandler = XmlLayoutUtilities.GetXmlTagHandler("Text");
            textTagHandler.SetInstance(textComponent.rectTransform, this.currentXmlLayoutInstance);

            var textAttributes = new AttributeDictionary(
                                            attributes.Where(a => TextTagHandler.TextAttributes.Contains(a.Key, StringComparer.OrdinalIgnoreCase))
                                                      .ToDictionary(a => a.Key, b => b.Value));

            if (attributes.ContainsKey("textshadow")) textAttributes.Add("shadow", attributes["textshadow"]);
            if (attributes.ContainsKey("textoutline")) textAttributes.Add("outline", attributes["textoutline"]);
            if (attributes.ContainsKey("textcolor")) textAttributes.Add("color", attributes["textcolor"]);
            if (attributes.ContainsKey("textalignment")) textAttributes.Add("alignment", attributes["textalignment"]);

            textTagHandler.ApplyAttributes(textAttributes);
            // disable the XmlElement component, it can interfere with mouse clicks/etc.
            textComponent.GetComponent<XmlElement>().enabled = false;

            var xmlLayoutDropdown = dropdown.GetComponent<XmlLayoutDropdown>();
            var arrow = xmlLayoutDropdown.Arrow;
            if (attributes.ContainsKey("arrowimage"))
            {
                arrow.sprite = attributes["arrowimage"].ToSprite();
            }

            if (attributes.ContainsKey("arrowcolor"))
            {
                arrow.color = attributes["arrowcolor"].ToColor();
            }

            // Apply text attributes to the item template
            var itemTemplate = xmlLayoutDropdown.ItemTemplate;
            var itemTagHandler = XmlLayoutUtilities.GetXmlTagHandler("Toggle");
            var itemAttributes = attributes.Clone();

            if (attributes.ContainsKey("itemheight"))
            {
                (itemTemplate.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, float.Parse(attributes["itemheight"]));
            }

            if (attributes.ContainsKey("checkcolor"))
            {
                itemAttributes.Add("togglecheckmarkcolor", attributes["checkcolor"]);
            }

            itemTagHandler.SetInstance(itemTemplate.transform as RectTransform, this.currentXmlLayoutInstance);
            itemTagHandler.ApplyAttributes(itemAttributes);

            if (attributes.ContainsKey("itembackgroundcolors"))
            {
                itemTemplateComponent.colors = attributes["itembackgroundcolors"].ToColorBlock();
            }

            if (attributes.ContainsKey("dropdownbackgroundcolor"))
            {
                dropdown.template.GetComponent<Image>().color = attributes["dropdownbackgroundcolor"].ToColor();
            }

            if (attributes.ContainsKey("dropdownbackgroundimage"))
            {
                dropdown.template.GetComponent<Image>().sprite = attributes["dropdownbackgroundimage"].ToSprite();
            }

            if (attributes.ContainsKey("itemtextcolor"))
            {
                var itemTextComponent = dropdown.itemText;
                itemTextComponent.color = attributes["itemtextcolor"].ToColor();
            }

            if (attributes.ContainsKey("scrollbarcolors"))
            {
                xmlLayoutDropdown.DropdownScrollbar.colors = attributes["scrollbarcolors"].ToColorBlock();
            }

            if (attributes.ContainsKey("scrollbarimage"))
            {
                xmlLayoutDropdown.DropdownScrollbar.image.sprite = attributes["scrollbarimage"].ToSprite();
            }

            if (attributes.ContainsKey("scrollbarbackgroundcolor"))
            {
                xmlLayoutDropdown.DropdownScrollbar.GetComponent<Image>().color = attributes["scrollbarbackgroundcolor"].ToColor();
            }

            if (attributes.ContainsKey("scrollbarbackgroundimage"))
            {
                xmlLayoutDropdown.DropdownScrollbar.GetComponent<Image>().sprite = attributes["scrollbarbackgroundimage"].ToSprite();
            }

            foreach (var attribute in attributes)
            {
                SetPropertyValue(templateComponent, attribute.Key, attribute.Value);
            }
        }

        public override void Close()
        {
            base.Close();

            (primaryComponent as Dropdown).RefreshShownValue();
        }

        public override bool ParseChildElements(XmlNode xmlNode)
        {
            var dropdown = (Dropdown)primaryComponent;
            dropdown.value = 0;

            int x = 0;
            int value = -1;
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.ToLower() != "option") continue;

                var optionName = node.InnerText;

                dropdown.options.Add(new Dropdown.OptionData { text = optionName });

                var attributes = node.Attributes.ToAttributeDictionary();

                if (attributes.ContainsKey("selected"))
                {
                    try
                    {
                        if (attributes["selected"].ToBoolean())
                        {
                            value = x;
                        }
                    }
                    catch { }
                }

                x++;
            }

            if (value >= 0)
            {
                dropdown.value = value;
                dropdown.RefreshShownValue();
            }

            return true;
        }

        protected override void HandleEventAttribute(string eventName, string eventValue)
        {
            switch (eventName)
            {
                case "onvaluechanged":
                    {
                        var dropdown = (Dropdown)primaryComponent;
                        var transform = currentInstanceTransform;
                        var layout = currentXmlLayoutInstance;

                        var eventData = eventValue.Trim(new Char[] { ')', ';' })
                                      .Split(',', '(');

                        string value = null;
                        if (eventData.Count() > 1)
                        {
                            value = eventData[1];
                        }

                        dropdown.onValueChanged.AddListener((e) =>
                        {
                            string _value = value;
                            var valueLower = value.ToLower();

                            if (valueLower == "selectedtext" || valueLower == "selectedvalue")
                            {
                                _value = dropdown.options[e].text;
                            }
                            else if (valueLower == "selectedindex")
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
            var dropDown = element.GetComponent<Dropdown>();

            return dropDown.options[dropDown.value].text;
        }

        public override void SetValue(string newValue)
        {
            if (String.IsNullOrEmpty(newValue)) return;

            var dropdown = (Dropdown)primaryComponent;

            int selectedValue = -1;
            if (int.TryParse(newValue, out selectedValue))
            {
                dropdown.SetSelectedValue(selectedValue);          
            }
            else
            {
                dropdown.SetSelectedValue(newValue);
            }
        }
    }
}
