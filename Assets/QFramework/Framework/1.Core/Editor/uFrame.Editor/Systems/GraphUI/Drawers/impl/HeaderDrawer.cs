using System;
using Invert.Common;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class HeaderDrawer : Drawer
    {
        private object _textStyle;
        private object _backgroundStyle;
        private INodeStyleSchema _styleSchema;
        private object _iconImage;
        private object _headerImage;

        public string IconTooltip { get; set; }

        public virtual INodeStyleSchema StyleSchema
        {
            get { return _styleSchema; }
            set { _styleSchema = value; }
        }

        public virtual object IconImage
        {
            get { return _iconImage != null && !_iconImage.Equals(null) ? _iconImage : (_iconImage = StyleSchema.GetIconImage(NodeViewModel.IconName, NodeViewModel.IconTint)); }
            set { _iconImage = value; }
        }

        public virtual object HeaderImage
        {
            get { return _headerImage != null && !_headerImage.Equals(null) ? _headerImage : (_headerImage = StyleSchema.GetHeaderImage(!NodeViewModel.IsCollapsed, NodeViewModel.HeaderColor, NodeViewModel.HeaderBaseImage)); }
            set { _headerImage = value; }
        }

        public object BackgroundStyle
        {
            get { return _backgroundStyle ?? (_backgroundStyle = CachedStyles.ItemStyle); }
            set { _backgroundStyle = value; }
        }

        public object TextStyle
        {
            get { return _textStyle ?? (_textStyle = CachedStyles.ViewModelHeaderStyle); }
            set { _textStyle = value; }
        }

        public DiagramNodeViewModel NodeViewModel
        {
            get { return ViewModelObject as DiagramNodeViewModel; }
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);

            TextSize = platform.CalculateTextSize(NodeViewModel.Label, StyleSchema.TitleStyleObject); //.CalcSize(new GUIContent(NodeViewModel.Label)));
            
            Vector2 subTitleSize = Vector2.zero; 
            
            if (StyleSchema.ShowSubtitle)
            {
                subTitleSize = platform.CalculateTextSize(NodeViewModel.SubTitle, StyleSchema.SubTitleStyleObject);
            }

            var padding = StyleSchema.HeaderPadding;

            var width = (TextSize.x > subTitleSize.x ? TextSize.x : subTitleSize.x)+(StyleSchema.ShowIcon ? 8 : 0); //Add icon padding

            var height = TextSize.y + subTitleSize.y + padding.top + padding.bottom;

            if (StyleSchema.ShowIcon && NodeViewModel.IconName != null)
            {
                var iconBounds = IconBounds ?? (IconBounds = new Vector2(16,16));
                width += iconBounds.Value.x;
//                if (height - Padding.y*2 < iconBounds.y)
//                {
//                    height = iconBounds.y + Padding.y*2;
//                }
            }

            if (NodeViewModel.IsCollapsed)
            {
                this.Bounds = new Rect(position.x, position.y, width + 12, height); //height > 35 ? height : 35);
            }
            else
            {
                this.Bounds = new Rect(position.x, position.y, width + 12, height); //height > 35 ? height : 35);
            }
            var cb = new Rect(this.Bounds);
            cb.width += 4;
            ViewModelObject.ConnectorBounds = cb;
        }

        public Vector2? IconBounds { get; set; }

        public Vector2 TextSize { get; set; }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);

            var headerPadding = StyleSchema.HeaderPadding;
//            var headerBounds = new Rect(
//                Bounds.x - headerPadding.left, 
//                Bounds.y, 
//                Bounds.width + headerPadding.left * 2 + 1,
//                Bounds.height + (NodeViewModel.IsCollapsed ? 0 : -20) + headerPadding.bottom);
            var headerBounds = new Rect(
                //Bounds.x-headerPadding.left-1, 
                Bounds.x-headerPadding.left+1, 
                Bounds.y+1, 
                Bounds.width + headerPadding.left + headerPadding.right + headerPadding.left -6,
                Bounds.height+0 + (NodeViewModel.IsCollapsed ? 9 : -2));

            var image = HeaderImage;

                platform.DrawNodeHeader(
                    headerBounds, 
                    NodeViewModel.IsCollapsed ? StyleSchema.CollapsedHeaderStyleObject : StyleSchema.ExpandedHeaderStyleObject, 
                    NodeViewModel.IsCollapsed, 
                    scale,image);
            

            // The bounds for the main text

//            var textBounds = new Rect(Bounds.x, Bounds.y + ((Bounds.height / 2f) - (TextSize.y / 2f)), Bounds.width,
//                Bounds.height);
            var padding = headerPadding;
            var titleBounds = new Rect(
                Bounds.x + padding.left, 
                Bounds.y + padding.top + (StyleSchema.ShowSubtitle ? 1 : 0 ) , 
                Bounds.width-padding.right-padding.left-(StyleSchema.ShowIcon ? 16 : 0), //Subtract icon size if shown
                Bounds.height-padding.top-padding.bottom);

            var titleSize = platform.CalculateTextSize(NodeViewModel.Label, StyleSchema.TitleStyleObject);

            var subtitleBound = new Rect(Bounds.x + padding.left+0, Bounds.y + padding.top + titleSize.y + 0, Bounds.width-padding.right, Bounds.height-padding.top);




            if (NodeViewModel.IsEditing && NodeViewModel.IsEditable)
            {

                //UnityEngine.GUI.SetNextControlName("EditingField");
                //DiagramDrawer.IsEditingField = true;
                //UnityEditor.EditorGUI.BeginChangeCheck();

                //var newText = GUI.TextField(textBounds.Scale(scale), NodeViewModel.Name,
                //    ElementDesignerStyles.ViewModelHeaderEditingStyle);

                //if (UnityEditor.EditorGUI.EndChangeCheck())
                //{
                //    NodeViewModel.Rename(newText);
                //    Dirty = true;
                //}

                //textBounds.y += TextSize.y / 2f;
                platform.DrawTextbox(NodeViewModel.GraphItemObject.Identifier, titleBounds.Scale(scale), NodeViewModel.Name, CachedStyles.ViewModelHeaderStyle, (v, finished) =>
                {
                    NodeViewModel.Rename(v);
                    ParentDrawer.Refresh(platform);
                    if (finished)
                    {
                        NodeViewModel.EndEditing();
                    }
                });

            }
            else
            {
                //var titleStyle = new GUIStyle(TextStyle);
                //titleStyle.normal.textColor = BackgroundStyle.normal.textColor;
                //titleStyle.alignment = TextAnchor.MiddleCenter;
                //titleStyle.fontSize = Mathf.RoundToInt(12*scale);
                platform.DrawLabel(titleBounds.Scale(scale), NodeViewModel.Label ?? string.Empty, StyleSchema.TitleStyleObject, StyleSchema.ShowSubtitle ? DrawingAlignment.TopLeft : DrawingAlignment.MiddleLeft);

                if (StyleSchema.ShowSubtitle && !string.IsNullOrEmpty(NodeViewModel.SubTitle))
                {
                    platform.DrawLabel(subtitleBound.Scale(scale), NodeViewModel.SubTitle ?? string.Empty,
                        StyleSchema.SubTitleStyleObject, StyleSchema.ShowSubtitle ? DrawingAlignment.TopLeft : DrawingAlignment.MiddleLeft);
                }

                if (StyleSchema.ShowIcon && !string.IsNullOrEmpty(NodeViewModel.IconName))
                {
                    var iconsize = IconBounds ?? (IconBounds = new Vector2(16,16));
                    var size = 16;
                    var imageBounds = new Rect(Bounds.xMax - padding.right - size, Bounds.y + ((Bounds.height / 2f) - (size / 2f)), 16, 16);


                    //var imageBounds = new Rect().WithSize(16, 16).InnerAlignWithUpperRight(Bounds).AlignHorisonallyByCenter(headerBounds).Translate(-headerPadding.right,0);
                    var cCache = GUI.color;
                    GUI.color = new Color(cCache.r, cCache.g, cCache.b, 0.7f);
                    platform.DrawImage(imageBounds.Scale(scale), IconImage, true);
                    GUI.color = cCache;

                    if (!string.IsNullOrEmpty(IconTooltip))
                    {
                        platform.SetTooltipForRect(imageBounds.Scale(scale), IconTooltip);
                    }


                }

                //GUI.Label(textBounds.Scale(scale), NodeViewModel.Label ?? string.Empty, titleStyle);
                //if (NodeViewModel.IsCollapsed)
                //{
                //    textBounds.y += TextSize.y/2f;
                //    //titleStyle.fontSize = Mathf.RoundToInt(10*scale);
                //    //titleStyle.fontStyle = FontStyle.Italic;

                //    GUI.Label(textBounds.Scale(scale), NodeViewModel.SubTitle, titleStyle);
                //}
            }
        }
    }
}