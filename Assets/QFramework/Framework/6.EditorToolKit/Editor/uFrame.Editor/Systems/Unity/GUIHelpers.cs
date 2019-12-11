
using System;
using System.Collections.Generic;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace Invert.Common.UI
{
    public class GUIHelpers
    {
        // loads a png resources from the dll
        public static Rect GetRect(GUIStyle style, bool fullWidth = true, params GUILayoutOption[] options)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, style, options);
            if (!fullWidth) return rect;
            //var indentAmount = (Indent * 25);
            rect.x -= 13;
            //rect.x += +(indentAmount);
            rect.width += 17;
            //rect.width -= indentAmount;
            rect.y += 3;
            return rect;
        }
        public static bool DoToolbar(string label, 
            bool open, 
            Action add = null, 
            Action leftButton = null, 
            Action paste = null, 
            GUIStyle addButtonStyle = null, 
            GUIStyle pasteButtonStyle = null,
            bool fullWidth = true, Color? textColor = null)
        {
            var rect = GetRect(open ? ElementDesignerStyles.ToolbarStyle : ElementDesignerStyles.ToolbarStyleCollapsed, GUIHelpers.IsInsepctor);
            return DoToolbar(label, open, rect, add, leftButton, paste, addButtonStyle, pasteButtonStyle, fullWidth, textColor);
        }

        public static bool DoToolbar(string label,
           bool open, 
           Rect forceRect,
           Action add = null,
           Action leftButton = null,
           Action paste = null,
           GUIStyle addButtonStyle = null,
           GUIStyle pasteButtonStyle = null,
           bool fullWidth = true, Color? textColor = null)
        {
            var rect = forceRect;
            GUI.Box(rect, "", open ? ElementDesignerStyles.ToolbarStyle : ElementDesignerStyles.ToolbarStyleCollapsed);
            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                normal = new GUIStyleState() { textColor = textColor ?? ElementDesignerStyles.ToolbarStyle.normal.textColor },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                fontSize = 11
            };

            var labelRect = new Rect(rect.x + 2, rect.y + (rect.height / 2) - 8, rect.width - (add != null ? 50 : 0), 16);
            var result = open;
            if (leftButton == null)
            {
                result = GUI.Button(labelRect,
                    new GUIContent(label,
                        open ? ElementDesignerStyles.ArrowDownTexture : ElementDesignerStyles.CollapseRightArrowTexture),
                    labelStyle);
            }
            else
            {
                if (GUI.Button(labelRect, new GUIContent(label, ElementDesignerStyles.ArrowLeftTexture), labelStyle))
                {
                    leftButton();
                }
            }

            if (paste != null)
            {
                var addButtonRect = new Rect(rect.x + rect.width - 42, rect.y + (rect.height / 2) - 8, 16, 16);
                if (GUI.Button(addButtonRect, "", pasteButtonStyle ?? ElementDesignerStyles.PasteButtonStyle))
                {
                    paste();
                }
            }

            if (add != null)
            {
                var addButtonRect = new Rect(rect.x + rect.width - 21, rect.y + (rect.height / 2) - 8, 16, 16);
                if (GUI.Button(addButtonRect, "", addButtonStyle ?? ElementDesignerStyles.AddButtonStyleUnscaled))
                {
                    add();
                }
            }
            return result;
        }

        public static bool IsInsepctor { get; set; }

        public static bool DoToolbar(string label, Action add = null, Action leftButton = null, Action paste = null)
        {
            return DoToolbar(label, true, add, leftButton, paste);
        }
        public static bool DoToolbarEx(string label, Action add = null, Action leftButton = null, Action paste = null, Action clicked = null, bool defOn = true, Color? color = null)
        {
            if (!EditorPrefs.HasKey(label))
            {
                EditorPrefs.SetBool(label,defOn);
            }
            var tBar = DoToolbar(label, EditorPrefs.GetBool(label, true), add, leftButton, paste,null,null,true,color);
            if (tBar)
            {
                if (clicked != null)
                clicked();
                EditorPrefs.SetBool(label,!EditorPrefs.GetBool(label));
            }
            return EditorPrefs.GetBool(label);
        }

        public static bool DoToolbarEx(string label,Rect rect, Action add = null, Action leftButton = null, Action paste = null, Action clicked = null, bool defOn = true, Color? color = null)
        {
            if (!EditorPrefs.HasKey(label))
            {
                EditorPrefs.SetBool(label, defOn);
            }
            var tBar = DoToolbar(label, EditorPrefs.GetBool(label, true), rect, add, leftButton, paste, null, null, true, color);
            if (tBar)
            {
                if (clicked != null)
                    clicked();
                EditorPrefs.SetBool(label, !EditorPrefs.GetBool(label));
            }
            return EditorPrefs.GetBool(label);
        }


        public static void DoArray<TType>(string label, IEnumerable<TType> items, Action<TType> removeItem, Action add) where TType : IItem
        {
            if (DoToolbarEx(label, add))
            {
                foreach (var item in items)
                {
                    var item1 = item;
                    DoTriggerButton(new UFStyle(item.Title, ElementDesignerStyles.EventButtonStyleSmall, null,
                        ElementDesignerStyles.RemoveButtonStyle, () =>
                        {
                            removeItem(item1);
                        }));
                }
            }
        }
        public static bool DoTriggerButton(UFStyle ubTriggerContent)
        {
            var hasSubLabel = !String.IsNullOrEmpty(ubTriggerContent.SubLabel);

            var rect = GetRect(ubTriggerContent.BackgroundStyle,ubTriggerContent.FullWidth && !ubTriggerContent.IsWindow);

            var style = ubTriggerContent.BackgroundStyle;

            if (UFStyle.MouseDownStyle != null && ubTriggerContent.IsMouseDown(rect))
                style = UFStyle.MouseDownStyle;

            if (!ubTriggerContent.Enabled)
            {
                style = GUIStyle.none;
            }

            GUI.Box(rect, "", style);

            if (ubTriggerContent.MarkerStyle != null)
            {
                var rectIndicator = new Rect(rect);
                rectIndicator.width = 2;
                rectIndicator.y -= 2;
                rectIndicator.x = rect.width - 2;
                rectIndicator.height -= 3;
                GUI.Box(rectIndicator, "", ubTriggerContent.MarkerStyle);
            }
            if (ubTriggerContent.IconStyle != null )
            {
                var eventOptionsButtonRect = new Rect(rect.x + 5, rect.y + ((rect.height / 2) - 8), 16, 16);
                if (GUI.Button(eventOptionsButtonRect, "", ubTriggerContent.IconStyle))
                {
                    if (ubTriggerContent.OnShowOptions != null)
                    ubTriggerContent.OnShowOptions();
                }
                var seperatorRect = new Rect(rect) {width = 3};
                seperatorRect.y += 2;
                seperatorRect.height -= 5;
                seperatorRect.x = eventOptionsButtonRect.x + 17;
                GUI.Box(seperatorRect, String.Empty, ElementDesignerStyles.SeperatorStyle);
            }

            var labelStyle =  new GUIStyle(EditorStyles.label) { alignment = ubTriggerContent.TextAnchor, fontSize = 11,fontStyle = FontStyle.Bold,normal = new GUIStyleState() { textColor = ubTriggerContent.BackgroundStyle.normal.textColor}};
            if (!ubTriggerContent.Enabled)
            {
                labelStyle.normal.textColor = new Color(0.4f,0.4f,0.4f);
                
            }
            var labelRect = new Rect(rect.x, rect.y - (hasSubLabel ? 6 : 0), rect.width - 30, rect.height);
            var lbl = ubTriggerContent.Label;
            var result = GUI.Button(labelRect, lbl, labelStyle);

            if (hasSubLabel)
            {
                var subLabelRect = new Rect(labelRect);
                subLabelRect.y += 18;
                subLabelRect.width -= 26;
                subLabelRect.x += 13;
                GUI.Label(subLabelRect, ubTriggerContent.SubLabel, UFStyle.SubLabelStyle);
            }
            if (ubTriggerContent.ShowArrow)
                GUI.DrawTexture(new Rect(rect.x + rect.width - 18f, rect.y + ((rect.height / 2) - 8), 16, 16), ElementDesignerStyles.ArrowRightTexture2);
            if (ubTriggerContent.Enabled)
            {
                return result;
            }
            return result;
        }

        public static bool DoToggle(string name, bool on)
        {
            if (DoTriggerButton(new UFStyle()
            {
                Label = name,
                BackgroundStyle = ElementDesignerStyles.EventButtonStyleSmall,
                IconStyle = on ? ElementDesignerStyles.TriggerActiveButtonStyle : ElementDesignerStyles.TriggerInActiveButtonStyle
            }))
            {
                return !on;
            }
            return on;
        }
    }
}
