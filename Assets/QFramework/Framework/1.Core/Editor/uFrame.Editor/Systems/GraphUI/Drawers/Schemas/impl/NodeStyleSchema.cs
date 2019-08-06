using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner.Schemas.impl
{
    public abstract class NodeStyleSchema : INodeStyleSchema
    {
        public object CollapsedHeaderStyleObject { get; protected set; }
        public object ExpandedHeaderStyleObject { get; protected set; }
        public object TitleStyleObject { get; protected set; }
        public object SubTitleStyleObject { get; protected set; }
        public bool ShowTitle { get; protected set; }
        public bool ShowSubtitle { get; protected set; }
        public bool ShowIcon { get; protected set; }
        public int TitleFontSize { get; protected set; }
        public int SubTitleFontSize { get; protected set; }
        public Color TitleColor { get; protected set; }
        public Color SubTitleColor { get; protected set; }
        public string TitleFont { get; protected set; }
        public string SubTitleFont { get; protected set; }
        public string HeaderImage { get; protected set; }
        public NodeColor HeaderColor { get; protected set; }
        public FontStyle TitleFontStyle { get; protected set; }
        public FontStyle SubTitleFontStyle { get; protected set; }
        public RectOffset HeaderPadding { get; protected set; }

        public virtual INodeStyleSchema RecomputeStyles()
        {
            return this;
        }

        protected abstract INodeStyleSchema GetInstance();

        public virtual INodeStyleSchema Clone()
        {
            var newSceham = GetInstance();
            newSceham.WithHeaderImage(HeaderImage);
            newSceham.WithIcon(ShowIcon);
            newSceham.WithTitle(ShowTitle);
            newSceham.WithHeaderPadding(HeaderPadding);
            newSceham.WithSubTitle(ShowSubtitle);
            newSceham.WithTitleFont(TitleFont, TitleFontSize, TitleColor,
                TitleFontStyle);
            newSceham.WithSubTitleFont(SubTitleFont, SubTitleFontSize, SubTitleColor,
                SubTitleFontStyle);
            return newSceham;
        }

        public virtual INodeStyleSchema WithHeaderColor(NodeColor color)
        {
            HeaderColor = color;
            return this;
        }

        public virtual INodeStyleSchema WithHeaderImage(string image)
        {
            HeaderImage = image;
            return this;
        }        
        
        public virtual INodeStyleSchema WithTitleFont(string font, int? fontsize, Color? color, FontStyle? style)
        {
            TitleFontStyle = style.GetValueOrDefault(TitleFontStyle);
            TitleFontSize = fontsize.GetValueOrDefault(TitleFontSize);
            TitleColor = color.GetValueOrDefault(TitleColor);
            if (!string.IsNullOrEmpty(font))
                TitleFont = font;
            return this;
        }

        public virtual INodeStyleSchema WithSubTitleFont(string font, int? fontsize, Color? color, FontStyle? style)
        {
            SubTitleFontStyle = style.GetValueOrDefault(SubTitleFontStyle);
            SubTitleFontSize = fontsize.GetValueOrDefault(SubTitleFontSize);
            SubTitleColor = color.GetValueOrDefault(SubTitleColor);
            if (!string.IsNullOrEmpty(font))
                SubTitleFont = font;
            return this;
        }

        public INodeStyleSchema WithTitle(bool shown)
        {
            ShowTitle = shown;
            return this;
        }

        public INodeStyleSchema WithSubTitle(bool shown)
        {
            ShowSubtitle = shown;
            return this;
        }

        public INodeStyleSchema WithIcon(bool shown)
        {
            ShowIcon = shown;
            return this;
        }

        public INodeStyleSchema WithHeaderPadding(RectOffset padding)
        {
            HeaderPadding = padding;
            return this;
        }

        internal struct IconColorItem
        {
            public string Name { get; set; }
            public Color Color { get; set; }
            public bool Expanded { get; set; }
        }

        private static Dictionary<IconColorItem, object> ImagePool = new Dictionary<IconColorItem, object>();

        public object GetHeaderImage(bool expanded, Color color = default(Color), string iconName = null)
        {
            var item = new IconColorItem()
            {
                Name = iconName,
                Color = color,
                Expanded = expanded
            };


            if (!ImagePool.ContainsKey(item) || ImagePool[item].Equals(null))
            {
                ImagePool[item] = ConstructHeaderImage(expanded, color, iconName);
            }

            return ImagePool[item];
        }

        public object GetIconImage(string iconName, Color color = default(Color))
        {
            var item = new IconColorItem()
            {
                Name = iconName,
                Color = color,
            };

            if (!ImagePool.ContainsKey(item) || ImagePool[item].Equals(null))
            {
                ImagePool[item] = ConstructIcon(iconName, color);
            }

            return ImagePool[item];
        }

        protected abstract object ConstructHeaderImage(bool expanded, Color color = default(Color), string iconName = null);
        protected abstract object ConstructIcon(string iconName, Color color = default(Color));

    }
}
