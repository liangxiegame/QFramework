using System;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.PackageManager.UI
{
    internal static class UIUtils
    {
        private const string DisplayNone = "display-none";

        public static void SetElementDisplay(VisualElement element, bool value)
        {
            if (element == null)
                return;
            
            if (value)
                element.RemoveFromClassList(DisplayNone);
            else
                element.AddToClassList(DisplayNone);

            element.visible = value;
        }
        
        public static void SetElementDisplayNonEmpty(Label element)
        {
            if (element == null)
                return;

            var empty = string.IsNullOrEmpty(element.text);
            if (empty)
                element.AddToClassList(DisplayNone);
            else
                element.RemoveFromClassList(DisplayNone);

            element.visible = !empty;
        }

        public static void SetElementDisplayNonEmpty(Button element)
        {
            if (element == null)
                return;

            var empty = string.IsNullOrEmpty(element.text);
            if (empty)
                element.AddToClassList(DisplayNone);
            else
                element.RemoveFromClassList(DisplayNone);

            element.visible = !empty;
        }

        public static bool IsElementVisible(VisualElement element)
        {
            return element.visible && !element.ClassListContains(DisplayNone);
        }

        public static bool IsElementDisplayNone(VisualElement element)
        {
            return element.ClassListContains(DisplayNone);
        }

        public static void ShowTextTooltipOnSizeChange<T>(this T element, int deltaWidth = 0) where T : TextElement
        {
            element.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.newRect.width == evt.oldRect.width)
                    return;

                var target = evt.target as TextElement;
                if (target == null)
                    return;

                var size = target.MeasureTextSize(target.text, float.MaxValue, VisualElement.MeasureMode.AtMost, evt.newRect.height, VisualElement.MeasureMode.Undefined);
                var width = evt.newRect.width + deltaWidth;
                target.tooltip = width < size.x ? target.text : string.Empty;
            });
        }
    }
}
