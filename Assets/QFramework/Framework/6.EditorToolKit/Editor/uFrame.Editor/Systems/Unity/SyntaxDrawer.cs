using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class SyntaxDrawer : Drawer<SyntaxViewModel>
    {
        private GUIStyle guiStyle;

        public SyntaxDrawer(SyntaxViewModel viewModelObject) : base(viewModelObject)
        {
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            guiStyle = new GUIStyle(EditorStyles.label);
            guiStyle.padding = new RectOffset(0,0,0,0);
            guiStyle.margin = new RectOffset(0,0,0,0);
            var height = 0f;
            var y = position.y;
            var maxLineWidth = 0f;
            var maxWidth = 0f;
            foreach (var line in ViewModel.Lines)
            {
                var x = position.x;
                var maxHeight = 0f;
                foreach (var token in line.Tokens)
                {
                    if (token.Bold)
                    {
                        guiStyle.fontStyle = FontStyle.Bold;
                    }
                    else
                    {
                        guiStyle.fontStyle = FontStyle.Normal;
                    }
                    if (token.Text.All(char.IsWhiteSpace))
                    {
                        token.TextSize = guiStyle.CalcSize(new GUIContent("f"));
                    }
                    else
                    {
                        token.TextSize = guiStyle.CalcSize(new GUIContent(token.Text));
                    }
                    
                    token.Bounds = new Rect(x, y, token.TextSize.x,  token.TextSize.y);
                    x += token.TextSize.x;
                    maxWidth = Math.Max(token.TextSize.x, maxWidth);
                    maxHeight = Math.Max(token.TextSize.y, maxHeight);
                }
                line.Bounds = new Rect(x, y, line.Tokens.Sum(p => p.TextSize.x), line.Tokens.Sum(p => p.TextSize.y));

                y += maxHeight;
                height += maxHeight;
                maxLineWidth = Math.Max(maxLineWidth, line.Bounds.width);
            }
            var newBounds = new Rect(Bounds);
            newBounds.x = position.x;
            newBounds.y = position.y;
            newBounds.height = height;
            newBounds.width = maxLineWidth;
            Bounds = newBounds;
            
        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
            GUI.Box(Bounds.Scale(scale),string.Empty,EditorStyles.textArea);
            foreach (var line in ViewModel.Lines)
            {

                foreach (var token in line.Tokens)
                {
                    guiStyle.normal.textColor = token.Color;
                    if (token.Bold)
                    {
                        guiStyle.fontStyle = FontStyle.Bold;
                    }
                    else
                    {
                        guiStyle.fontStyle = FontStyle.Normal;
                    }
                    GUI.Label(token.Bounds,token.Text,guiStyle);
                }
            }
        }
    }
}