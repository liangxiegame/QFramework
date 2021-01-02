using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UI.Tables;

namespace UI.Xml.Tags
{
    public class ButtonTagHandler : ElementTagHandler
    {
        public override MonoBehaviour primaryComponent
        {
            get
            {
                if (currentInstanceTransform == null) return null;

                return currentInstanceTransform.GetComponent<Button>();
            }
        }

        public override void ApplyAttributes(AttributeDictionary attributes)
        {
            base.ApplyAttributes(attributes);

            var textComponent = currentInstanceTransform.GetComponentInChildren<Text>(true);
            
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

            var xmlLayoutButton = currentInstanceTransform.GetComponent<XmlLayoutButton>();

            // Button image
            if (attributes.ContainsKey("icon"))
            {
                var cell = xmlLayoutButton.IconCell;

                // position the cell on the left or the right
                var imageAlignment = attributes.ContainsKey("iconalignment")
                                                ? (ButtonIconAlignment)Enum.Parse(typeof(ButtonIconAlignment), attributes["iconalignment"])
                                                : ButtonIconAlignment.Left;

                var buttonImageWidth = attributes.ContainsKey("iconwidth") ? float.Parse(attributes["iconwidth"]) : 0;

                xmlLayoutButton.ButtonTableLayout.ColumnWidths = new List<float>() { 0, 0 };


                if (imageAlignment == ButtonIconAlignment.Left)
                {
                    cell.transform.SetAsFirstSibling();
                    xmlLayoutButton.ButtonTableLayout.ColumnWidths[0] = buttonImageWidth;
                }
                else
                {
                    cell.transform.SetAsLastSibling();
                    xmlLayoutButton.ButtonTableLayout.ColumnWidths[1] = buttonImageWidth;
                }

                xmlLayoutButton.IconComponent.preserveAspect = true;
                xmlLayoutButton.IconComponent.sprite = attributes["icon"].ToSprite();

                if (attributes.ContainsKey("iconcolor"))
                {
                    xmlLayoutButton.IconColor = attributes["iconcolor"].ToColor();
                }

                if (attributes.ContainsKey("iconhovercolor"))
                {
                    xmlLayoutButton.IconHoverColor = attributes["iconhovercolor"].ToColor();
                }

                cell.gameObject.SetActive(true);

                if (!attributes.ContainsKey("text") || String.IsNullOrEmpty(attributes["text"]))
                {
                    xmlLayoutButton.TextCell.gameObject.SetActive(false);
                }
                else
                {
                    xmlLayoutButton.TextCell.gameObject.SetActive(true);
                }
            }

            if (attributes.ContainsKey("padding"))
            {
                xmlLayoutButton.ButtonTableLayout.padding = attributes["padding"].ToRectOffset();
            }
        }
    }
}
