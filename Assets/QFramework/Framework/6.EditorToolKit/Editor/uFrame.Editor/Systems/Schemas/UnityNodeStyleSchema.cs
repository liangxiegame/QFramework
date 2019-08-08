using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using QF.GraphDesigner.Schemas.impl;
using UnityEngine;

namespace QF.GraphDesigner.Unity.Schemas
{
    public class UnityNodeStyleSchema : NodeStyleSchema
    {
        public override INodeStyleSchema RecomputeStyles()
        {
            CollapsedHeaderStyleObject = Header(true);
            ExpandedHeaderStyleObject = Header(false);

            (CollapsedHeaderStyleObject as GUIStyle).padding = HeaderPadding;

            TitleStyleObject = new GUIStyle()
            {
                fontSize = TitleFontSize,
                fontStyle = TitleFontStyle
            }.WithAllStates(TitleColor);

            if (!string.IsNullOrEmpty(TitleFont))
            {
                (TitleStyleObject as GUIStyle).font = InvertGraphEditor.StyleProvider.GetFont(TitleFont) as Font;
            }

            SubTitleStyleObject = new GUIStyle()
            {
                fontSize = SubTitleFontSize,
                fontStyle = SubTitleFontStyle
            }.WithAllStates(SubTitleColor);

            if (!string.IsNullOrEmpty(SubTitleFont))
            {
                (SubTitleStyleObject as GUIStyle).font = InvertGraphEditor.StyleProvider.GetFont(SubTitleFont) as Font;
            }

            return this;
        }

        protected override INodeStyleSchema GetInstance()
        {
            return new UnityNodeStyleSchema();
        }

        protected override object ConstructHeaderImage(bool expanded, Color color = default(Color), string iconName = null)
        {
            Texture2D texture =
                ElementDesignerStyles.GetSkinTexture(string.IsNullOrEmpty(iconName) ? "Header3" : iconName);

            if (expanded)
            {
                texture = texture.CutTextureBottomBorder(35);
            }

            if (color != default(Color))
            {
                texture = texture.Tint(color);
            }

            return texture;
        }

        protected override object ConstructIcon(string iconName, Color color = new Color())
        {
            var texture = ElementDesignerStyles.GetSkinTexture(iconName);
            if (texture == null) texture = ElementDesignerStyles.GetSkinTexture("CommandIcon");
            //if (color != default(Color)) texture = texture.Tint(color);
            return texture;
        }

        private GUIStyle Header(bool collapsed)
        {
            return new GUIStyle
            {
                //normal = { background = texture },
                //padding = new RectOffset(-9, 1, 19, 9),
                stretchHeight = true,
                stretchWidth = true,
                border = new RectOffset(16, 16, 13, 0)
                // fixedHeight = 36,
                // border = new RectOffset(44, 50, 20, 34),
                //padding = new RectOffset(7, 7, 7, 7)
            };
        }





    }
}
