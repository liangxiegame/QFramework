using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QF.GraphDesigner
{
    public static class PlatformDrawerExtensions
    {
        //public static void DoVertical(float startX, float startY, float width, float itemHeight, params Action<Rect>[] rows)
        //{

        //}
        //public static bool DoToolbar(this IPlatformDrawer drawer, Rect rect, string label, bool open, Action add = null, Action leftButton = null, Action paste = null, GUIStyle addButtonStyle = null, GUIStyle pasteButtonStyle = null, bool fullWidth = true)
        //{
        //    var style =  open ? CachedStyles.Toolbar : CachedStyles.ToolbarButton;
        //    drawer.DrawStretchBox(rect, style ,0f);

        //    var labelRect = new Rect(rect.x + 2, rect.y + (rect.height / 2) - 8, rect.width - (add != null ? 50 : 0), 16);
        //    var result = open;
        //    if (leftButton == null)
        //    {
        //        drawer.DoButton(rect, label, style, () =>
        //        {

        //        });
        //    }
        //    else
        //    {
        //        if (GUI.Button(labelRect, new GUIContent(label, ElementDesignerStyles.ArrowLeftTexture), labelStyle))
        //        {
        //            leftButton();
        //        }
        //    }

        //    if (paste != null)
        //    {
        //        var addButtonRect = new Rect(rect.x + rect.width - 42, rect.y + (rect.height / 2) - 8, 16, 16);
        //        if (GUI.Button(addButtonRect, "", pasteButtonStyle ?? ElementDesignerStyles.PasteButtonStyle))
        //        {
        //            paste();
        //        }
        //    }

        //    if (add != null)
        //    {
        //        var addButtonRect = new Rect(rect.x + rect.width - 21, rect.y + (rect.height / 2) - 8, 16, 16);
        //        if (GUI.Button(addButtonRect, "", addButtonStyle ?? ElementDesignerStyles.AddButtonStyleUnscaled))
        //        {
        //            add();
        //        }
        //    }
        //    return result;
        //}

        //public static bool DoSectionBar(this IPlatformDrawer drawer, Rect rect, string title)
        //{
        //    var tBar = DoToolbar(label, EditorPrefs.GetBool(label, true), add, leftButton, paste);
        //    if (tBar)
        //    {
        //        EditorPrefs.SetBool(label, !EditorPrefs.GetBool(label));
        //    }
        //    return EditorPrefs.GetBool(label);
        //}
        //public static void DoTriggerButton()
        //{

        //}

        public static void BeginImmediate<TControl>(this IImmediateControlsDrawer<TControl> drawer)
        {
            if (drawer.Controls == null)
            {
                drawer.Controls = new Dictionary<string, TControl>();
            }
            if (drawer.ControlsLeftOver == null)
            {
                drawer.ControlsLeftOver = new List<string>();
            }
            drawer.ControlsLeftOver.AddRange(drawer.Controls.Select(p => p.Key));
        }
        public static void EndImmediate<TControl>(this IImmediateControlsDrawer<TControl> drawer)
        {
            for (int index = 0; index < drawer.ControlsLeftOver.Count; index++)
            {
                var item = drawer.ControlsLeftOver[index];
                drawer.RemoveControlFromCanvas(drawer.Controls[item]);
                drawer.Controls.Remove(item);
            }
            drawer.ControlsLeftOver.Clear();

        }
        public static TControlType EnsureControl<TControl, TControlType>(this IImmediateControlsDrawer<TControl> drawer, string id, Rect rect, Func<TControlType> init = null) where TControlType : TControl
        {

            TControl control;
            if (!drawer.Controls.TryGetValue(id, out control))
            {
                if (init != null)
                {
                    control = init();
                    drawer.AddControlToCanvas(control);
                    drawer.Controls.Add(id, control);
                }

            }
            drawer.ControlsLeftOver.Remove(id);
            drawer.SetControlPosition(control, rect.x, rect.y, rect.width, rect.height);
            return (TControlType)control;
        }
    }
}