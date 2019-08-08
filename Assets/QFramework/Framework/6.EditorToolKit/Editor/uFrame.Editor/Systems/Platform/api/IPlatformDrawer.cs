using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IPlatformDrawer
    {
        void BeginRender(object sender, MouseEvent mouseEvent);

        //void DrawConnector(float scale, ConnectorViewModel viewModel);
        Vector2 CalculateTextSize(string text, object styleObject);
        
        float CalculateTextHeight(string text, object styleObject, float width);
        
        Vector2 CalculateImageSize(string imageName);

        void DoButton(Rect scale, string label, object style, System.Action action, System.Action rightClick = null);
        
        void DoButton(Rect scale, string label, object style, Action<Vector2> action, Action<Vector2> rightClick = null);

        void DrawBezier(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, float width);

        void DrawColumns(Rect rect, float[] columnWidths, params Action<Rect>[] columns);

        void DrawColumns(Rect rect, params Action<Rect>[] columns);

        void DrawImage(Rect bounds, string texture, bool b);
        
        void DrawImage(Rect bounds, object texture, bool b);

        void DrawLabel(Rect rect, string label, object style, DrawingAlignment alignment = DrawingAlignment.MiddleLeft);
        
        void DrawPolyLine(Vector2[] lines, Color color);

        void DrawLine(Vector3[] lines, Color color);
        
        void SetTooltipForRect(Rect rect, string tooltip);
        
        string GetTooltip();

        void ClearTooltip();

        void DrawPropertyField(PropertyFieldViewModel propertyFieldDrawer, float scale);
        void DrawPropertyField(Rect r, PropertyFieldViewModel propertyFieldDrawer, float scale);

        void DrawStretchBox(Rect scale, object nodeBackground, float offset);

        void DrawStretchBox(Rect scale, object nodeBackground, Rect offset);

        void DrawTextbox(string id, Rect bounds, string value, object itemTextEditingStyle, Action<string, bool> valueChangedAction);

        void DrawWarning(Rect rect, string key);

        void DrawNodeHeader(Rect boxRect, object backgroundStyle, bool isCollapsed, float scale, object image);

        void DoToolbar(Rect toolbarTopRect, DesignerWindow designerWindow, ToolbarPosition position);

        void DoTabs(Rect tabsRect, DesignerWindow designerWindow);

        void DisableInput();

        void EnableInput();

        void EndRender();
        //Rect GetRect(object style);
        void DrawRect(Rect boundsRect, Color color);
        
    }
}