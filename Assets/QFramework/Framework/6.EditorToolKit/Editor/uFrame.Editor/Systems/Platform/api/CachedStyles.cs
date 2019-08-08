using UnityEngine;

namespace QF.GraphDesigner
{
    public static class CachedStyles
    {
        private static object mBoxHighlighter5;
        private static object mBoxHighlighter1;
        private static object mNodeExpand;
        private static object mItem1;
        private static object mDefaultLabel;
        private static object mNodeBackground;
        private static object mNodeCollapse;
        private static object mBoxHighlighter2;
        private static object mBoxHighlighter3;
        private static object mBoxHighlighter4;
        private static object mBoxHighlighter6;
        private static object mNodeHeader1;
        private static object mNodeHeader2;
        private static object mNodeHeader3;
        private static object mNodeHeader4;
        private static object mNodeHeader5;
        private static object mNodeHeader6;
        private static object mNodeHeader7;
        private static object mNodeHeader8;
        private static object mNodeHeader9;
        private static object mNodeHeader10;
        private static object mNodeHeader11;
        private static object mNodeHeader12;
        private static object mNodeHeader13;
        private static object mItemTextEditingStyle;
        private static object mAddButtonStyle;
        private static object mHeaderStyle;
        private static object mDefaultLabelLarge;
        private static object mClearItemStyle;
        private static object mViewModelHeaderStyle;
        private static object mItemStyle;
        private static object mSelectedItemStyle;
        private static object mItem2;
        private static object mItem3;
        private static object mItem4;
        private static object mItem5;
        private static object mItem6;
        private static object mTag1;
        private static object mToolbar;
        private static object mToolbarButton;
        private static object mToolbarButtonDrop;
        private static object mGraphTitleLabel;
        private static IConnectorStyleSchema mConnectorStyleSchemaTriangle;
        private static IConnectorStyleSchema mConnectorStyleSchemaCircle;
        private static INodeStyleSchema mNodeStyleSchemaNormal;
        private static INodeStyleSchema mNodeStyleSchemaMinimalistic;
        private static INodeStyleSchema mNodeStyleSchemaBold;
        private static object mNodeBackgroundBorderless;
        private static object mBreadcrumbTitleStyle;
        private static object mBreadcrumbBoxStyle;
        private static object mBreadcrumbBoxActiveStyle;
        private static IBreadcrumbsStyleSchema mDefaultBreadcrumbsStyleSchema;
        private static object mTabBoxStyle;
        private static object mTabBoxActiveStyle;
        private static object mTabTitleStyle;
        private static object mTabCloseButton;
        private static object mWizardBoxStyle;
        private static object mWizardSubBoxStyle;
        private static object mWizardActionButtonStyle;
        private static object mWizardActionTitleStyle;
        private static object mWizardSubBoxTitleStyle;
        private static object mTooltipBoxStyle;
        private static object mWizardListItemBoxStyle;
        private static object mSearchBarText;
        private static object mListItemTitleStyle;

        public static object Item1
        {
            get { return mItem1 ?? (mItem1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item1)); }
        }
        public static object Item2
        {
            get { return mItem2 ?? (mItem2 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item2)); }
        }
        public static object Item3
        {
            get { return mItem3 ?? (mItem3 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item3)); }
        }
        public static object Item4
        {
            get { return mItem4 ?? (mItem4 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item4)); }
        }
        public static object Item5
        {
            get { return mItem5 ?? (mItem5 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item5)); }
        }
        public static object Item6
        {
            get { return mItem6 ?? (mItem6 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Item6)); }
        }
        public static object DefaultLabel
        {
            get { return mDefaultLabel ?? (mDefaultLabel = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.DefaultLabel)); }
        }
        public static object NodeBackground
        {
            get { return mNodeBackground ?? (mNodeBackground = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeBackground)); }
        }       
        public static object NodeBackgroundBorderless
        {
            get { return mNodeBackgroundBorderless ?? (mNodeBackgroundBorderless = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeBackgroundBorderless)); }
        }
        public static object NodeExpand
        {
            get { return mNodeExpand ?? (mNodeExpand = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeExpand)); }
        }
        public static object NodeCollapse
        {
            get { return mNodeCollapse ?? (mNodeCollapse = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeCollapse)); }
        }
        public static object BoxHighlighter1
        {
            get { return mBoxHighlighter1 ?? (mBoxHighlighter1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter1)); }
        }
        public static object BoxHighlighter2
        {
            get { return mBoxHighlighter2 ?? (mBoxHighlighter2 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter2)); }
        }
        public static object BoxHighlighter3
        {
            get { return mBoxHighlighter3 ?? (mBoxHighlighter3 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter3)); }
        }
        public static object BoxHighlighter4
        {
            get { return mBoxHighlighter4 ?? (mBoxHighlighter4 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter4)); }
        }
        public static object BoxHighlighter5
        {
            get { return mBoxHighlighter5 ?? (mBoxHighlighter5 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter5)); }
        }
        public static object BoxHighlighter6
        {
            get { return mBoxHighlighter6 ?? (mBoxHighlighter6 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BoxHighlighter6)); }
        }
        public static object NodeHeader1
        {
            get { return mNodeHeader1 ?? (mNodeHeader1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader1)); }
        }
        public static object NodeHeader2
        {
            get { return mNodeHeader2 ?? (mNodeHeader2 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader2)); }
        }
        public static object NodeHeader3
        {
            get { return mNodeHeader3 ?? (mNodeHeader3 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader3)); }
        }
        public static object NodeHeader4
        {
            get { return mNodeHeader4 ?? (mNodeHeader4 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader4)); }
        }
        public static object NodeHeader5
        {
            get { return mNodeHeader5 ?? (mNodeHeader5 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader5)); }
        }
        public static object NodeHeader6
        {
            get { return mNodeHeader6 ?? (mNodeHeader6 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader6)); }
        }
        public static object NodeHeader7
        {
            get { return mNodeHeader7 ?? (mNodeHeader7 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader7)); }
        }
        public static object NodeHeader8
        {
            get { return mNodeHeader8 ?? (mNodeHeader8 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader8)); }
        }
        public static object NodeHeader9
        {
            get { return mNodeHeader9 ?? (mNodeHeader9 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader9)); }
        }
        public static object NodeHeader10
        {
            get { return mNodeHeader10 ?? (mNodeHeader10 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader10)); }
        }
        public static object NodeHeader11
        {
            get { return mNodeHeader11 ?? (mNodeHeader11 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader11)); }
        }
        public static object NodeHeader12
        {
            get { return mNodeHeader12 ?? (mNodeHeader12 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader12)); }
        }
        public static object NodeHeader13
        {
            get { return mNodeHeader13 ?? (mNodeHeader13 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.NodeHeader13)); }
        }
   
        public static object SearchBarText
        {
            get { return mSearchBarText ?? (mSearchBarText = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.SearchBarText)); }
        }


        public static object ItemTextEditingStyle
        {
            get { return mItemTextEditingStyle ?? (mItemTextEditingStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ItemTextEditingStyle)); }
        }
        public static object AddButtonStyle
        {
            get { return mAddButtonStyle ?? (mAddButtonStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.AddButtonStyle)); }
        }
        public static object HeaderStyle
        {
            get { return mHeaderStyle ?? (mHeaderStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.HeaderStyle)); }
        }
        public static object DefaultLabelLarge
        {
            get { return mDefaultLabelLarge ?? (mDefaultLabelLarge = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.DefaultLabelLarge)); }
        }
        public static object GraphTitleLabel
        {
            get { return mGraphTitleLabel ?? (mGraphTitleLabel = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.GraphTitleLabel)); }
        }
        public static object ClearItemStyle
        {
            get { return mClearItemStyle ?? (mClearItemStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ClearItemStyle)); }
        }
        public static object ViewModelHeaderStyle
        {
            get { return mViewModelHeaderStyle ?? (mViewModelHeaderStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ViewModelHeaderStyle)); }
        }
        public static object ItemStyle
        {
            get { return mItemStyle ?? (mItemStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ItemStyle)); }
        }
        public static object SelectedItemStyle
        {
            get { return mSelectedItemStyle ?? (mSelectedItemStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.SelectedItemStyle)); }
        }
        public static object Tag1
        {
            get { return mTag1 ?? (mTag1 = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Tag1)); }
        }
        public static object Toolbar
        {
            get { return mToolbar ?? (mToolbar = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.Toolbar)); }
        }
        public static object ToolbarButton
        {
            get { return mToolbarButton ?? (mToolbarButton = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ToolbarButton)); }
        }
        public static object ToolbarButtonDrop
        {
            get { return mToolbarButtonDrop ?? (mToolbarButtonDrop = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ToolbarButtonDown)); }
        }
        public static object HeaderTitleStyle
        {
            get { return mToolbarButtonDrop ?? (mToolbarButtonDrop = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.HeaderTitleStyle)); }
        }
        public static object HeaderSubTitleStyle
        {
            get { return mToolbarButtonDrop ?? (mToolbarButtonDrop = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.HeaderSubTitleStyle)); }
        }
        public static object WizardBoxStyle
        {
            get { return mWizardBoxStyle ?? (mWizardBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardBox)); }
        }
                
        public static object WizardSubBoxStyle
        {
            get { return mWizardSubBoxStyle ?? (mWizardSubBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardSubBox)); }
        }        
        
        public static object WizardActionButtonStyle
        {
            get { return mWizardActionButtonStyle ?? (mWizardActionButtonStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardActionButton)); }
        }
        
        public static IConnectorStyleSchema ConnectorStyleSchemaTriangle
        {
            get { return mConnectorStyleSchemaTriangle ?? (mConnectorStyleSchemaTriangle = InvertGraphEditor.StyleProvider.GetConnectorStyleSchema(ConnectorStyle.Triangle)); }
        }

        public static IConnectorStyleSchema ConnectorStyleSchemaCircle
        {
            get { return mConnectorStyleSchemaCircle ?? (mConnectorStyleSchemaCircle = InvertGraphEditor.StyleProvider.GetConnectorStyleSchema(ConnectorStyle.Circle)); }
        }

        public static INodeStyleSchema NodeStyleSchemaNormal
        {
            get { return mNodeStyleSchemaNormal ?? (mNodeStyleSchemaNormal = InvertGraphEditor.StyleProvider.GetNodeStyleSchema(NodeStyle.Normal)); }
            set { mNodeStyleSchemaNormal = value; }
        }

        public static INodeStyleSchema NodeStyleSchemaMinimalistic
        {
            get { return mNodeStyleSchemaMinimalistic ?? (mNodeStyleSchemaMinimalistic = InvertGraphEditor.StyleProvider.GetNodeStyleSchema(NodeStyle.Minimalistic)); }
            set { mNodeStyleSchemaMinimalistic = value; }
        }

        public static INodeStyleSchema NodeStyleSchemaBold
        {
            get { return mNodeStyleSchemaBold ?? (mNodeStyleSchemaBold = InvertGraphEditor.StyleProvider.GetNodeStyleSchema(NodeStyle.Bold)); }
            set { mNodeStyleSchemaBold = value; }
        }

        public static IBreadcrumbsStyleSchema DefaultBreadcrumbsStyleSchema
        {
            get { return mDefaultBreadcrumbsStyleSchema ?? (mDefaultBreadcrumbsStyleSchema = InvertGraphEditor.StyleProvider.GetBreadcrumbStyleSchema(BreadcrumbsStyle.Default)); }
            set { mDefaultBreadcrumbsStyleSchema = value; }
        }

        public static object BreadcrumbTitleStyle
        {
            get { return mBreadcrumbTitleStyle ?? (mBreadcrumbTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BreadcrumbTitleStyle)); }
            set { mBreadcrumbTitleStyle = value; }
        }      
        
        public static object ListItemTitleStyle
        {
            get { return mListItemTitleStyle ?? (mListItemTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.ListItemTitleStyle)); }
            set { mListItemTitleStyle = value; }
        }

        public static object TabBoxStyle
        {
            get { return mTabBoxStyle ?? (mTabBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TabBox)); }
            set { mTabBoxStyle = value; }
        }

        public static object TabBoxActiveStyle
        {
            get { return mTabBoxActiveStyle ?? (mTabBoxActiveStyle = InvertGraphEditor.StyleProvider.GetStyle((InvertStyles.TabBoxActive))); }
            set { mTabBoxActiveStyle = value; }
        }

        public static object TabTitleStyle
        {
            get { return mTabTitleStyle ?? (mTabTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TabTitle)); }
            set { mTabTitleStyle = value; }
        }

        public static object TabCloseButton
        {
            get { return mTabCloseButton ?? (mTabCloseButton = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TabCloseButton)); }
            set { mTabCloseButton = value; }
        }

        public static object BreadcrumbBoxStyle
        {
            get { return mBreadcrumbBoxStyle ?? (mBreadcrumbBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BreadcrumbBoxStyle) ); }
            set { mBreadcrumbBoxStyle = value; }
        }      
        
        public static object BreadcrumbBoxActiveStyle
        {
            get { return mBreadcrumbBoxActiveStyle ?? (mBreadcrumbBoxActiveStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.BreadcrumbBoxActiveStyle)); }
            set { mBreadcrumbBoxActiveStyle = value; }
        }

        public static object WizardActionTitleStyle
        {
            get { return mWizardActionTitleStyle ?? (mWizardActionTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardActionTitle)); ; }
            set { mWizardActionTitleStyle = value; }
        }    
        
        public static object WizardSubBoxTitleStyle
        {
            get { return mWizardSubBoxTitleStyle ?? (mWizardSubBoxTitleStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardSubBoxTitle)); ; }
            set { mWizardSubBoxTitleStyle = value; }
        }

        public static object TooltipBoxStyle
        {
            get { return mTooltipBoxStyle ?? (mTooltipBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.TooltipBox)); }
            set { mTooltipBoxStyle = value; }
        }

        public static object WizardListItemBoxStyle
        {
            get { return mWizardListItemBoxStyle ?? (mWizardListItemBoxStyle = InvertGraphEditor.StyleProvider.GetStyle(InvertStyles.WizardListItemBox)); }
            set { mWizardListItemBoxStyle = value; }
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