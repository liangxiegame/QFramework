using UnityEngine;

namespace QFramework.GraphDesigner
{
    public static class CachedStyles
    {
        private static object _boxHighlighter5;
        private static object _boxHighlighter1;
        private static object _nodeExpand;
        private static object _item1;
        private static object _defaultLabel;
        private static object _nodeBackground;
        private static object _nodeCollapse;
        private static object _boxHighlighter2;
        private static object _boxHighlighter3;
        private static object _boxHighlighter4;
        private static object _boxHighlighter6;
        private static object _nodeHeader1;
        private static object _nodeHeader2;
        private static object _nodeHeader3;
        private static object _nodeHeader4;
        private static object _nodeHeader5;
        private static object _nodeHeader6;
        private static object _nodeHeader7;
        private static object _nodeHeader8;
        private static object _nodeHeader9;
        private static object _nodeHeader10;
        private static object _nodeHeader11;
        private static object _nodeHeader12;
        private static object _nodeHeader13;
        private static object _itemTextEditingStyle;
        private static object _addButtonStyle;
        private static object _headerStyle;
        private static object _defaultLabelLarge;
        private static object _clearItemStyle;
        private static object _viewModelHeaderStyle;
        private static object _itemStyle;
        private static object _selectedItemStyle;
        private static object _item2;
        private static object _item3;
        private static object _item4;
        private static object _item5;
        private static object _item6;
        private static object _tag1;
        private static object _toolbar;
        private static object _toolbarButton;
        private static object _toolbarButtonDrop;
        private static object _graphTitleLabel;
        private static IConnectorStyleSchema _connectorStyleSchemaTriangle;
        private static IConnectorStyleSchema _connectorStyleSchemaCircle;
        private static INodeStyleSchema _nodeStyleSchemaNormal;
        private static INodeStyleSchema _nodeStyleSchemaMinimalistic;
        private static INodeStyleSchema _nodeStyleSchemaBold;
        private static object _nodeBackgroundBorderless;
        private static object _breadcrumbTitleStyle;
        private static object _breadcrumbBoxStyle;
        private static object _breadcrumbBoxActiveStyle;
        private static IBreadcrumbsStyleSchema _defaultBreadcrumbsStyleSchema;
        private static object _tabBoxStyle;
        private static object _tabBoxActiveStyle;
        private static object _tabTitleStyle;
        private static object _tabCloseButton;
        private static object _wizardBoxStyle;
        private static object _wizardSubBoxStyle;
        private static object _wizardActionButtonStyle;
        private static object _wizardActionTitleStyle;
        private static object _wizardSubBoxTitleStyle;
        private static object _tooltipBoxStyle;
        private static object _wizardListItemBoxStyle;
        private static object _searchBarText;
        private static object _listItemTitleStyle;

        public static object Item1
        {
            get { return _item1 ?? (_item1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item1)); }
        }
        public static object Item2
        {
            get { return _item2 ?? (_item2 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item2)); }
        }
        public static object Item3
        {
            get { return _item3 ?? (_item3 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item3)); }
        }
        public static object Item4
        {
            get { return _item4 ?? (_item4 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item4)); }
        }
        public static object Item5
        {
            get { return _item5 ?? (_item5 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item5)); }
        }
        public static object Item6
        {
            get { return _item6 ?? (_item6 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item6)); }
        }
        public static object DefaultLabel
        {
            get { return _defaultLabel ?? (_defaultLabel = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.DefaultLabel)); }
        }
        public static object NodeBackground
        {
            get { return _nodeBackground ?? (_nodeBackground = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeBackground)); }
        }       
        public static object NodeBackgroundBorderless
        {
            get { return _nodeBackgroundBorderless ?? (_nodeBackgroundBorderless = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeBackgroundBorderless)); }
        }
        public static object NodeExpand
        {
            get { return _nodeExpand ?? (_nodeExpand = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeExpand)); }
        }
        public static object NodeCollapse
        {
            get { return _nodeCollapse ?? (_nodeCollapse = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeCollapse)); }
        }
        public static object BoxHighlighter1
        {
            get { return _boxHighlighter1 ?? (_boxHighlighter1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter1)); }
        }
        public static object BoxHighlighter2
        {
            get { return _boxHighlighter2 ?? (_boxHighlighter2 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter2)); }
        }
        public static object BoxHighlighter3
        {
            get { return _boxHighlighter3 ?? (_boxHighlighter3 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter3)); }
        }
        public static object BoxHighlighter4
        {
            get { return _boxHighlighter4 ?? (_boxHighlighter4 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter4)); }
        }
        public static object BoxHighlighter5
        {
            get { return _boxHighlighter5 ?? (_boxHighlighter5 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter5)); }
        }
        public static object BoxHighlighter6
        {
            get { return _boxHighlighter6 ?? (_boxHighlighter6 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter6)); }
        }
        public static object NodeHeader1
        {
            get { return _nodeHeader1 ?? (_nodeHeader1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader1)); }
        }
        public static object NodeHeader2
        {
            get { return _nodeHeader2 ?? (_nodeHeader2 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader2)); }
        }
        public static object NodeHeader3
        {
            get { return _nodeHeader3 ?? (_nodeHeader3 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader3)); }
        }
        public static object NodeHeader4
        {
            get { return _nodeHeader4 ?? (_nodeHeader4 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader4)); }
        }
        public static object NodeHeader5
        {
            get { return _nodeHeader5 ?? (_nodeHeader5 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader5)); }
        }
        public static object NodeHeader6
        {
            get { return _nodeHeader6 ?? (_nodeHeader6 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader6)); }
        }
        public static object NodeHeader7
        {
            get { return _nodeHeader7 ?? (_nodeHeader7 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader7)); }
        }
        public static object NodeHeader8
        {
            get { return _nodeHeader8 ?? (_nodeHeader8 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader8)); }
        }
        public static object NodeHeader9
        {
            get { return _nodeHeader9 ?? (_nodeHeader9 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader9)); }
        }
        public static object NodeHeader10
        {
            get { return _nodeHeader10 ?? (_nodeHeader10 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader10)); }
        }
        public static object NodeHeader11
        {
            get { return _nodeHeader11 ?? (_nodeHeader11 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader11)); }
        }
        public static object NodeHeader12
        {
            get { return _nodeHeader12 ?? (_nodeHeader12 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader12)); }
        }
        public static object NodeHeader13
        {
            get { return _nodeHeader13 ?? (_nodeHeader13 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader13)); }
        }
   
        public static object SearchBarText
        {
            get { return _searchBarText ?? (_searchBarText = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.SearchBarText)); }
        }


        public static object ItemTextEditingStyle
        {
            get { return _itemTextEditingStyle ?? (_itemTextEditingStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ItemTextEditingStyle)); }
        }
        public static object AddButtonStyle
        {
            get { return _addButtonStyle ?? (_addButtonStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.AddButtonStyle)); }
        }
        public static object HeaderStyle
        {
            get { return _headerStyle ?? (_headerStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.HeaderStyle)); }
        }
        public static object DefaultLabelLarge
        {
            get { return _defaultLabelLarge ?? (_defaultLabelLarge = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.DefaultLabelLarge)); }
        }
        public static object GraphTitleLabel
        {
            get { return _graphTitleLabel ?? (_graphTitleLabel = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.GraphTitleLabel)); }
        }
        public static object ClearItemStyle
        {
            get { return _clearItemStyle ?? (_clearItemStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ClearItemStyle)); }
        }
        public static object ViewModelHeaderStyle
        {
            get { return _viewModelHeaderStyle ?? (_viewModelHeaderStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ViewModelHeaderStyle)); }
        }
        public static object ItemStyle
        {
            get { return _itemStyle ?? (_itemStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ItemStyle)); }
        }
        public static object SelectedItemStyle
        {
            get { return _selectedItemStyle ?? (_selectedItemStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.SelectedItemStyle)); }
        }
        public static object Tag1
        {
            get { return _tag1 ?? (_tag1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Tag1)); }
        }
        public static object Toolbar
        {
            get { return _toolbar ?? (_toolbar = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Toolbar)); }
        }
        public static object ToolbarButton
        {
            get { return _toolbarButton ?? (_toolbarButton = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ToolbarButton)); }
        }
        public static object ToolbarButtonDrop
        {
            get { return _toolbarButtonDrop ?? (_toolbarButtonDrop = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ToolbarButtonDown)); }
        }
        public static object HeaderTitleStyle
        {
            get { return _toolbarButtonDrop ?? (_toolbarButtonDrop = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.HeaderTitleStyle)); }
        }
        public static object HeaderSubTitleStyle
        {
            get { return _toolbarButtonDrop ?? (_toolbarButtonDrop = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.HeaderSubTitleStyle)); }
        }
        public static object WizardBoxStyle
        {
            get { return _wizardBoxStyle ?? (_wizardBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardBox)); }
        }
                
        public static object WizardSubBoxStyle
        {
            get { return _wizardSubBoxStyle ?? (_wizardSubBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardSubBox)); }
        }        
        
        public static object WizardActionButtonStyle
        {
            get { return _wizardActionButtonStyle ?? (_wizardActionButtonStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardActionButton)); }
        }
        
        public static IConnectorStyleSchema ConnectorStyleSchemaTriangle
        {
            get { return _connectorStyleSchemaTriangle ?? (_connectorStyleSchemaTriangle = InvertGraphEditor.StyleProvider.GetConnectorStyleSchema(ConnectorStyle.Triangle)); }
        }

        public static IConnectorStyleSchema ConnectorStyleSchemaCircle
        {
            get { return _connectorStyleSchemaCircle ?? (_connectorStyleSchemaCircle = InvertGraphEditor.StyleProvider.GetConnectorStyleSchema(ConnectorStyle.Circle)); }
        }

        public static INodeStyleSchema NodeStyleSchemaNormal
        {
            get { return _nodeStyleSchemaNormal ?? (_nodeStyleSchemaNormal = InvertGraphEditor.StyleProvider.GetNodeStyleSchema(NodeStyle.Normal)); }
            set { _nodeStyleSchemaNormal = value; }
        }

        public static INodeStyleSchema NodeStyleSchemaMinimalistic
        {
            get { return _nodeStyleSchemaMinimalistic ?? (_nodeStyleSchemaMinimalistic = InvertGraphEditor.StyleProvider.GetNodeStyleSchema(NodeStyle.Minimalistic)); }
            set { _nodeStyleSchemaMinimalistic = value; }
        }

        public static INodeStyleSchema NodeStyleSchemaBold
        {
            get { return _nodeStyleSchemaBold ?? (_nodeStyleSchemaBold = InvertGraphEditor.StyleProvider.GetNodeStyleSchema(NodeStyle.Bold)); }
            set { _nodeStyleSchemaBold = value; }
        }

        public static IBreadcrumbsStyleSchema DefaultBreadcrumbsStyleSchema
        {
            get { return _defaultBreadcrumbsStyleSchema ?? (_defaultBreadcrumbsStyleSchema = InvertGraphEditor.StyleProvider.GetBreadcrumbStyleSchema(BreadcrumbsStyle.Default)); }
            set { _defaultBreadcrumbsStyleSchema = value; }
        }

        public static object BreadcrumbTitleStyle
        {
            get { return _breadcrumbTitleStyle ?? (_breadcrumbTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BreadcrumbTitleStyle)); }
            set { _breadcrumbTitleStyle = value; }
        }      
        
        public static object ListItemTitleStyle
        {
            get { return _listItemTitleStyle ?? (_listItemTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ListItemTitleStyle)); }
            set { _listItemTitleStyle = value; }
        }

        public static object TabBoxStyle
        {
            get { return _tabBoxStyle ?? (_tabBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TabBox)); }
            set { _tabBoxStyle = value; }
        }

        public static object TabBoxActiveStyle
        {
            get { return _tabBoxActiveStyle ?? (_tabBoxActiveStyle = InvertGraphEditor.StyleProvider.GetStyle((InvertStyles.TabBoxActive))); }
            set { _tabBoxActiveStyle = value; }
        }

        public static object TabTitleStyle
        {
            get { return _tabTitleStyle ?? (_tabTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TabTitle)); }
            set { _tabTitleStyle = value; }
        }

        public static object TabCloseButton
        {
            get { return _tabCloseButton ?? (_tabCloseButton = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TabCloseButton)); }
            set { _tabCloseButton = value; }
        }

        public static object BreadcrumbBoxStyle
        {
            get { return _breadcrumbBoxStyle ?? (_breadcrumbBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BreadcrumbBoxStyle) ); }
            set { _breadcrumbBoxStyle = value; }
        }      
        
        public static object BreadcrumbBoxActiveStyle
        {
            get { return _breadcrumbBoxActiveStyle ?? (_breadcrumbBoxActiveStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BreadcrumbBoxActiveStyle)); }
            set { _breadcrumbBoxActiveStyle = value; }
        }

        public static object WizardActionTitleStyle
        {
            get { return _wizardActionTitleStyle ?? (_wizardActionTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardActionTitle)); ; }
            set { _wizardActionTitleStyle = value; }
        }    
        
        public static object WizardSubBoxTitleStyle
        {
            get { return _wizardSubBoxTitleStyle ?? (_wizardSubBoxTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardSubBoxTitle)); ; }
            set { _wizardSubBoxTitleStyle = value; }
        }

        public static object TooltipBoxStyle
        {
            get { return _tooltipBoxStyle ?? (_tooltipBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TooltipBox)); }
            set { _tooltipBoxStyle = value; }
        }

        public static object WizardListItemBoxStyle
        {
            get { return _wizardListItemBoxStyle ?? (_wizardListItemBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardListItemBox)); }
            set { _wizardListItemBoxStyle = value; }
        }

        public static Color GetColor(NodeColor color)
        {
            switch (color)
            {
                case NodeColor.Gray:
                    return new Color32(104, 105, 109, 255);
                case NodeColor.DarkGray:
                    return new Color32(56, 56, 57, 255);
                case NodeColor.Blue:
                    return new Color32(115, 110, 180, 255);
                case NodeColor.LightGray:
                    return new Color32(87, 101, 108, 255);
                case NodeColor.Black:
                    return new Color32(50, 50, 50, 255);
                case NodeColor.DarkDarkGray:
                    return new Color32(51, 56, 58, 255);
                case NodeColor.Orange:
                    return new Color32(235, 126, 21, 255);
                case NodeColor.Red:
                    return new Color32(234, 20, 20, 255);
                case NodeColor.Yellow:
                    return new Color32(134, 134, 18, 255);
                case NodeColor.Green:
                    return new Color32(25, 99, 9, 255);
                case NodeColor.Purple:
                    return new Color32(58, 70, 94, 255);
                case NodeColor.Pink:
                    return new Color32(79, 44, 115, 255);
                case NodeColor.YellowGreen:
                    return new Color32(197, 191, 25, 255);
                case NodeColor.Palevioletred:
                    return new Color32(219,112,147,255);
                case NodeColor.Palevioletred4:
                    return new Color32(139,71,93,255);
                case NodeColor.Violetred2:
                    return new Color32(238,58,140,255);
                case NodeColor.Violetred4:
                    return new Color32(139,71,93,255);
                case NodeColor.Mediumorchid2:
                    return new Color32(209,95,238,255);
                case NodeColor.Mediumorchid4:
                    return new Color32(122,55,139,255);
                case NodeColor.Mediumpurple2:
                    return new Color32(159,121,238,255);
                case NodeColor.Stateblue2:
                    return new Color32(106,90,205,255);
                case NodeColor.Royalblue2:
                    return new Color32(67,110,238,255);         
                case NodeColor.Royalblue4:
                    return new Color32(39,64,139,255);
                case NodeColor.Lightsteelblue3:
                    return new Color32(162,181,205,255);
                case NodeColor.Lightskyblue2:
                    return new Color32(164,211,238,255);
                case NodeColor.Azure3:
                    return new Color32(193,205,205,255);       
                case NodeColor.Azure4:
                    return new Color32(131, 139, 139, 255);
                case NodeColor.Manganeseblue:
                    return new Color32(3, 168, 158, 255);
                case NodeColor.Aquamarine3:
                    return new Color32(102, 205, 170, 255);
                case NodeColor.Seagreen2:
                    return new Color32(78, 238, 148, 255);
                case NodeColor.Seagreen4:
                    return new Color32(46,139,87,255);
                case NodeColor.Darkolivegreen3:
                    return new Color32(162,205,90,255);
                case NodeColor.Darkolivegreen4:
                    return new Color32(110,139,61,255);
                case NodeColor.Olivedrab:
                    return new Color32(107,142,35,255);
                case NodeColor.Lightgoldenrod3:
                    return new Color32(205,190,112,255);
                case NodeColor.Lightgoldenrod4:
                    return new Color32(139,129,76,255);
                case NodeColor.Gold3:
                    return new Color32(205,173,0,255);
                case NodeColor.Tan:
                    return new Color32(210,180,140,255);
                case NodeColor.Carrot:
                    return new Color32(237,145,33,255);
                case NodeColor.Sienna2:
                    return new Color32(238,121,66,255);
                case NodeColor.Salmon2:
                    return new Color32(238,130,98,255);
                case NodeColor.Indianred2:
                    return new Color32(238,99,99,255);
                case NodeColor.Indianred4:
                    return new Color32(139,58,58,255);
                case NodeColor.SgiTeal:
                    return new Color32(56,142,142,255);
                case NodeColor.SgiDarkgrey:
                    return new Color32(85,85,85,255);
                case NodeColor.SgiLightBlue:
                    return new Color32(125,158,192,255);
                case NodeColor.SgiChartReuse:
                    return new Color32(113,198,113,255);
                case NodeColor.SgiBeet:
                    return new Color32(142,56,142,255);
                case NodeColor.SgiSlateBlue:
                    return new Color32(113,113,198,255);
                case NodeColor.SgiBrightGrey:
                    return new Color32(197,193,170,255);


                default:
                    return default(Color);
                    break;

            }
        }

    }
}