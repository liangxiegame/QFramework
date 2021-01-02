using System;
using UnityEngine;
using UnityEngine.UI;
using UI.Tables;

namespace UI.Xml
{
    public class XmlLayoutTooltip : MonoBehaviour
    {
        public Text TextComponent;
        public Outline OutlineComponent;
        public Image BackgroundComponent;
        public Image BorderComponent;

        protected RectTransform m_rectTransform;
        protected RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = this.transform as RectTransform;

                return m_rectTransform;
            }
        }

        public TooltipPosition tooltipPosition = TooltipPosition.Right;
        public float offsetDistance = 8f;
        public bool followMouse = false;

        void Update()
        {
            // Always on top
            this.transform.SetAsLastSibling();

            if (followMouse)
            {
                SetPositionAdjacentToCursor();
            }
        }        

        public void SetText(string text)
        {
            TextComponent.text = text;
        }

        public void SetTextColor(Color color)
        {
            TextComponent.color = color;
        }

        public void SetBackgroundColor(Color color)
        {
            BackgroundComponent.color = color;
        }

        public void SetBackgroundImage(Sprite image)
        {
            BackgroundComponent.sprite = image;
        }

        public void SetBorderColor(Color color)
        {
            BorderComponent.color = color;
        }

        public void SetBorderImage(Sprite image)
        {
            BorderComponent.sprite = image;
        }

        public void SetFontSize(int size)
        {
            TextComponent.fontSize = size;
        }

        public void SetFont(Font font)
        {
            TextComponent.font = font;
        }

        public void SetTooltipPadding(RectOffset padding)
        {
            BackgroundComponent.GetComponent<HorizontalOrVerticalLayoutGroup>().padding = padding;
        }

        public void SetTextOutlineColor(Color color)
        {
            if (color == default(Color))
            {
                OutlineComponent.enabled = false;
            }
            else
            {
                OutlineComponent.enabled = true;
                OutlineComponent.effectColor = color;
            }
        }

        public void SetStylesFromXmlElement(XmlElement element)
        {            
            LoadAttributes(element.attributes);
        }

        public void SetPositionAdjacentTo(XmlElement element)
        {            
            // Set our position directly over the target element
            rectTransform.position = element.rectTransform.position;

            rectTransform.pivot = GetPivotForPosition(tooltipPosition);

            // then calculate and apply an offset based on tooltipPosition
            var offset = GetTooltipOffset(element, tooltipPosition, offsetDistance);            

            rectTransform.position = rectTransform.position + (Vector3)offset;

            ClampWithinCanvas();
        }

        public void SetPositionAdjacentToCursor()
        {            
            rectTransform.pivot = GetPivotForPosition(tooltipPosition);
            rectTransform.position = Input.mousePosition;

            var offset = new Vector2();
            
            switch (tooltipPosition)
            {
                case TooltipPosition.Above:
                    offset.y = offsetDistance;
                    break;
                case TooltipPosition.Below:
                    offset.y = -offsetDistance - rectTransform.rect.height;
                    break;
                case TooltipPosition.Left:
                    offset.x = -offsetDistance;
                    break;
                case TooltipPosition.Right:
                    offset.x = offsetDistance;
                    break;
            }
            
            rectTransform.position = rectTransform.position + (Vector3)offset;

            ClampWithinCanvas();
        }

        protected void ClampWithinCanvas()
        {
            var canvasRect = ((RectTransform)GetComponentInParent<Canvas>().transform).rect;

            var minPosition = canvasRect.min - rectTransform.rect.min;
            var maxPosition = canvasRect.max - rectTransform.rect.max;

            var clampedPosition = new Vector3();

            clampedPosition.x = Mathf.Clamp(rectTransform.anchoredPosition.x, minPosition.x, maxPosition.x);
            clampedPosition.y = Mathf.Clamp(rectTransform.anchoredPosition.y, minPosition.y, maxPosition.y);

            rectTransform.anchoredPosition = clampedPosition;
        }

        public enum TooltipPosition
        {
            Above,
            Below,
            Left,
            Right            
        }

        protected Vector2 GetTooltipOffset(XmlElement element, TooltipPosition position, float tooltipOffsetDistance)
        {            
            float desiredXChange = 0, desiredYChange = 0;

            Vector3[] elementCorners = new Vector3[4], tooltipCorners = new Vector3[4];
            ((RectTransform)element.rectTransform).GetWorldCorners(elementCorners);
            rectTransform.GetWorldCorners(tooltipCorners);

            switch (position)
            {
                case TooltipPosition.Above:
                    {
                        // distance from the element's bottom edge to the parent's top edge
                        var parentTopEdge = elementCorners[2].y;
                        var elementBottomEdge = tooltipCorners[0].y;

                        desiredYChange = parentTopEdge - elementBottomEdge + tooltipOffsetDistance;
                    }
                    break;

                case TooltipPosition.Below:
                    {
                        // distance from the element's top edge to the parent's bottom edge
                        var parentBottomEdge = elementCorners[3].y;
                        var elementTopEdge = tooltipCorners[1].y;

                        desiredYChange = parentBottomEdge - elementTopEdge - tooltipOffsetDistance;
                    }
                    break;

                case TooltipPosition.Left:
                    {
                        // distance from the element's right edge to the parent's left edge
                        var parentLeftEdge = elementCorners[0].x;
                        var elementRightEdge = tooltipCorners[3].x;

                        desiredXChange = parentLeftEdge - elementRightEdge - tooltipOffsetDistance;
                    }
                    break;

                case TooltipPosition.Right:
                    {
                        // distance from the element's left edge to the parent's right edge
                        var parentRightEdge = elementCorners[3].x;
                        var elementLeftEdge = tooltipCorners[0].x;

                        desiredXChange = parentRightEdge - elementLeftEdge + tooltipOffsetDistance;
                    }
                    break;
            }

            return new Vector2(desiredXChange, desiredYChange);
        }

        protected Vector2 GetPivotForPosition(TooltipPosition position)
        {
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            switch (position)
            {
                case TooltipPosition.Above:
                case TooltipPosition.Below:
                    pivot = new Vector2(0.5f, 0);
                    break;
                case TooltipPosition.Left:
                    pivot = new Vector2(1, 0.5f);
                    break;
                case TooltipPosition.Right:
                    pivot = new Vector2(0, 0.5f);
                    break;
            }

            return pivot;
        }

        public void LoadAttributes(AttributeDictionary attributes)
        {
            if (attributes.ContainsKey("tooltipTextColor")) SetTextColor(attributes["tooltipTextColor"].ToColor());
            if (attributes.ContainsKey("tooltipBackgroundColor")) SetBackgroundColor(attributes["tooltipBackgroundColor"].ToColor());
            if (attributes.ContainsKey("tooltipBorderColor")) SetBorderColor(attributes["tooltipBorderColor"].ToColor());

            if (attributes.ContainsKey("tooltipBackgroundImage")) SetBackgroundImage(attributes["tooltipBackgroundImage"].ToSprite());
            if (attributes.ContainsKey("tooltipBorderImage")) SetBorderImage(attributes["tooltipBorderImage"].ToSprite());

            if (attributes.ContainsKey("tooltipFontSize")) SetFontSize(int.Parse(attributes["tooltipfontsize"]));
            if (attributes.ContainsKey("tooltipPadding")) SetTooltipPadding(attributes["tooltipPadding"].ToRectOffset());

            if (attributes.ContainsKey("tooltipTextOutlineColor")) SetTextOutlineColor(attributes["tooltipTextOutlineColor"].ToColor());
            if (attributes.ContainsKey("tooltipFont")) SetFont(attributes["tooltipFont"].ToFont());

            if (attributes.ContainsKey("tooltipPosition")) tooltipPosition = (TooltipPosition)Enum.Parse(typeof(TooltipPosition), attributes["tooltipPosition"]);
            if (attributes.ContainsKey("tooltipFollowMouse")) followMouse = attributes["tooltipFollowMouse"].ToBoolean();
            if (attributes.ContainsKey("tooltipOffset")) offsetDistance = float.Parse(attributes["tooltipOffset"]);            
        }
    }
}
