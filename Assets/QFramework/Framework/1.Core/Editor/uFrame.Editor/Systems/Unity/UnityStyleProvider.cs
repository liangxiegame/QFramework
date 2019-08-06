using System;
using System.Collections.Generic;
using Invert.Common;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class UnityStyleProvider : IStyleProvider
    {
        private static Dictionary<string, Texture2D> _textures;

        public GUIStyle Style(InvertStyles style)
        {
            return (GUIStyle)GetStyle(style);
        }

        protected static Dictionary<string, Texture2D> Textures
        {
            get { return _textures ?? (_textures = new Dictionary<string, Texture2D>()); }
            set { _textures = value; }
        }

        static UnityStyleProvider()
        {
            


        }

        public Texture2D Image(string name)
        {
            return (Texture2D)GetImage(name);
        }
        public object GetImage(string name)
        {
            if (Textures.ContainsKey(name))
                return Textures[name];
            return ElementDesignerStyles.GetSkinTexture(name);
        }

        public object GetStyle(InvertStyles name)
        {
            if (Textures.Count < 1)
            {
                Textures.Add("DiagramArrowRight", ElementDesignerStyles.ArrowRightTexture);
                Textures.Add("DiagramArrowLeft", ElementDesignerStyles.ArrowLeftTexture);
                Textures.Add("DiagramArrowUp", ElementDesignerStyles.ArrowUpTexture);
                Textures.Add("DiagramArrowDown", ElementDesignerStyles.ArrowDownTexture);
                Textures.Add("DiagramArrowRightEmpty", ElementDesignerStyles.ArrowRightEmptyTexture);
                Textures.Add("DiagramArrowLeftEmpty", ElementDesignerStyles.ArrowLeftEmptyTexture);
                Textures.Add("DiagramCircleConnector", ElementDesignerStyles.DiagramCircleConnector);
            }
            switch (name)
            {
                case InvertStyles.DefaultLabel:
                    return EditorStyles.label;
                case InvertStyles.DefaultLabelLarge:
                    return EditorStyles.largeLabel;
                case InvertStyles.Tag1:
                    return ElementDesignerStyles.Tag1;         
                case InvertStyles.ListItemTitleStyle:
                    return ElementDesignerStyles.ListItemTitleStyle;
                case InvertStyles.BreadcrumbBoxStyle:
                    return ElementDesignerStyles.BreadcrumbBoxStyle;            
                case InvertStyles.BreadcrumbBoxActiveStyle:
                    return ElementDesignerStyles.BreadcrumbBoxActiveStyle;
                case InvertStyles.BreadcrumbTitleStyle:
                    return ElementDesignerStyles.BreadcrumbTitleStyle;                
                case InvertStyles.WizardBox:
                    return ElementDesignerStyles.WizardBoxStyle;      
                case InvertStyles.WizardSubBox:
                    return ElementDesignerStyles.WizardSubBoxStyle;          
                case InvertStyles.WizardActionButton:
                    return ElementDesignerStyles.WizardActionButtonStyle;
                case InvertStyles.WizardActionTitle:
                    return ElementDesignerStyles.WizardActionTitleStyle;     
                case InvertStyles.WizardSubBoxTitle:
                    return ElementDesignerStyles.WizardSubBoxTitleStyle;
                case InvertStyles.TooltipBox:
                    return ElementDesignerStyles.TooltipBoxStyle;
                case InvertStyles.WizardListItemBox:
                    return ElementDesignerStyles.WizardListItemBoxStyle;
                case InvertStyles.TabBox:
                    return ElementDesignerStyles.TabBoxStyle;       
                case InvertStyles.SearchBarText:
                    return ElementDesignerStyles.SearchBarTextStyle;
                case InvertStyles.TabCloseButton:
                    return ElementDesignerStyles.TabCloseButtonStyle;
                case InvertStyles.TabBoxActive:
                    return ElementDesignerStyles.TabBoxActiveStyle;
                case InvertStyles.TabTitle:
                    return ElementDesignerStyles.TabTitleStyle;
                case InvertStyles.NodeBackground:
                    return ElementDesignerStyles.NodeBackground;           
                case InvertStyles.NodeBackgroundBorderless:
                    return ElementDesignerStyles.NodeBackgroundBorderless;
                case InvertStyles.NodeExpand:
                    return ElementDesignerStyles.NodeExpand;
                case InvertStyles.NodeCollapse:
                    return ElementDesignerStyles.NodeCollapse;
                case InvertStyles.BoxHighlighter1:
                    return ElementDesignerStyles.BoxHighlighter1;
                case InvertStyles.BoxHighlighter2:
                    return ElementDesignerStyles.BoxHighlighter2;
                case InvertStyles.BoxHighlighter3:
                    return ElementDesignerStyles.BoxHighlighter3;
                case InvertStyles.BoxHighlighter4:
                    return ElementDesignerStyles.BoxHighlighter4;
                case InvertStyles.BoxHighlighter5:
                    return ElementDesignerStyles.BoxHighlighter5;
                case InvertStyles.BoxHighlighter6:
                    return ElementDesignerStyles.BoxHighlighter6;
                case InvertStyles.NodeHeader1:
                    return ElementDesignerStyles.NodeHeader1;
                case InvertStyles.NodeHeader2:
                    return ElementDesignerStyles.NodeHeader2;
                case InvertStyles.NodeHeader3:
                    return ElementDesignerStyles.NodeHeader3;
                case InvertStyles.NodeHeader4:
                    return ElementDesignerStyles.NodeHeader4;
                case InvertStyles.NodeHeader5:
                    return ElementDesignerStyles.NodeHeader5;
                case InvertStyles.NodeHeader6:
                    return ElementDesignerStyles.NodeHeader6;
                case InvertStyles.NodeHeader7:
                    return ElementDesignerStyles.NodeHeader7;
                case InvertStyles.NodeHeader8:
                    return ElementDesignerStyles.NodeHeader8;
                case InvertStyles.NodeHeader9:
                    return ElementDesignerStyles.NodeHeader9;
                case InvertStyles.NodeHeader10:
                    return ElementDesignerStyles.NodeHeader10;
                case InvertStyles.NodeHeader11:
                    return ElementDesignerStyles.NodeHeader11;
                case InvertStyles.NodeHeader12:
                    return ElementDesignerStyles.NodeHeader12;
                case InvertStyles.NodeHeader13:
                    return ElementDesignerStyles.NodeHeader13;
                case InvertStyles.Item1:
                    return ElementDesignerStyles.Item1;
                case InvertStyles.Item2:
                    return ElementDesignerStyles.Item2;
                case InvertStyles.Item3:
                    return ElementDesignerStyles.Item3;
                case InvertStyles.Item4:
                    return ElementDesignerStyles.Item4;
                case InvertStyles.Item5:
                    return ElementDesignerStyles.Item5;
                case InvertStyles.Item6:
                    return ElementDesignerStyles.Item6;
                case InvertStyles.SelectedItemStyle:
                    return ElementDesignerStyles.SelectedItemStyle;
                case InvertStyles.HeaderStyle:
                    return ElementDesignerStyles.HeaderStyle;
                case InvertStyles.ViewModelHeaderStyle:
                    return ElementDesignerStyles.ViewModelHeaderStyle;
                case InvertStyles.AddButtonStyle:
                    return ElementDesignerStyles.AddButtonStyle;
                case InvertStyles.ItemTextEditingStyle:
                    return ElementDesignerStyles.ItemTextEditingStyle;
                case InvertStyles.GraphTitleLabel:
                    return ElementDesignerStyles.GraphTitleLabel;
            }


            return ElementDesignerStyles.ClearItemStyle;
        }


        private static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();

        public object GetFont(string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) return null;
            if (!Fonts.ContainsKey(fontName))
            {
                Fonts.Add(fontName, Resources.Load<Font>("fonts/" + fontName));
            }
            return Fonts[fontName];
        }

        internal struct IconTintItem
        {
            public string Name { get; set; }
            public Color Tint { get; set; }
        }

        private Dictionary<IconTintItem, object> IconCache = new Dictionary<IconTintItem, object>();

        public object GetIcon(string name, Color tint)
        {
            return null;
        }

        public INodeStyleSchema GetNodeStyleSchema(NodeStyle name)
        {
            switch (name)
            {
                case NodeStyle.Minimalistic:
                    return ElementDesignerStyles.NodeStyleSchemaMinimalistic;
                    break;
                case NodeStyle.Bold:
                    return ElementDesignerStyles.NodeStyleSchemaBold;
                    break;
                case NodeStyle.Normal:
                    return ElementDesignerStyles.NodeStyleSchemaNormal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("name", name, null);
            }
        }

        public IConnectorStyleSchema GetConnectorStyleSchema(ConnectorStyle name)
        {
            switch (name)
            {
                case ConnectorStyle.Triangle:
                    return ElementDesignerStyles.ConnectorStyleSchemaTriangle;
                    break;
                case ConnectorStyle.Circle:
                    return ElementDesignerStyles.ConnectorStyleSchemaCircle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("name", name, null);
            }
        }

        public IBreadcrumbsStyleSchema GetBreadcrumbStyleSchema(BreadcrumbsStyle name)
        {
            switch (name)
            {
                case BreadcrumbsStyle.Default:
                    return ElementDesignerStyles.DefaultBreadcrumbsStyleSchema;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("name", name, null);
            }
        }

        public object GetNodeHeaderStyle(NodeColor color)
        {
            switch (color)
            {
                case NodeColor.DarkGray:
                    return ElementDesignerStyles.NodeHeader1;
                case NodeColor.Blue:
                    return ElementDesignerStyles.NodeHeader2;
                case NodeColor.Gray:
                    return ElementDesignerStyles.NodeHeader3;
                case NodeColor.LightGray:
                    return ElementDesignerStyles.NodeHeader4;
                case NodeColor.Black:
                    return ElementDesignerStyles.NodeHeader5;
                case NodeColor.DarkDarkGray:
                    return ElementDesignerStyles.NodeHeader6;
                case NodeColor.Orange:
                    return ElementDesignerStyles.NodeHeader7;
                case NodeColor.Red:
                    return ElementDesignerStyles.NodeHeader8;
                case NodeColor.YellowGreen:
                    return ElementDesignerStyles.NodeHeader9;
                case NodeColor.Green:
                    return ElementDesignerStyles.NodeHeader10;
                case NodeColor.Purple:
                    return ElementDesignerStyles.NodeHeader11;
                case NodeColor.Pink:
                    return ElementDesignerStyles.NodeHeader12;
                case NodeColor.Yellow:
                    return ElementDesignerStyles.NodeHeader13;

            }
            return ElementDesignerStyles.NodeHeader1;
        }
    }
}